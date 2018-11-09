// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable InconsistentNaming
using System.Collections.Generic;
using System.Net;
using NetcodeIO.NET;
using ReliableNetcode;
using UnityEngine;

namespace Unity.MMO.Client
{      
    public class ConnectionManager : MonoBehaviour
    {
        private static ConnectionManager instance = null;

        static readonly byte[] _privateKey = new byte[]
        {
            0x60, 0x6a, 0xbe, 0x6e, 0xc9, 0x19, 0x10, 0xea,
            0x9a, 0x65, 0x62, 0xf6, 0x6f, 0x2b, 0x30, 0xe4,
            0x43, 0x71, 0xd6, 0x2c, 0xd1, 0x99, 0x27, 0x26,
            0x6b, 0x3c, 0x60, 0xf4, 0xb7, 0x15, 0xab, 0xa1,
        };
        
        private NetcodeIO.NET.Client _client;
        
        private ReliableEndpoint _reliableClient;
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
            _client = new NetcodeIO.NET.Client();
            // Called when the client's state has changed
            // Use this to detect when a client has connected to a server, or has been disconnected from a server, or connection times out, etc.
            _client.OnStateChanged += OnClientStateChanged;			// void( ClientState state )
            // Called when a payload has been received from the server
            // Note that you should not keep a reference to the payload, as it will be returned to a pool after this call completes.
            _client.OnMessageReceived += OnClientMessageReceivedHandler;		// void( byte[] payload, int payloadSize )

            _reliableClient = new ReliableEndpoint();
            _reliableClient.ReceiveCallback = OnReliableReceiveCallback;
            _reliableClient.TransmitCallback = OnReliableTransmitCallback;
            

            var connectToken  = generateToken();
            _client.Connect(connectToken);
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }

        private byte[] generateToken()
        {
            List<IPEndPoint> addressList = new List<IPEndPoint>();
            addressList.Add(new IPEndPoint(IPAddress.Loopback, 8559));

            var serverAddress = addressList.ToArray();
            
            TokenFactory tokenFactory = new TokenFactory(
                1,		// must be the same protocol ID as passed to both client and server constructors
                _privateKey		// byte[32], must be the same as the private key passed to the Server constructor
            );
            
            return tokenFactory.GenerateConnectToken(
                serverAddress,		// IPEndPoint[] list of addresses the client can connect to. Must have at least one and no more than 32.
                30,		// in how many seconds will the token expire
                5,		// how long it takes until a connection attempt times out and the client tries the next server.
                1UL,		// ulong token sequence number used to uniquely identify a connect token.
                1UL,		// ulong ID used to uniquely identify this client
                new byte[256]		// byte[], up to 256 bytes of arbitrary user data (available to the server as RemoteClient.UserData)
            );
            
        }

        public void Send(byte[] payload, int payloadSize)
        {
            _reliableClient.SendMessage(payload, payloadSize, QosType.Unreliable);
        }
        
        public void SendReliable(byte[] payload, int payloadSize)
        {
            _reliableClient.SendMessage(payload, payloadSize, QosType.Reliable);
        }
              
        public void SendUnreliableOrdered (byte[] payload, int payloadSize)
        {
            _reliableClient.SendMessage(payload, payloadSize, QosType.UnreliableOrdered);
        }
        
        private void OnClientStateChanged(ClientState state)
        {
            // Check Status of Client
        }

        private void OnReliableTransmitCallback(byte[] payload, int payloadSize)
        {
            _client.Send( payload, payloadSize );
        }
        
        private void OnClientMessageReceivedHandler(byte[] payload, int payloadSize)
        {
            _reliableClient.ReceivePacket( payload, payloadSize );
        }

        
        private void OnReliableReceiveCallback(byte[] payload, int payloadSize)
        {
            // Process Data
        }
    }
}