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
	
	public ConnectionManager _connectionManager;
	public TextMeshProUGUI textUI;
	public Text textBox;
	
	private string textLog = "";
	
	public void Awake()
	{
		ConnectionManager.OnReceiveChatMessage += UpdateChat;	
	}

	public void UpdateChat(string message)
	{
		Debug.Log($"Message: {message}");
		var text = textUI.text += message + "\n";
		textUI.SetText(text);
		textBox.text = message;
	}
	
	public void OnChatEnter()
	{
		var text = ChatInput.GetComponent<TMP_InputField>().text;
		ChatInput.GetComponent<TMP_InputField>().text = "";

		byte[] data = Encoding.ASCII.GetBytes(text);
		byte[] type = BitConverter.GetBytes((byte)MessageType.Chat);
		byte[] payload = type.Concat(data).ToArray();
		
		_connectionManager.Send(payload, payload.Length);	
	}

}
