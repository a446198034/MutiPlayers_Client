using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TCPHelper : MonoBehaviour {

    public string ServerIPAddress = "127.0.0.1";
    public int ServerPort = 4002;

    public InputField ServerIpAddress;

	// Use this for initialization
	void Start () {
        NVSGVideoRecorder.IpServerAddress = ServerIpAddress.text;
        NVSGVideoRecorder.ServerPort = ServerPort;
        NVSGVideoRecorder.Instance.ReConnectWithServer(); 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnApplicationQuit()
    {
        NVSGVideoRecorder.Instance.stop();
    }

}
