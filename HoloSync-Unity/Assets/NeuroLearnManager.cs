using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WebSocketClient))]
public class NeuroLearnManager : MonoBehaviour
{
    public float requestTimeGap = 1f; // ms
    WebSocketClient webSocketClient;

    // Start is called before the first frame update
    void Start()
    {
        webSocketClient = GetComponent<WebSocketClient>();
        webSocketClient.StartWebSocket();
        StartCoroutine("NeuroLearnRequest",requestTimeGap);
    }

    void NeuroLearnRequest()
    {
        if (webSocketClient.isWebSocketStarted)
        {
            webSocketClient.SendWebSocketMessage("data");
        }

        else
        {
            webSocketClient.StartWebSocket();
        }
    }
}
