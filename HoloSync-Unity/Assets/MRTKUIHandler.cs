using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.Events;
public class MRTKUIHandler : MonoBehaviour
{
    public MRTKTMPInputField ip_input;
    public MRTKTMPInputField port_input;
    public TextMeshProUGUI consoleText;
    public PressableButtonHoloLens2 connectButton;
    public PressableButtonHoloLens2 closeButton;
    private string lastPacket = null;
    public UnityEvent onConnect;
    public UnityEvent onDisconnect;

    public void Start()
    {
        connectButton.ButtonPressed.AddListener(() => onConnect.Invoke());
        closeButton.ButtonPressed.AddListener(() => onDisconnect.Invoke());

    }

    public void Update()
    {
        if (lastPacket != null)
        {
            //do something
            ShowData(lastPacket);
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

    public void UpdateDataPacket(string newData)
    {
        lastPacket = newData;
    }
}
