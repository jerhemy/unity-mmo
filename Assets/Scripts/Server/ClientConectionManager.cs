using System;
using System.Collections;
using System.Collections.Generic;
using NetcodeIO.NET;
using UnityEngine;
using UnityEngine.tvOS;
using UnityNetcodeIO;

namespace Server
{
    public class ClientConnectionManager : MonoBehaviour
    {
        public delegate void ClientServerConnect(RemoteClient client);
        public delegate void ClientServerDisconnect(RemoteClient client);
        public delegate void ClientServerDataReceived(RemoteClient client, ByteBuffer payload);

        public static ClientServerConnect OnClientServerConnect;
        public static ClientServerDisconnect OnClientServerDisconnect;
        public static ClientServerDataReceived OnClientServerDataReceived;
        
        private NetcodeServer _server;

        private string _ip;
        private int _port;

        private ulong protocolID;

        private int maxClients;

        private byte[] privateKey = new byte[32];
        
        // Start is called before the first frame update
        void Start()
        {
            try
            {
                 _server = UnityNetcode.CreateServer(
                    _ip, // string public IP clients will connect to
                    _port, // port clients will connect to
                    protocolID, // ulong number used to identify this application. must be the same as the token server generating connect tokens.
                    maxClients, // maximum number of clients who can connect
                    privateKey); // byte[32] private encryption key shared between token server and game server

                // Called when a client connects to the server
                _server.ClientConnectedEvent.AddListener(ClientConnected); // void( RemoteClient client );

                // Called when a client disconnects from the server
                _server.ClientDisconnectedEvent.AddListener(ClientDisconnected); // void( RemoteClient client );

                // Called when a client sends a payload to the server
                // Note that byte[] payload will be returned to a pool after the callback, so don't keep a reference to it.
                _server.ClientMessageEvent.AddListener(ClientMessageEvent); // void( RemoteClient sender, ByteBuffer payload );

                _server.StartServer(); // start listening for clients
            }
            catch (Exception e)
            {
                Application.Quit();
            }
        }


        private void ClientConnected(RemoteClient client)
        {
            OnClientServerConnect?.Invoke(client);
        }

        private void ClientDisconnected(RemoteClient client)
        {
            OnClientServerDisconnect?.Invoke(client);
        }

        private void ClientMessageEvent(RemoteClient sender, ByteBuffer payload)
        {
            OnClientServerDataReceived?.Invoke(sender, payload);
        }
        
        private void OnDestroy()
        {

            _server.StopServer(); // stop server and disconnect any clients
            _server.Dispose();
        }
    }
}
