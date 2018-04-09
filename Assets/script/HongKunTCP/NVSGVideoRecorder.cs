using UnityEngine;
using System.Collections;
using Assets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

public  class NVSGVideoRecorder  {

    public static string IpServerAddress = "127.0.0.1";
    public static int ServerPort = 4002;
    bool isHeadbeatChecking = false;

    string Splitetag = "^";
    bool isVLCInstanceExit = false;


    #region Instance

    static NVSGVideoRecorder instance;
    public static NVSGVideoRecorder Instance
    {
        get 
        {
            if (instance == null)
                instance = new NVSGVideoRecorder(); 

            return instance;
        }
        set { }
    }


    #endregion


    #region TcpEventBing

    TcpClient client;
    NVSGVideoRecorder()
    {
        client = new TcpClient(IpServerAddress, ServerPort);
        client.DataReceived += client_DataReceived;
    }
    public void client_OnConnected(object sender, EventArgs e)
    {
        // Debug.Log("Connect Success with IP " + IPServerAddress + " Port " + ServerPort);
    }
    public void client_OnClosed(object sender, EventArgs e)
    {
        Debug.Log("Socket is closed ");
    }
    void client_XinTiao(object sender, EventArgs e)
    {
        if (!isHeadbeatChecking)
        {
            isHeadbeatChecking = true;
            Client_Headbest_Normal(this, null);
        }
        else
        {
            isHeadbeatChecking = false;
            Debug.Log("Connecting is Duanle");
            client.Stop(); 
            Client_Headbest_Stop(this, null);
        }
    }

    public void Client_Headbest_Normal(object sender, EventArgs e)
    {

    }

    public void Client_Headbest_Stop(object sender, EventArgs e)
    {

    }

    public void client_DataReceived(object sender, DataReceivedEventArgs e)
    {
        isHeadbeatChecking = false;
        string content = System.Text.Encoding.UTF8.GetString(e.Data, 0, e.RealDataSize);

        List<string> ResultList = AnalyzeDateFromClient(content);

        foreach (string str in ResultList)
        {
            MessageDealCenter(str);
        }


    }


    #endregion


    #region Public function

    /// <summary>
    /// 通知服务端，这个客户端创建了用户
    /// </summary>
    /// <param name="cd"></param>
    public void InstanceNewPlayer(CubeDao cd)
    {
        if (client != null)
        {
            string str = "CreatePlayer" + Splitetag + cd.getCubeDaoMessageBySpliteTag(Splitetag);
            client.Send(str);
        }
    }

    /// <summary>
    /// 将本地所有的数据发送到服务端
    /// </summary>
    /// <param name="cdList"></param>
    public void UpdateAllClients(List<CubeDao> cdList)
    {
        if (client != null)
        {
            string str = "UpdatePersons" + Splitetag;
            
            for ( int i = 0; i < cdList.Count; i ++)
            {
                CubeDao cd = cdList[i];
                str +=  "UpLoadObj" + Splitetag + cd.getCubeDaoMessageBySpliteTag(Splitetag);
                str += i < cdList.Count - 1 ? Splitetag : "";
            }
            client.Send(str);
        }
    }

    /// <summary>
    /// 通知服务端，我的位置发生改变，需要将其他客户端也更新我的位置
    /// </summary>
    /// <param name="cd"></param>
    public void UpdateObjPositionsToServer(CubeDao cd)
    {
        if (client != null)
        {
            string str = "UpdateObjPositions" + Splitetag + cd.getCubeDaoMessageBySpliteTag(Splitetag);
            client.Send(str);
        }
    }

    /// <summary>
    /// 通知服务端，我这个ID创建了子弹
    /// </summary>
    /// <param name="Id"></param>
    public void CreateBulletToServer(string Id)
    {
        if (client != null)
        {
            string str = "CreateBullet" + Splitetag + Id;
            client.Send(str);
        }
    }

    /// <summary>
    /// CubeDao 退出客户端
    /// </summary>
    /// <param name="cd"></param>
    public void stopWithCubeDao(CubeDao cd)
    {
        if (client != null)
        {
            string str = "stopPlayer" + Splitetag + cd.getCubeDaoMessageBySpliteTag(Splitetag);
            client.Send(str);
            stop();
        }
    }

    public void stop()
    {
        if (client != null)
        {
            client.Stop();
        }
    }



    public void ReConnectWithServer()
    { 
        if (client != null)
        {
            client.Start();
        }
    }

    #endregion


    #region LocalFunction

    #endregion


    #region 消息处理中心


