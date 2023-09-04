using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using System.IO;
[RequireComponent(typeof(TCPClientHololens))]
public class IPConfig : MonoBehaviour
{
    MRTKTMPInputField ip_input;
    MRTKTMPInputField port_input;

    string ip;
    string port;

    // Start is called before the first frame update
    void Awake()
    {
        ip_input = GetComponent<TCPClientHololens>().ip_input;
        port_input = GetComponent<TCPClientHololens>().port_input;
        Read("config.txt");
        ip_input.text = ip;
        port_input.text = port;
    }

    // Update is called once per frame
    void OnApplicationQuit()
    {
        Write("config.txt");
    }

    public void Read(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(filePath))
        {
            // Read the text file contents
            string fileContents;
            using (StreamReader reader = new StreamReader(filePath))
            {
                fileContents = reader.ReadToEnd();
            }

            // Do something with the file contents
            Debug.Log("File contents: " + fileContents);
            string[] lines = fileContents.Split('\n');
            ip = lines[0].Trim();
            port = lines[1].Trim();
        }

        else
        {
            Debug.LogError("File not found at path: " + filePath);
        }
    }

    public void Write(string fileName)
    {
        string[] content = new string[2] { ip_input.text, port_input.text };
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(filePath))
        {
            // Write the first two lines to the output file
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (string line in content)
                {
                    writer.WriteLine(line);
                }
            }
        }


        else
        {
            Debug.LogError("File not found at path: " + filePath);
        }
    }
}
