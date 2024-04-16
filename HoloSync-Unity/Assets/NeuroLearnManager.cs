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
        StartCoroutine("NeuroLearnRequest",requestTimeGap);
    }

    void NeuroLearnRequest()
    {
        webSocketClient.SendWebSocketMessage("GetProcessedData");
    }
}
