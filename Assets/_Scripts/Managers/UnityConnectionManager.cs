using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using GameServer.Models;
using NetcodeIO.NET;
using ReliableNetcode;
using Unity.MMO.Client;
using UnityEngine;
using UnityMMO.Models;
using UnityNetcodeIO;

namespace UnityMMO.Manager
{
	public class UnityConnectionManager : MonoBehaviour
	{
		public delegate void ChatMessageEvent(ChatMessage chatMessage);
		public delegate void ConnectionStatus(NetcodeClientStatus status);
		public delegate void EntityReceived(Entity entity);

		private static UnityConnectionManager _instance = null;

		private NetcodeClient 		_client;
		private ReliableEndpoint 	_reliableClient;

		private const int HEADER_OFFSET = 2;
		
		static readonly byte[] _privateKey = new byte[]
		{
			0x60, 0x6a, 0xbe, 0x6e, 0xc9, 0x19, 0x10, 0xea,
			0x9a, 0x65, 0x62, 0xf6, 0x6f, 0x2b, 0x30, 0xe4,
			0x43, 0x71, 0xd6, 0x2c, 0xd1, 0x99, 0x27, 0x26,
			0x6b, 0x3c, 0x60, 0xf4, 0xb7, 0x15, 0xab, 0xa1,
		};

		public static ChatMessageEvent OnReceiveChatMessage;
		public static ConnectionStatus OnConnectionStatus;
		public static EntityReceived OnEntityReceived;

		void Awake()
		{
			//Check if instance already exists
			if (_instance == null)

				//if not, set instance to this
				_instance = this;

			//If instance already exists and it's not this:
			else if (_instance != this)

				//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
				Destroy(gameObject);

			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad(gameObject);
		}

		void FixedUpdate()
		{
			if (_client.Status == NetcodeClientStatus.Connected)
			{
				_reliableClient?.Update();
			}
		}

		// Use this for initialization
		void Start()
		{
			Debug.Log("Starting Connection...");
			UnityNetcode.QuerySupport(supportStatus =>
			{
				Debug.Log("Network Support:" + supportStatus);
				switch (supportStatus)
				{
					case NetcodeIOSupportStatus.HelperNotInstalled:
					case NetcodeIOSupportStatus.Unavailable:
						return;
					case NetcodeIOSupportStatus.Available:
						Connect(NetcodeIOClientProtocol.IPv4);
						break;
					case NetcodeIOSupportStatus.Unknown:
						break;
					default:
						Connect(NetcodeIOClientProtocol.IPv4);
						break;
				}
			});
		}

		#region Connection Setup

		private void Connect(NetcodeIOClientProtocol protocol)
		{
			UnityNetcode.CreateClient(protocol, client =>
			{
				_client = client;
				var connectToken = generateToken();
				client.Connect(connectToken, OnConnectSuccess, OnConnectFailure);
			});
		}

		private void OnStatusChange(NetcodeClientStatus status)
		{
			if (_client.Status == NetcodeClientStatus.Disconnected)
			{
				Application.Quit();
			}
			else
			{
				OnConnectionStatus?.Invoke(status);
			}
		}

		private void OnConnectSuccess()
		{
			_client.QueryStatus(OnStatusChange);

			_reliableClient = new ReliableEndpoint();
			_reliableClient.ReceiveCallback += OnReliableReceiveCallback;
			_reliableClient.TransmitCallback += OnReliableTransmitCallback;
			_client.AddPayloadListener(OnMessageReceive);
		}

		private void OnConnectFailure(string error)
		{
			Debug.Log("Could not connect to server: " + error);
		}

		#endregion

		void OnMessageReceive(NetcodeClient client, NetcodePacket packet)
		{
			var payload = packet.PacketBuffer.InternalBuffer;
			_reliableClient.ReceivePacket(payload, packet.PacketBuffer.Length);
		}

		public void Send(byte[] payload, int payloadSize)
		{
			//Debug.Log($"Sending Payload of {payloadSize} bytes.");
			_reliableClient.SendMessage(payload, payloadSize, QosType.Unreliable);
		}
		
		public void Send<T>(T data, MessageType type, QosType qosType = QosType.Unreliable)
		{
			if (_client.Status == NetcodeClientStatus.Connected)
			{
				byte[] objData = StructTools.RawSerialize(data);
				byte[] objType = BitConverter.GetBytes((short) type);
				byte[] payload = objType.Concat(objData).ToArray();
				_reliableClient.SendMessage(payload, payload.Length, QosType.Unreliable);
			}
		}

		private byte[] generateToken()
		{
			List<IPEndPoint> addressList = new List<IPEndPoint>();
			addressList.Add(new IPEndPoint(IPAddress.Loopback, 8559));

			var serverAddress = addressList.ToArray();

			TokenFactory tokenFactory = new TokenFactory(
				1UL, // must be the same protocol ID as passed to both client and server constructors
				_privateKey // byte[32], must be the same as the private key passed to the Server constructor
			);

			return tokenFactory.GenerateConnectToken(
				serverAddress, // IPEndPoint[] list of addresses the client can connect to. Must have at least one and no more than 32.
				60, // in how many seconds will the token expire
				60, // how long it takes until a connection attempt times out and the client tries the next server.
				1UL, // ulong token sequence number used to uniquely identify a connect token.
				1UL, // ulong ID used to uniquely identify this client
				new byte[256] // byte[], up to 256 bytes of arbitrary user data (available to the server as RemoteClient.UserData)
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
			//Extract Header Type
			MessageType type = (MessageType) BitConverter.ToInt16(payload, 0);
			if (type == MessageType.Chat)
			{
				var msg = StructTools.RawDeserialize<ChatMessage>(payload, HEADER_OFFSET);			
				OnReceiveChatMessage?.Invoke(msg);
			}

			if (type == MessageType.Entity)
			{
				Debug.Log($"Entity: {payloadSize - 2}");
				var entity = StructTools.RawDeserialize<Entity>(payload, 2);
				OnEntityReceived?.Invoke(entity);
			}
		}

		private void OnDestroy()
		{
			UnityNetcode.DestroyClient(_client);
		}
	}
}
