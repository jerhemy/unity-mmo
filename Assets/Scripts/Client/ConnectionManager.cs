// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable InconsistentNaming
using System;
using System.IO;
using System.Net;  
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;  
using System.Threading.Tasks;  
using System.Text;
using UnityEditor.MemoryProfiler;
using UnityEngine;


namespace Server
{
    public delegate void DataReceived(byte[] data);

    public class StateObject {

        // Client socket.  
        public Socket workSocket = null;  
        // Size of receive buffer.  
        public const int BufferSize = 256;  
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];  
        
        private MemoryStream ms = new MemoryStream();

        private int bytesLoaded = 0;

        public byte[] getData()
        {
            return ms.ToArray();
        }
        
        public void Merge(byte[] data, int bytesRead)
        {
            ms.Write(data, bytesLoaded, BufferSize);
            bytesLoaded += bytesRead;
        }

        public bool hasData()
        {
            return ms.Length > 0;
        }
    }  
        
    public class ConnectionManager : MonoBehaviour
    {
        private static ConnectionManager instance = null;
        private const int PORT = 11000;

        public static DataReceived OnDataReceived;
        
        // ManualResetEvent instances signal completion.  
        private static readonly ManualResetEvent connectDone = new ManualResetEvent(false);  
        private static readonly ManualResetEvent sendDone = new ManualResetEvent(false);  
        private static readonly ManualResetEvent receiveDone = new ManualResetEvent(false);  
        
        private static readonly object padlock = new object();
        
        void Awake()
        {
            //Check if instance already exists
            if (instance == null)
                
                //if not, set instance to this
                instance = this;
            
            //If instance already exists and it's not this:
            else if (instance != this)
                
                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);    
            
            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            Connect();
        }
        
        
        public void Connect()
        {
            try {  
                // Establish the remote endpoint for the socket.  
                IPAddress ipAddress = IPAddress.Loopback;
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, PORT);  

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);  

                // Connect to the remote endpoint.  
                client.BeginConnect( remoteEP, new AsyncCallback(ConnectCallback), client);  
                connectDone.WaitOne();  
                
                Receive(client);

            } catch (Exception e) {  
                Console.WriteLine(e.ToString());  
                Debug.Log(e.ToString());
            }  
        }
        
        private static void ConnectCallback(IAsyncResult ar) {  
            try {  
                // Retrieve the socket from the state object.  
                Socket client = (Socket) ar.AsyncState;  
    
                // Complete the connection.  
                client.EndConnect(ar);  
    
                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());  
    
                // Signal that the connection has been made.  
                connectDone.Set();  
            } catch (Exception e) {  
                Console.WriteLine(e.ToString());  
            }  
        }  

        private static void Receive(Socket client) {  
            try {  
                // Create the state object.  
                StateObject state = new StateObject {workSocket = client};

                // Begin receiving the data from the remote device.  
                client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                
            } catch (Exception e) {  
                Console.WriteLine(e.ToString());  
            }  
        }  

    private static void ReceiveCallback( IAsyncResult ar ) {  
        try {  
            // Retrieve the state object and the client socket   
            // from the asynchronous state object.  
            StateObject state = (StateObject) ar.AsyncState;  
            Socket client = state.workSocket;  

            // Read data from the remote device.  
            int bytesRead = client.EndReceive(ar);  
            
            // Did we receive data?
            if (bytesRead > 0)
            {
                state.Merge(state.buffer, bytesRead);     
                // Store Data and Get More
                client.BeginReceive(state.buffer,0,StateObject.BufferSize,0, new AsyncCallback(ReceiveCallback), state); 
            }
            else
            {
                if (state.hasData())
                {
                    OnDataReceived(state.getData());
                }

                Receive(client);
            }
        } catch (Exception e) {  
            Console.WriteLine(e.ToString());  
        }  
    }  

    private static void Send<T>(Socket client, T data) {  
        // Convert the string data to byte data using ASCII encoding.  
        byte[] byteData;

        using (MemoryStream ms = new MemoryStream())
        {
            IFormatter writer = new BinaryFormatter();
            writer.Serialize(ms, data);

            byteData = ms.ToArray();
        }
        
        // Begin sending the data to the remote device.  
        client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);  
    }  

    private static void SendCallback(IAsyncResult ar) {  
        try {  
            // Retrieve the socket from the state object.  
            Socket client = (Socket) ar.AsyncState;  

            // Complete sending the data to the remote device.  
            int bytesSent = client.EndSend(ar);  
            Console.WriteLine("Sent {0} bytes to server.", bytesSent);  

            // Signal that all bytes have been sent.  
            sendDone.Set();  
        } catch (Exception e) {  
            Console.WriteLine(e.ToString());  
        }  
    }
    }
}