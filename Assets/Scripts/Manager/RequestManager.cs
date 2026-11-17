using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketGameProtocol;

public class RequestManager : BaseManager
{
    public RequestManager(GameFace face) : base(face){}

    private Dictionary<ActionCode,BaseRequest> requestDict = new Dictionary<ActionCode, BaseRequest>();

    public void AddRequest(BaseRequest request)
    {
        requestDict.Add(request.GetActionCode, request);
    }
    public void RemoveRequest(ActionCode actionCode)
    {
        requestDict.Remove(actionCode);
    }
    public void HandleResponse(MainPack pack)
    {
        if(requestDict.TryGetValue(pack.Actioncode, out BaseRequest request))
        {
            request.OnResponse(pack); //处理响应消息包
            Debug.Log("处理");
        }
        else
        {
            Debug.LogWarning("未找到对应的请求处理方法");
        }
    }
}
