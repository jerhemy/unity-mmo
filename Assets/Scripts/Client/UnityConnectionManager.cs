using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using NetcodeIO.NET;
using ReliableNetcode;
using Unity.MMO.Client;
using UnityEngine;
using UnityNetcodeIO;

public class UnityConnectionManager : MonoBehaviour {

	
	private static UnityConnectionManager instance = null;

	private NetcodeClient _client;
	private ReliableEndpoint _reliableClient;
	
	static readonly byte[] _privateKey = new byte[]
	{
		0x60, 0x6a, 0xbe, 0x6e, 0xc9, 0x19, 0x10, 0xea,
		0x9a, 0x65, 0x62, 0xf6, 0x6f, 0x2b, 0x30, 0xe4,
		0x43, 0x71, 0xd6, 0x2c, 0xd1, 0x99, 0x27, 0x26,
		0x6b, 0x3c, 0x60, 0xf4, 0xb7, 0x15, 0xab, 0xa1,
	};
	
	public delegate void ReceiveChatMessage(string msg);
	public static ReceiveChatMessage OnReceiveChatMessage;
	
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
	
	// Use this for initialization
	void Start () {
		
		// check for Netcode.IO extension
		// Will provide NetcodeIOSupportStatus enum, either:
		// Available, if Netcode.IO is available and the standalone helper is installed (or if in standalone),
		// Unavailable, if Netcode.IO is unsupported (direct user to install extension)
		// HelperNotInstalled, if Netcode.IO is available but the standalone helper is not installed (direct user to install the standalone helper)
		UnityNetcode.QuerySupport( (supportStatus) =>
		{
			if (supportStatus != NetcodeIOSupportStatus.Available) return;
			
			UnityNetcode.CreateClient( NetcodeIOClientProtocol.IPv4, (client) =>
			{
				var connectToken = generateToken();
				client.Connect( connectToken, () =>
				{
					Debug.Log("Connected");
					//StartListening();
					_client = client;
					_reliableClient = new ReliableEndpoint();
					_reliableClient.ReceiveCallback += OnReliableReceiveCallback;
					_reliableClient.TransmitCallback += OnReliableTransmitCallback;
					
					_client.AddPayloadListener(OnMessageReceive);
					
				}, ( err ) =>
				{
					Debug.Log("Cound not connect to server");
				} );
				
			} );
//			_reliableClient = new ReliableEndpoint();
//			_reliableClient.ReceiveCallback += OnReliableReceiveCallback;
//			_reliableClient.TransmitCallback += OnReliableTransmitCallback;
			//Connect();
		} );
		

	}
	
	void OnMessageReceive(NetcodeClient client, NetcodePacket packet)
	{
		var payload = packet.PacketBuffer.InternalBuffer;
		_reliableClient.ReceivePacket(payload, packet.PacketBuffer.Length);
	}
	
	public void Send(byte[] payload, int payloadSize)
	{
		Debug.Log($"Sending Payload of {payloadSize} bytes.");
		_reliableClient.SendMessage(payload, payloadSize, QosType.Unreliable);
		
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
			60,		// in how many seconds will the token expire
			60,		// how long it takes until a connection attempt times out and the client tries the next server.
			1UL,		// ulong token sequence number used to uniquely identify a connect token.
			1UL,		// ulong ID used to uniquely identify this client
			new byte[256]		// byte[], up to 256 bytes of arbitrary user data (available to the server as RemoteClient.UserData)
		);
            
	}
	
	private void OnReliableTransmitCallback(byte[] payload, int payloadSize)
	{
		if (_client.Status == NetcodeClientStatus.Connected)
		{
			_client.Send(payload, payloadSize);
		}
}
        
	private void OnReliableReceiveCallback(byte[] payload, int payloadSize)
	{
		Debug.Log($"Received Payload of {payloadSize} bytes.");
		MessageType type = (MessageType)BitConverter.ToInt16(payload, 0);
		Debug.Log($"Type: {(MessageType) Enum.Parse(typeof(MessageType), type.ToString())}");
		if (type == MessageType.Chat)
		{
			
			var message = Encoding.ASCII.GetString(payload, 2, payloadSize - 2);
			Debug.Log($"Message Received: {message}");
			OnReceiveChatMessage?.Invoke(message);
		}
	}

	private void OnDestroy()
	{
		UnityNetcode.DestroyClient( _client );
	}
}
