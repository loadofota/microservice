using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using Packet;
using System.Collections.Generic;
using UnityEngine.UI;
using BestHTTP.WebSocket;
using System;

namespace Packet
{
	class Login2Command
	{
		public string id {get;set;}
		public string pw {get;set;}
	}
}


public class Login : MonoBehaviour
{
	public InputField inputFieldId;
	public InputField inputFieldPw;
	WebSocket ws;
	Queue<System.Action> actionQueue = new Queue<System.Action> ();

	public void StartLogin ()
	{
		if (ws == null) {
			ws = new WebSocket (new Uri ("ws://gbjp.cloudapp.net:19191/ws"));

			ws.OnOpen += OnOpen;
			ws.OnMessage += OnMessageReceived;
            
			ws.Open ();
		} else if (ws != null && ws.IsOpen == false) {
			ws.Close ();
			ws.Open ();
		} else if (ws != null && ws.IsOpen == true) {
			OnOpen (ws);
		}

		GetComponent<Button> ().interactable = false;
	}

	void Update ()
	{
		lock (actionQueue) {
			while (actionQueue.Count > 0) {
				var action = actionQueue.Dequeue ();
				if (action != null) {
					action ();
				}

			}
		}
	}



	void OnOpen (WebSocket webSocket)
	{
		var serializedObject = JsonConvert.SerializeObject (new Login2Command { id = inputFieldId.text, pw = inputFieldPw.text }, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
		
		ws.Send (serializedObject);
	}

	void OnMessageReceived (WebSocket webSocket, string message)
	{
		Debug.Log ("StartLogin reply: " + message);
		
		lock (actionQueue) {
			actionQueue.Enqueue (() =>
			{
				Context.ShowPopup ("로그인", "로그인했습니다.\n" + message, "확인", "",
				                  () =>
				{
					Context.ClosePopup ();
				},
				null);
				
				GetComponent<Button> ().interactable = true;
			});
		}
	}
}
