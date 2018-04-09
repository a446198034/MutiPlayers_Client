using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class CubeDao  {

    public string CubeId;

    public float Pos_X;

    public float Pos_Y;

    public float Pos_Z;

    public float Qua_X;

    public float Qua_Y;

    public float Qua_Z;

    public float Color_R;

    public float Color_G;

    public float Color_B;

    public float Color_A; 

    public CubeDao()
    {
        CubeId = Guid.NewGuid().ToString();
        Pos_X = 0;
        Pos_Y = 0;
        Pos_Z = 0;
        Qua_X = 0;
        Qua_Y = 0;
        Qua_Z = 0;
        Color_R = 0;
        Color_G = 0;
        Color_B = 0;
        Color_A = 0;

    }

    public CubeDao(string idd, string posX, string posY, string posZ, string QuaX, string QuaY, string QuaZ,string ColorR,string ColorG,string ColorB,string ColorA)
    {
        CubeId = idd;
        Pos_X = float.Parse(posX);
        Pos_Y = float.Parse(posY);
        Pos_Z = float.Parse(posZ);
        Qua_X = float.Parse(QuaX);
        Qua_Y = float.Parse(QuaY);
        Qua_Z = float.Parse(QuaZ);
        Color_R = float.Parse(ColorR);
        Color_G = float.Parse(ColorG);
        Color_B = float.Parse(ColorB);
        Color_A = float.Parse(ColorA);
    }

    public CubeDao(string[] strArray)
    {
        CubeId = strArray[1];
        Pos_X = float.Parse(strArray[2]);
        Pos_Y = float.Parse(strArray[3]);
        Pos_Z = float.Parse(strArray[4]);
        Qua_X = float.Parse(strArray[5]);
        Qua_Y = float.Parse(strArray[6]);
        Qua_Z = float.Parse(strArray[7]);
        Color_R = float.Parse(strArray[8]);
        Color_G = float.Parse(strArray[9]);
        Color_B = float.Parse(strArray[10]);
        Color_A = float.Parse(strArray[11]);
            
    }

    /// <summary>
    /// 返回一个新的实例
    /// </summary>
    /// <param name="cd"></param>
    /// <returns></returns>
    public CubeDao getANewCubeDao(CubeDao cd)
    {
        CubeDao res = new CubeDao();
        res.CubeId = cd.CubeId;
        res.Pos_X = cd.Pos_X;
        res.Pos_Y = cd.Pos_Y;
        res.Pos_Z = cd.Pos_Z; 
        res.Qua_X = cd.Qua_X;
        res.Qua_Y = cd.Qua_Y;
        res.Qua_Z = cd.Qua_Z;
        res.Color_R = cd.Color_R;
        res.Color_G = cd.Color_G;
        res.Color_B = cd.Color_B;
        res.Color_A = cd.Color_A;
        return res;
    }

    /// <summary>
    /// 获取我的位置
    /// </summary>
    /// <returns></returns>
    public Vector3 getMyPosition()
    {
        return new Vector3(Pos_X,Pos_Y,Pos_Z);
    }

    /// <summary>
    /// 获取的旋转值
    /// </summary>
    /// <returns></returns>
    public Vector3 getMyRotation()
    {
        return new Vector3(Qua_X,Qua_Y,Qua_Z);
    }

    /// <summary>
    /// 获取我身体的颜色
    /// </summary>
    /// <returns></returns>
    public Color getMyColor()
    {
        return new Color(Color_R,Color_G,Color_B,Color_A);
    }

    /// <summary>
    /// 外部请求
    /// 返回详细信息的字符串
    /// 这里要做下判断，像颜色的就不需要一直发送
    /// </summary>
    /// <param name="SpliteTag"></param>
    /// <returns></returns>
    public string getCubeDaoMessageBySpliteTag(string SpliteTag)
    {
        string res = CubeId + SpliteTag + Pos_X + SpliteTag + Pos_Y + SpliteTag + Pos_Z 
                    + SpliteTag + Qua_X + SpliteTag + Qua_Y + SpliteTag + Qua_Z + SpliteTag 
                    + Color_R + SpliteTag + Color_G + SpliteTag + Color_B + SpliteTag + Color_A;
        return res;
    }

    /// <summary>
    /// 设置当前的详细信息
    /// </summary>
    /// <param name="tran"></param>
    public void SetCubeDao(Transform tran)
    {
        Vector3 v = tran.position;
        Pos_X = v.x;
        Pos_Y = v.y;
        Pos_Z = v.z;

        Vector3 r = tran.rotation.eulerAngles;
        Qua_X = r.x;
        Qua_Y = r.y;
        Qua_Z = r.z;
       
    }

    /// <summary>
    /// 设置颜色信息
    /// </summary>
    /// <param name="c"></param>
    public void setCubeDaoColor(Color c)
    {
        Color_R = c.r;
        Color_G = c.g;
        Color_B = c.b;
        Color_A = c.a;
    }

}
