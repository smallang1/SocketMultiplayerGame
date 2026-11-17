using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketGameProtocol;

public class BaseRequest : MonoBehaviour
{
    // 请求码
    protected RequestCode requestCode;
    protected ActionCode actionCode;
    protected GameFace face;

    public ActionCode GetActionCode
    {
        get
        {
            return actionCode;
        }
    }

    public virtual void Awake()
    {
        face = GameFace.Face; // 获取GameFace实例
        
    }
    public virtual void Start()
    {
        face.AddRequest(this); // 添加请求
        Debug.Log("添加：" + actionCode.ToString());
    }
    public virtual void OnDestroy()
    {
        face.RemoveRequest(actionCode); // 移除请求
    }
    /// <summary>
    /// 处理响应消息包
    /// </summary>
    public virtual void OnResponse(MainPack pack)
    {

    }
    /// <summary>
    /// 发送请求消息包
    /// </summary>

    public virtual void SendRequest(MainPack pack)
    {
        face.Send(pack); // 发送消息包
    }
}
