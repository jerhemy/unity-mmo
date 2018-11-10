using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.MMO.Client;
using UnityEngine;

public class ChatEvents : MonoBehaviour {
	
	public GameObject ChatInput;
	public GameObject ChatLog;
	
	public ConnectionManager _connectionManager;

	public void Start()
	{
		ConnectionManager.OnReceiveChatMessage += UpdateChat;
	}

	public void UpdateChat(string message)
	{
		ChatLog.GetComponent<TextMeshProUGUI>().text += message + "\n";
	}
	
	public void OnChatEnter()
	{
		var text = ChatInput.GetComponent<TMP_InputField>().text;
		ChatInput.GetComponent<TMP_InputField>().text = "";

		byte[] data = Encoding.Unicode.GetBytes(text);
		byte[] type = BitConverter.GetBytes((byte)MessageType.Chat);
		byte[] payload = type.Concat(data).ToArray();
		
		_connectionManager.Send(payload, payload.Length);	
	}
}
