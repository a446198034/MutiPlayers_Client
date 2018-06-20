using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBodyTrigger : MonoBehaviour {

    public GameObject MyFatherObj;
    CubeHealthManager chm;

	// Use this for initialization
	void Start () {
        chm = MyFatherObj.GetComponent<CubeHealthManager>();
	}


    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Bullet"))
        {
            int p = other.GetComponent<CubeBullet>().MyPower;
            chm.getHurt(p);
        }
    }

}
