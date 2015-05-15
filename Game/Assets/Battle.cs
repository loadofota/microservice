using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using Packet;
using System.Collections.Generic;
using UnityEngine.UI;
using BestHTTP.WebSocket;
using System;

public class Battle : MonoBehaviour
{
    WebSocket ws;
    Queue<System.Action> actionQueue = new Queue<System.Action>();

    public void StartBattle()
    {
        if (ws == null)
        {
            ws = new WebSocket(new Uri("ws://gasbank.mmzhanqilai.com:19191/ws"));

			ws.OnMessage += OnMessageReceived;
			ws.OnOpen += OnOpen;

            ws.Open();
        }
        else if (ws != null && ws.IsOpen == false)
        {
            ws.Close();
            ws.Open();
        }
		else if (ws != null && ws.IsOpen == true)
		{
			OnOpen(ws);
		}

        GetComponent<Button>().interactable = false;
    }

    void Update()
    {
        lock (actionQueue)
        {
            while (actionQueue.Count > 0)
            {
                var action = actionQueue.Dequeue();
                if (action != null)
                {
                    action();
                }

            }
        }
    }

	void OnOpen (WebSocket webSocket)
	{
		var serializedObject = JsonConvert.SerializeObject(new BattleCommand(), Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
		
		ws.Send(serializedObject);
	}

	void OnMessageReceived (WebSocket webSocket, string message)
	{
		Debug.Log("StartBattle reply: " + message);
		
		lock (actionQueue)
		{
			actionQueue.Enqueue(() =>
			                    {
				Context.ShowPopup("전투", "전투했습니다.\n" + message, "확인", "",
				                  () =>
				                  {
					Context.ClosePopup();
				},
				null);
				
				GetComponent<Button>().interactable = true;
			});
		}
	}
}
