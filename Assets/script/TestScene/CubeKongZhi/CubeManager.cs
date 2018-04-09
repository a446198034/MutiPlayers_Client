using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;


public class CubeManager : MonoBehaviour {

    [Header("Cube 预制体")]
    public GameObject CubePrefabs;
    public Transform CubeParents;
    public List<GameObject> CubeObjList; //无须赋值

    public Text TEstShowUIText;


    #region Public static List

    /// <summary>
    /// 创建物体
    /// </summary>
    public static List<CubeDao> CreatePlayerMessageList;

    /// <summary>
    /// 当有新客户端加入的时候Server呼叫第一个客户端提交他所有玩家的数据
    /// Server端再将数据分配到所有客户端中ing
    /// 让所有客户端自己筛选，差哪个加哪个
    /// </summary>
    public static List<string> UpdateAllClientsList;

    /// <summary>
    /// 更新物体实时位置的List
    /// </summary>
    public static List<CubeDao> UpdateObjPositionsMessageList;

    /// <summary>
    /// 更新别人创建的子弹
    /// </summary>
    public static List<string> UpdateCreateBullet;

    /// <summary>
    /// 某个客户端退出服务端
    /// 需要其他客户端删除这个玩家
    /// </summary>
    public static List<CubeDao> stopPlayerClientsList;

    

    #endregion


    // Use this for initialization
    void Start () {
        CreatePlayerMessageList = new List<CubeDao>();
        UpdateAllClientsList = new List<string>();
        UpdateObjPositionsMessageList = new List<CubeDao>();
        UpdateCreateBullet = new List<string>();
        stopPlayerClientsList = new List<CubeDao>();
        CreateACubeInstance();
	}
	
	// Update is called once per frame
	void Update () {
        ListenPlayerMessageList();

        //Test
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    string res = "";
            
        //    foreach (GameObject go in CubeObjList)
        //    {
        //        CubeController cc = go.GetComponent<CubeController>();
        //        res += cc.isIamLocalPlayer + "\r\n" + cc.MyDaoBao.getCubeDaoMessageBySpliteTag("^") + "\r\n";
        //    }
        //    TEstShowUIText.text = res;
        //}
    }

    #region Public Function

    /// <summary>
    /// 初始化默认的对象
    /// </summary>
    public void CreateACubeInstance()
    {
        GameObject go = Instantiate(CubePrefabs) as GameObject;
        go.transform.SetParent(CubeParents);
        go.transform.position = Vector3.zero;
         
        go.GetComponent<CubeController>().InitMySelf();

        CubeObjList.Add(go);

    }

    /// <summary>
    /// 初始化一个有数据的对象
    /// </summary>
    /// <param name="dc"></param>
    public void CreateACubeInstance(CubeDao dc)
    {
        if (IfIDisintheList(dc.CubeId)) return;

        GameObject go = Instantiate(CubePrefabs) as GameObject;
        go.transform.SetParent(CubeParents);


        CubeController cc = go.GetComponent<CubeController>();
        cc.MyDaoBao = cc.MyDaoBao.getANewCubeDao(dc);
       // cc.MyDaoBao.SetCubeDao(go.transform);

        go.transform.position = cc.MyDaoBao.getMyPosition();
        go.transform.rotation = Quaternion.Euler(cc.MyDaoBao.getMyRotation());
        go.GetComponent<CubeController>().BodyObj.GetComponent<Renderer>().material.color = cc.MyDaoBao.getMyColor();

        CubeObjList.Add(go);
    }

    /// <summary>
    /// 同步所有客户端的所有数据上去服务器
    /// </summary>
    public void UpLoadAllClientsDetail()
    {
        List<CubeDao> cdList = new List<CubeDao>();
        for (int i = 0; i < CubeObjList.Count; i++)
        {
            CubeController cc = CubeObjList[i].GetComponent<CubeController>();
            cc.MyDaoBao.SetCubeDao(CubeObjList[i].transform);
            cdList.Add(cc.MyDaoBao);
        }

        NVSGVideoRecorder.Instance.UpdateAllClients(cdList.ToList());

    }

    /// <summary>
    /// 更新所有客户端中关于某个物体的位置移动信息
    /// </summary>
    /// <param name="CubeList"></param>
    public void UpdateObjPositionsWithAllClients(List<CubeDao> CubeList)
    {
        for (int i = 0; i < CubeList.Count; i++)
        {
            for (int j = 0; j < CubeObjList.Count; j++)
            {
                CubeController cc = CubeObjList[j].GetComponent<CubeController>();

                if (cc.MyDaoBao.CubeId == CubeList[i].CubeId)
                {
                    //找到要同步的这个物体的iD了
                    cc.SynchroTransformByMessage(CubeList[i]);
           
                }
            }
        }
    }

