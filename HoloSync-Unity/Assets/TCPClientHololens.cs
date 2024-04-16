using System;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;

[RequireComponent(typeof(MRTKUIHandler))]
public class TCPClientHololens : MonoBehaviour
{
    System.Net.Sockets.TcpClient client;
    System.Net.Sockets.NetworkStream stream;
    private Task exchangeTask;
    private bool exchangeStopRequested = false;
    private MRTKUIHandler MRTKUI;

    private void Awake()
    {
        MRTKUI = GetComponent<MRTKUIHandler>();
    }
    public async void ConnectByUI()
    {
        await ConnectUWP(MRTKUI.ip_input.text, MRTKUI.port_input.text);
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

    public void RestartExchange()
    {
        if (exchangeTask != null) Task.Run(() => StopExchange());
        exchangeStopRequested = false;
        exchangeTask = Task.Run(() => ExchangePackets());
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
                MRTKUI.UpdateDataPacket(received);
                Debug.Log(received);
            }
        }
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
