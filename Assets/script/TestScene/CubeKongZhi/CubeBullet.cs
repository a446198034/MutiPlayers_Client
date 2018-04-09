using UnityEngine;
using System.Collections;

public class CubeBullet : MonoBehaviour {
    public int MyPower = 10;
    public float thrust = 0.06f;

    Vector3 MyTargetPos;
    bool isMOve = false;

	// Use this for initialization
	void Start () {
        Destroy(gameObject,1);
    }

    void Update()
    {
        if (isMOve)
        {
            transform.position = Vector3.Lerp(transform.position,MyTargetPos,Time.deltaTime * thrust);
        }
    }

    public void setBUlletMoveTarget(Vector3 v)
    {
        MyTargetPos = v;
        isMOve = true;

    }
}
