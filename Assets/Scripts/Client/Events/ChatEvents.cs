using GameServer.Models;
using TMPro;
using Unity.MMO.Client;
using UnityEngine;
using UnityEngine.UI;
using UnityMMO.Manager;

public class ChatEvents : MonoBehaviour
{

	public static bool isChatFocused = false; 
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

	public void OnFocus()
	{
			
	}
	
	public void UpdateChat(ChatMessage chatMessage)
	{
		textUI.text +=  $"{chatMessage.@from}:{chatMessage.message}\n";
	}
	
	public void OnChatEnter()
	{
		var text = ChatInput.GetComponent<TMP_InputField>().text;
		ChatInput.GetComponent<TMP_InputField>().text = "";
		var msg = new ChatMessage {@from = "Me", message = text};
		
		_connectionManager.Send(msg, MessageType.Chat);	
	}

	

}
