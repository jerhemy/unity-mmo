//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;  
//using System.Net;  
//using System.Net.Sockets;  
//using System.Threading;  
//using System.Text;  
//
//
///* 
// * Network Update Manager
// * This script is used to update the scene with information about other Entities in the Scene
// * 
// */
//
//public class StateObject {  
//	// Client socket.  
//	public Socket workSocket = null;  
//	// Size of receive buffer.  
//	public const int BufferSize = 256;  
//	// Receive buffer.  
//	public byte[] buffer = new byte[BufferSize];  
//	// Received data string.  
//	public StringBuilder sb = new StringBuilder();  
//}  
//
//
//public class NetworkUpdateManager : MonoBehaviour {
//
//	// Use this for initialization
//	void Start () {
//		try {  
//			// Establish the remote endpoint for the socket.  
//			// The name of the   
//			// remote device is "host.contoso.com".  
//			IPHostEntry ipHostInfo = Dns.GetHostEntry("host.contoso.com");  
//			IPAddress ipAddress = ipHostInfo.AddressList[0];  
//			IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);  
//
//			// Create a TCP/IP socket.  
//			Socket client = new Socket(ipAddress.AddressFamily,  
//				SocketType.Stream, ProtocolType.Tcp);  
//
//			// Connect to the remote endpoint.  
//			client.BeginConnect( remoteEP,   
//				new AsyncCallback(ConnectCallback), client);  
//			connectDone.WaitOne();  
//
//			// Send test data to the remote device.  
//			Send(client,"This is a test<EOF>");  
//			sendDone.WaitOne();  
//
//			// Receive the response from the remote device.  
//			Receive(client);  
//			receiveDone.WaitOne();  
//
//			// Write the response to the console.  
//			Console.WriteLine("Response received : {0}", response);  
//
//			// Release the socket.  
//			client.Shutdown(SocketShutdown.Both);  
//			client.Close();  
//
//		} catch (Exception e) {  
//			Console.WriteLine(e.ToString());  
//		} 
//	}
//	
//	// Update is called once per frame
//	void Update () {
//		
//	}
//}