    /// <summary>
    /// 分析客户端发来的数据里面有几条
    /// 并根据 <EOF> 作为判断依据，返回结果集
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    List<string> AnalyzeDateFromClient(string str)
    {
        List<string> resList = new List<string>();

        int EOFIndex = str.IndexOf("<EOF>");

        if (EOFIndex <= -1)
        {
            //-1 说明没有EOF结尾
            return resList;
        }

        //int EOFCount = str.ToCharArray().Count(x => x == '<EOF>');
        int EOFCount = Regex.Matches(str, @"<EOF>").Count;

        for (int i = 0; i < EOFCount; i++)
        {
            int index_EOF = str.IndexOf("<EOF>");
            string FirstConnect = str.Substring(0, index_EOF);
            resList.Add(FirstConnect);

            str = str.Substring(index_EOF + 5, str.Length - index_EOF - 5);
        }


        return resList;
    }


    /// <summary>
    /// 消息处理中心
    /// 只处理单条的数据
    /// </summary>
    /// <param name="content"></param>
    void MessageDealCenter(string content)
    {
              
        string message = content.Replace("<EOF>", "");
        string[] splits = message.Split(Splitetag.ToCharArray());
        if (splits[0] == "CreatePlayer")
        {
            DealWith_CreatePlayer(splits);
        }
        else if (splits[0] == "UpdatePersons")
        {
            DealWith_UpdatePersons(splits);
        }
        else if (splits[0] == "UpdateObjPositions")
        {
            DealWith_UpdateObjPositions(splits);
        }
        else if (splits[0] == "CreateBullet")
        {
            DealWith_CreateBullet(splits);
        }
        else if (splits[0] == "stopPlayer")
        {
            DealWith_stopPlayer(splits);
        }
        else if (splits[0] == "Command")
        {
            if (splits[1] == "stop")
            {

            }
            else if (splits[1] == "UpdateAllClients")
            {
                CubeManager.UpdateAllClientsList.Add("uu");
            }
            else if (splits[1] == "PauseSteaming")
            {

            }
            else if (splits[1] == "GameExit")
            {
                //程序退出时候调用的释放

            }
        }

    }

    /// <summary>
    /// CreatePlayer 消息处理
    /// </summary>
    /// <param name="splits"></param>
    void DealWith_CreatePlayer(string[] splits)
    {
        CubeDao cd = new CubeDao(splits);
        lock (CubeManager.CreatePlayerMessageList)
        {
            CubeManager.CreatePlayerMessageList.Add(cd);
        }

    }

    /// <summary>
    /// UpdatePersons 消息处理
    /// UpdatePersons^UpLoadObj^Id^x^y^z^Qx^Qy^Qz
    /// </summary>
    /// <param name="splits"></param>
    void DealWith_UpdatePersons(string[] splits)
    {
        for (int i = 1; i < splits.Length; i++)
        {
            if (splits[i] == "UpLoadObj")
            {
                string idd = splits[i + 1];
                string posx = splits[i + 2];
                string posy = splits[i + 3];
                string posz = splits[i + 4];
                string quax = splits[i + 5];
                string quay = splits[i + 6];
                string quaz = splits[i + 7];
                string colorR = splits[i + 8];
                string colorG = splits[i + 9];
                string colorB = splits[i + 10];
                string colorA = splits[i + 11];
                
                CubeDao cd = new CubeDao(idd,posx,posy,posz,quax,quay,quaz,colorR,colorG,colorB,colorA);

                lock (CubeManager.CreatePlayerMessageList)
                {
                    CubeManager.CreatePlayerMessageList.Add(cd);
                }
            }
        }
    }

    /// <summary>
    ///  UpdateObjPositions 消息处理
    /// </summary>
    /// <param name="splits"></param>
    void DealWith_UpdateObjPositions(string[] splits)
    {
        CubeDao cd = new CubeDao(splits);
        lock (CubeManager.UpdateObjPositionsMessageList)
        {
            CubeManager.UpdateObjPositionsMessageList.Add(cd);
        }
    }

    /// <summary>
    /// CreateBullet  创建子弹
    /// </summary>
    /// <param name="splits"></param>
    void DealWith_CreateBullet(string[] splits)
    {
        lock (CubeManager.UpdateCreateBullet)
        {
            CubeManager.UpdateCreateBullet.Add(splits[1]);
        }
    }

    /// <summary>
    /// stopPlayer 消息处理
    /// </summary>
    /// <param name="splits"></param>
    void DealWith_stopPlayer(string[] splits)
    {
        CubeDao cd = new CubeDao(splits);
        lock (CubeManager.stopPlayerClientsList)
        {
            CubeManager.stopPlayerClientsList.Add(cd);
        }
    }

    #endregion

}
