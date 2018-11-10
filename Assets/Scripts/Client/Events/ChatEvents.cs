using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using TMPro;
using Unity.MMO.Client;
using UnityEngine;
using UnityEngine.UI;

public class ChatEvents : MonoBehaviour {
	
	public GameObject ChatInput;
	
	public UnityConnectionManager _connectionManager;
	//public ConnectionManager _connectionManager;
	public TextMeshProUGUI textUI;
	public Text textBox;
	
	private string textLog = "";
	
	public void Awake()
	{
		UnityConnectionManager.OnReceiveChatMessage += UpdateChat;	
		//ConnectionManager.OnReceiveChatMessage += UpdateChat;	
	}

	public void UpdateChat(string message)
	{
		Debug.Log($"Message: {message}");
		textUI.text += message + "\n";
	}
	
	public void OnChatEnter()
	{
		var text = ChatInput.GetComponent<TMP_InputField>().text;
		ChatInput.GetComponent<TMP_InputField>().text = "";

		byte[] data = Encoding.ASCII.GetBytes(text);
		byte[] type = BitConverter.GetBytes((short)MessageType.Chat);
		byte[] payload = type.Concat(data).ToArray();
		
		_connectionManager.Send(payload, payload.Length);	
	}

}
