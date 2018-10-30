// ReSharper disable SuggestVarOrType_SimpleTypes

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;  
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;  
using System.Threading;
using System.Threading.Tasks;

namespace UnityMMOServer
{

    public delegate void HANDLE_REQUEST<in T>(OpCode msg, T data);
    
// State object for reading client data asynchronously  
    public class StateObject
    {
        // Client  socket.  
        public Socket workSocket = null;

        public static int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];

        public MemoryStream ms = new MemoryStream();
        
        public PlayerEntity? Entity = null;

        public void Reset()
        {
            ms.Flush();
        }
    }

    
    public class ZoneServer
    {
        // Thread signal.  
        private static ManualResetEvent allDone = new ManualResetEvent(false);

        private static List<Socket> _clients = new List<Socket>();

        private static int headerSize = Marshal.SizeOf(typeof(PlayerEntity)) + sizeof(int);
        
        public static void StartListening()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    // Wait until a connection is made before continuing.  
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the socket that handles the client request.  
            Socket listener = (Socket) ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            _clients.Add(handler);
           
            // Create the state object.  
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject) ar.AsyncState;
            Socket handler = state.workSocket;
            
            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                if (content.IndexOf("<EOF>") > -1)
                {

                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
            else
            {
                //Nothing Received, Check if Stream has Data
                if (state.ms.Length > 0)
                {
                    // Process Data
                }
                else
                {
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private static void SendAll(Socket handler, byte[] data)
        {
            Parallel.ForEach(_clients, (client) =>
            {
                
            });
        }
        private static void Send(Socket client, byte[] data)
        {
            // Begin sending the data to the remote device.  
            client.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket) ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        
        public static byte[] Serialize (List<PlayerEntity> player, List<Entity> entities) {
            
            using (MemoryStream ms = new MemoryStream()) {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();

                    var entityLength = entities.Count;
                    var entitySize = Marshal.SizeOf(typeof(Entity)) * entityLength;
                    
                    writer.Write(entitySize);
                    
                    writer.Write(getBytes(player));
                    
                    
                    for (var idx = 0; idx < entityLength; idx++)
                    {
                        writer.Write(getBytes(entities[idx]));
                    }
                    
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    Console.WriteLine($"Time to Serialize: {elapsedMs} ms");
                    
                    return ms.ToArray();
                }
            }
        }
        
    }
}