    /// <summary>
    /// 创建子弹
    /// </summary>
    /// <param name="BulletCreateList"></param>
    public void UpdateCreateBulletWithAllClients(List<string> BulletCreateList)
    {
        for (int i = 0; i < BulletCreateList.Count; i++)
        {
            for (int j = 0; j < CubeObjList.Count; j++)
            {
                CubeController cc = CubeObjList[j].GetComponent<CubeController>();

                if (cc.MyDaoBao.CubeId == BulletCreateList[i])
                {
                    cc.SynhroCreateBullet();
                }

            }
        }
    }

    /// <summary>
    /// 某个客户端退出服务，需要其他客户端把他删掉
    /// </summary>
    /// <param name="cubeList"></param>
    public void ClientStopAndRemove(List<CubeDao> cubeList)
    {
        for (int i = 0; i < cubeList.Count; i++)
        {
            for (int j = 0; j < CubeObjList.Count; j++)
            {
                CubeController cc = CubeObjList[j].GetComponent<CubeController>();

                if (cc.MyDaoBao.CubeId == cubeList[i].CubeId)
                {
                    GameObject go = CubeObjList[j];
                     Destroy(go);
                    CubeObjList.Remove(go);
                }
            }
        }
    }

    #endregion


    #region Local Function

    /// <summary>
    /// 将加的这个人跟本地进行比较
    /// </summary>
    /// <param name="str"></param>
    void CreateInstanceByMessage(List<CubeDao> CubeList)
    {
        List<CubeDao> CubeListBackUp = CubeList.ToList();
        for (int i = 0; i < CubeListBackUp.Count; i++)
        {
            for (int j = 0; j < CubeObjList.Count; j++)
            {
                CubeController cc = CubeObjList[j].GetComponent<CubeController>();

                if (cc.MyDaoBao.CubeId == CubeListBackUp[i].CubeId)
                {
                    //ID 相同即为本地的
                    CubeListBackUp.Remove(CubeListBackUp[i]);
                }
            }
        }

        //把本地相同ID的筛选过后，接着进行创建
        foreach (CubeDao cd in CubeListBackUp)
        {
            CreateACubeInstance(cd);
        }

    }

    /// <summary>
    /// 检测新加入的Cubeid 是否已经存在与List中
    /// </summary>
    /// <param name="Pid"></param>
    /// <returns>True 表示这个ID已经存在了</returns>
    bool IfIDisintheList(string Pid)
    {
        bool res = false;
        for (int i = 0; i < CubeObjList.Count; i++)
        {
            CubeController cc = CubeObjList[i].GetComponent<CubeController>();

            if (cc.MyDaoBao.CubeId == Pid)
            {
                res = true;
            }
        }
        return res;
    }
   

    #endregion


    #region 监听消息队列

    /// <summary>
    /// 监听创建PLayer的消息队列
    /// </summary>
    void ListenPlayerMessageList()
    {
        //创建实例
        lock (CreatePlayerMessageList)
        {
            if (CreatePlayerMessageList.Count > 0)
            {
                CreateInstanceByMessage(CreatePlayerMessageList);
                CreatePlayerMessageList.Clear();
            }
        }

        //发送所有客户端实例
        lock (UpdateAllClientsList)
        {
            if (UpdateAllClientsList.Count > 0)
            {
                UpLoadAllClientsDetail();
                UpdateAllClientsList.Clear();
            }
        }

        //更新所有客户端中关于某个物体的位置信息
        lock (UpdateObjPositionsMessageList)
        {
            if (UpdateObjPositionsMessageList.Count > 0)
            {
                UpdateObjPositionsWithAllClients(UpdateObjPositionsMessageList);
                UpdateObjPositionsMessageList.Clear();
            }
        }

        //更新别人发射的子弹
        lock (UpdateCreateBullet)
        {
            if (UpdateCreateBullet.Count > 0)
            {
                UpdateCreateBulletWithAllClients(UpdateCreateBullet); 
                UpdateCreateBullet.Clear();
            }
        }


        //某个客户端退出，其他客户端需要删除
        lock (stopPlayerClientsList)
        {
            if (stopPlayerClientsList.Count > 0)
            {
                ClientStopAndRemove(stopPlayerClientsList);
                stopPlayerClientsList.Clear();
            }
        }



    }


    #endregion

     
}
 