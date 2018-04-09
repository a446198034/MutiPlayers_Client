using UnityEngine;
using System.Collections;

public class CubeBulletManager : MonoBehaviour {

    [Header("子弹预制体")]
    public GameObject BulletPreObj;

    [Header("子弹发射的地方")]
    public Transform bulletShotPos;

    CubeController cc;

	// Use this for initialization
	void Start () {
        cc = GetComponent<CubeController>();
	}
	
	// Update is called once per frame
	void Update () {

        if (!cc.isIamLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateBullet();
            NVSGVideoRecorder.Instance.CreateBulletToServer(cc.MyDaoBao.CubeId);

        }

	}


    public void CreateBullet()
    {
        GameObject go = Instantiate(BulletPreObj, bulletShotPos.position, bulletShotPos.rotation) as GameObject;
        // go.transform.SetParent(bulletShotPos);
        Vector3 v = bulletShotPos.position + bulletShotPos.forward * 1000;
        go.GetComponent<CubeBullet>().setBUlletMoveTarget(v);
    }

}
