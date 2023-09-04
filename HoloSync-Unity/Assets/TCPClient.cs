using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class TCPClient : MonoBehaviour
{
    public InputField ip_input;
    public InputField port_input;
    public Text consoleText;
    System.Net.Sockets.TcpClient client;
    System.Net.Sockets.NetworkStream stream;
    private Task exchangeTask;
    private CancellationTokenSource exchangeTokenSource;

    public void ConnectByUI()
    {
        ConnectUWP(ip_input.text, port_input.text);
    }

    public void Connect(string host, string port)
    {
        ConnectUWP(host, port);
    }

    private void ConnectUWP(string host, string port)
    {
        try
        {
            client = new System.Net.Sockets.TcpClient(host, Int32.Parse(port));
            stream = client.GetStream();
            RestartExchange();
            Debug.Log("Connected to server.");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to connect: " + e.ToString());
        }
    }

    private string lastPacket = null;

    public void RestartExchange()
    {
        if (exchangeTask != null)
        {
            StopExchange();
        }

        exchangeTokenSource = new CancellationTokenSource();
        CancellationToken token = exchangeTokenSource.Token;
        exchangeTask = Task.Run(() => ExchangePackets(token));
    }

    public void LateUpdate()
    {
        if (lastPacket != null)
        {
            ShowData(lastPacket);
            Debug.Log(lastPacket);
            lastPacket = null;
        }
    }

    public void ExchangePackets(CancellationToken token)
    {
        byte[] bytes = new byte[client.ReceiveBufferSize];

        while (!token.IsCancellationRequested)
        {
            try
            {
                int recv = stream.Read(bytes, 0, client.ReceiveBufferSize);
                if (recv > 0)
                {
                    string received = Encoding.UTF8.GetString(bytes, 0, recv);
                    lastPacket = received;
                }
            }
            catch (IOException)
            {
                // Socket has been closed
                break;
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to read data: " + e.ToString());
                break;
            }
        }

        Debug.Log("Socket closed.");
    }

    private void ShowData(string data)
    {
        if (data == null)
        {
            consoleText.text = "Received a frame but data was null";
        }
        else
        {
            consoleText.text = data;
        }
    }

    public async void StopExchange()
    {
        if (exchangeTask != null && exchangeTask.Status == TaskStatus.Running)
        {
            exchangeTokenSource.Cancel();
            await exchangeTask;
            exchangeTokenSource.Dispose();
            exchangeTask = null;
        }

        if (client != null)
        {
            client.Close();
            client = null;
        }
    }
    public void OnDestroy()
    {
        StopExchange();
    }

    public void CloseConnection()
    {
        StopExchange();
        Debug.Log("Manually closed the connection.");
    }
}
