using UnityEngine;
using System.Collections;
using WebSocketSharp;
using Newtonsoft.Json;
using Packet;
using System.Collections.Generic;
using UnityEngine.UI;

public class Battle : MonoBehaviour
{
    WebSocket ws;
    Queue<System.Action> actionQueue = new Queue<System.Action>();

    public void StartBattle()
    {
        if (ws == null)
        {
            ws = new WebSocket("ws://gasbank.mmzhanqilai.com:19191/ws");

            ws.OnMessage += (sender, e) =>
            {
                Debug.Log("StartBattle reply: " + e.Data);

                lock (actionQueue)
                {
                    actionQueue.Enqueue(() =>
                    {
                        Context.ShowPopup("전투", "전투했습니다.\n" + e.Data, "확인", "",
                            () =>
                            {
                                Context.ClosePopup();
                            },
                            null);

                        GetComponent<Button>().interactable = true;
                    });
                }
            };

            ws.Connect();
        }
        else if (ws != null && ws.IsAlive == false)
        {
            ws.Close();
            ws.Connect();
        }

        var serializedObject = JsonConvert.SerializeObject(new BattleCommand(), Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

        ws.Send(serializedObject);

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
}
