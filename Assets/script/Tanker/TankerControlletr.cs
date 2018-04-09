using UnityEngine;
using System.Collections;

public class TankerControlletr : MonoBehaviour {

    public CubeDao MyDaoBao;

    [Header("同步的速度")]
    public float LeapSpeed = 20;
    Vector3 MyTargetPosition;

    Vector3 MyTargetRotation;
    bool isLeapFromServer;

    [Header("跟随的最小距离")]
    public float LeapMinDistance = 0.03f;
    
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
