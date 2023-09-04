using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;
using System.Threading.Tasks;

public class TCPClientHololens : MonoBehaviour
{
    public MRTKTMPInputField ip_input;
    public MRTKTMPInputField port_input;
    public TextMeshProUGUI consoleText;

    System.Net.Sockets.TcpClient client;
    System.Net.Sockets.NetworkStream stream;
    private Task exchangeTask;
    public int lastPacketToInt = -1;
    public void Start()
    {
        
    }

    public async void ConnectByUI()
    {
        await ConnectUWP(ip_input.text, port_input.text);
    }

    public async void Connect(string host, string port)
    {
        await ConnectUWP(host, port);
    }

    private async Task ConnectUWP(string host, string port)
    {
        try
        {
            client = new System.Net.Sockets.TcpClient();
            Debug.Log("Socket built successfully");  // Alert when socket is built
            await client.ConnectAsync(host, Int32.Parse(port));
            stream = client.GetStream();
            RestartExchange();

        }
        catch (Exception e)
        {
            // Do something
            Debug.Log(e.ToString());
        }
    }

    private bool exchangeStopRequested = false;
    private string lastPacket = null;

    public void RestartExchange()
    {
        if (exchangeTask != null) Task.Run(() => StopExchange());
        exchangeStopRequested = false;
        exchangeTask = Task.Run(() => ExchangePackets());
    }

    public void Update()
    {
        if (lastPacket != null)
        {
            //do something
            ShowData(lastPacket);
            int.TryParse(lastPacket, out lastPacketToInt);
        }
    }

    public void ExchangePackets()
    {
        while (!exchangeStopRequested)
        {
            string received = null;
            byte[] bytes = new byte[client.ReceiveBufferSize];
            int recv = 0;
            while (true)
            {
                recv = stream.Read(bytes, 0, client.ReceiveBufferSize);
                received = Encoding.UTF8.GetString(bytes, 0, recv);
                lastPacket = received;
                Debug.Log(received);
            }
        }
    }

    private void ShowData(string data)
    {
        if (data == null)
        {
            consoleText.text = "Received a frame but data was null";
            return;
        }

        consoleText.text = data;
    }

    public async Task StopExchange()
    {
        exchangeStopRequested = true;
        if (exchangeTask != null)
        {
            await exchangeTask; // asynchronously wait for the task
            Debug.Log("Socket closed successfully");  // Alert when socket is closed
            client.Dispose();
            exchangeTask = null;
        }
    }

    public void CloseConnection()
    {
        if (client != null)
        {
            Task.Run(() => StopExchange()); // run StopExchange asynchronously
        }
    }

    void OnApplicationQuit()
    {
        CloseConnection(); 
    }
}
