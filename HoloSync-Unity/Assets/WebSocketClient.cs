using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using UnityEngine.Events;

[RequireComponent(typeof(MRTKUIHandler))]
public class WebSocketClient : MonoBehaviour
{
    WebSocket websocket;
    [HideInInspector]
    public string message;
    public string web = "ws://127.0.0.1:5001";
    public UnityEvent onMessageReceived;
    public UnityEvent onDisableInteraction;
    public UnityEvent onEnableInteraction;
    public bool isWebSocketStarted = false;
    MRTKUIHandler MRTKUI;
    // Start is called before the first frame update
    private void Awake()
    {
        MRTKUI = GetComponent<MRTKUIHandler>();
    }
    public async void StartWebSocket()
    {
        websocket = new WebSocket(web);
        websocket.OnOpen += () =>
        {
            SendWebSocketMessage("data");
            MRTKUI.UpdateDataPacket("Connection open!");
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            MRTKUI.UpdateDataPacket("Error! " + e);
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            MRTKUI.UpdateDataPacket("Connection closed!");
            Debug.Log("Connection closed!");

        };

        websocket.OnMessage += (bytes) =>
        {
            message = System.Text.Encoding.UTF8.GetString(bytes);
            MRTKUI.UpdateDataPacket(message);
            Debug.Log("OnMessage! " + message);
            onMessageReceived.Invoke();
        };

        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (isWebSocketStarted)
        {
            websocket.DispatchMessageQueue();
        }
#endif
    }

    public async void SendWebSocketMessage(string request)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText(request);
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

}