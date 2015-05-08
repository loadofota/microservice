﻿using UnityEngine;
using System.Collections;
using WebSocketSharp;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using UnityEngine.UI;
using Packet;
using System.Collections.Generic;

public class Login : MonoBehaviour
{
    public InputField inputFieldId;
    public InputField inputFieldPw;

    WebSocket ws;
    Queue<System.Action> actionQueue = new Queue<System.Action>();

    public void StartLogin()
    {
        if (ws == null)
        {
            ws = new WebSocket("ws://gasbank.mmzhanqilai.com:19191/ws");

            ws.OnMessage += (sender, e) =>
            {
                Debug.Log("StartLogin reply: " + e.Data);

                lock (actionQueue)
                {
                    actionQueue.Enqueue(() =>
                    {
                        Context.ShowPopup("로그인", "로그인했습니다.\n" + e.Data, "확인", "",
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
            ws.Connect();
        }

        var serializedObject = JsonConvert.SerializeObject(new LoginCommand { id = inputFieldId.text, pw = inputFieldPw.text }, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

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
