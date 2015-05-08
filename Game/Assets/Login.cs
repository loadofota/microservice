using UnityEngine;
using System.Collections;
using WebSocketSharp;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using UnityEngine.UI;
using Packet;

public class Login : MonoBehaviour
{
    public InputField inputFieldId;
    public InputField inputFieldPw;

    WebSocket ws;

    public void StartLogin()
    {
        if (ws == null)
        {
            ws = new WebSocket("ws://gasbank.mmzhanqilai.com:19191/ws");

            ws.OnMessage += (sender, e) =>
            {
                Debug.Log("Server says: " + e.Data);
            };

            ws.Connect();
        }
        else if (ws != null && ws.IsAlive == false)
        {
            ws.Connect();
        }
        
        var serializedObject = JsonConvert.SerializeObject(new LoginCommand { id = inputFieldId.text, pw = inputFieldPw.text }, Formatting.None, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

        ws.Send(serializedObject);
    }
}
