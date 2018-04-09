using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CubeBulletManager))]
public class CubeController : MonoBehaviour {

    public CubeDao MyDaoBao;

    public GameObject BodyObj;


    [Header("同步跟随速度")]
    public float LeapSpeed = 30;
    Vector3 MyTargetPosition;

    Vector3 MyTargetRotation;
    bool isLeapFromServer;
    /// <summary>
    /// 跟随的最小距离
    /// 小于这个距离就不用继续移动了
    /// </summary>
    public float LeapMinDistance = 0.03f;


    [Header("是否是本地玩家")]
    public bool isIamLocalPlayer = false;


    CubeBulletManager cbm;
    // Use this for initialization
    void Start() {
        //Start 比 InitMySelf 还慢

        cbm = GetComponent<CubeBulletManager>();
        

    }

    // Update is called once per frame
    void Update() {
        UpdateLeapTargetFromServer();
       // TestCubeMoveWithKeyboard();
        UpdateTankeController();
    }

    #region TestModel
    /// <summary>
    /// 废弃
    /// </summary>
    void TestCubeMoveWithKeyboard()
    {
        if (!isIamLocalPlayer) return;


        #region Move

        float v = Input.GetAxis("Vertical") * 8;
        float h = Input.GetAxis("Horizontal") * 8;

        if (v != 0 || h != 0)
        {
            Vector3 MytarPos = new Vector3(h, v, 0);
            //     MytarPos += transform.position;
            //     transform.position = Vector3.Lerp(transform.position, MytarPos, Time.deltaTime * LeapSpeed);

            transform.Translate(MytarPos * Time.deltaTime);
        }

        #endregion


        #region Rotate
        bool rr = false;
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 60);
            rr = true;
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.down * Time.deltaTime * 60);
            rr = true;
        }

        #endregion


        if (rr || (v != 0 || h != 0))
        {
            MyDaoBao.SetCubeDao(transform);
            NVSGVideoRecorder.Instance.UpdateObjPositionsToServer(MyDaoBao);
        }

    }

    /// <summary>
    /// 键盘控制移动
    /// </summary>
    void UpdateTankeController()
    {
        if (!isIamLocalPlayer) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 qian = new Vector3(h,0,v);
        Vector3 QianPoint = transform.position + qian;

        float InputMotionValue = Mathf.Max(Mathf.Abs(qian.x),Mathf.Abs(qian.z));

        if (InputMotionValue > 0)
        {
            Quaternion lookRot = Quaternion.LookRotation(qian);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, LeapSpeed * InputMotionValue * Time.deltaTime);
            transform.Translate(Vector3.forward * InputMotionValue * LeapSpeed * Time.deltaTime);

            //赋值好本地信息后同步到网络
            MyDaoBao.SetCubeDao(transform);
            NVSGVideoRecorder.Instance.UpdateObjPositionsToServer(MyDaoBao);

        }
    }


    #endregion

    #region Public Function

    /// <summary>
    /// 这个比Start块，所以用这条方法来初始化
    /// </summary>
    public void InitMySelf()
    {
        InitLocalData();

        int x = Random.Range(-5, 5);
        int y = 0;
        int z = Random.Range(-5, 5);
        Vector3 vv = new Vector3(x, y, z);
        transform.position = vv;

        //设置颜色
        BodyObj.GetComponent<Renderer>().material.color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));

        MyDaoBao = new CubeDao();
        MyDaoBao.SetCubeDao(transform);
        MyDaoBao.setCubeDaoColor(BodyObj.GetComponent<Renderer>().material.color);

        UpdateMyCreateInstanceMessage();

        isIamLocalPlayer = true;
    }

    /// <summary>
    /// 通知Server，我创建了角色
    /// </summary>
    public void UpdateMyCreateInstanceMessage()
    {
        //通知服务端，我这里新建了角色
        NVSGVideoRecorder.Instance.InstanceNewPlayer(MyDaoBao);
    }

    /// <summary>
    /// 根据Server发来的位置信息，完成移动和旋转
    /// </summary>
    /// <param name="cd"></param>
    public void SynchroTransformByMessage(CubeDao cd)
    {
        if (isIamLocalPlayer) return;

        MyDaoBao = MyDaoBao.getANewCubeDao(cd);
        MyTargetPosition = MyDaoBao.getMyPosition();

        MyTargetRotation = MyDaoBao.getMyRotation();
        isLeapFromServer = true;
    }

    /// <summary>
    /// 根据Server 创建子弹，在别的客户端找到我，我来创建子弹
    /// </summary>
    public void SynhroCreateBullet()
    {
        if (isIamLocalPlayer) return;

        cbm.CreateBullet();

    }

    #endregion


    #region TargetMoveModel

    /// <summary>
    /// 同步操作，跟在Server 里面一样
    /// </summary>
    void UpdateLeapTargetFromServer()
    {
        if (!isLeapFromServer) return;

        transform.position = Vector3.MoveTowards(transform.position, MyTargetPosition, Time.deltaTime * LeapSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(MyTargetRotation), Time.deltaTime * LeapSpeed);

        //小于这个距离就不用同步距离了，但是旋转值有可能就停止了
        if (Vector3.Distance(transform.position, MyTargetPosition) < LeapMinDistance)
        {

            isLeapFromServer = false;
        }

    }


    #endregion


    #region LocalFunrion

    /// <summary>
    /// 初始化本地数据
    /// </summary>
    void InitLocalData()
    {
        isLeapFromServer = false;
        MyTargetPosition = Vector3.zero;
        MyTargetRotation = Vector3.zero;
    }

    #endregion


    void OnApplicationQuit()
    {
        NVSGVideoRecorder.Instance.stopWithCubeDao(MyDaoBao);
    }

}
