using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketGameProtocol;
using Unity.VisualScripting;

public class GameFace : MonoBehaviour
{
    private ClientManager clientManager;
    private RequestManager requestManager;
    private UIManager uiManager; //UI管理器
    private PlayerManager playerManager;

    public string UserName
    {
        get;
        set;
    }

    private static GameFace face;
    public static GameFace Face
    {
        get
        {   
            if (face == null)
            {
                face = GameObject.Find("GameFace").GetComponent<GameFace>(); //获取GameFace组件
            }
            return face;  //返回GameFace实例
        }
    }

    void Awake()
    {
        uiManager = new UIManager(this);
        clientManager = new ClientManager(this);
        requestManager = new RequestManager(this);
        playerManager = new PlayerManager(this);

        uiManager.OnInit();
        clientManager.OnInit();   
        requestManager.OnInit();
        playerManager.OnInit();
    }
    private void OnDestroy()
    {
        clientManager.OnDestroy();
        requestManager.OnDestroy(); 
        uiManager.OnDestroy();
    }
    public void Send(MainPack pack)
    {
        clientManager.Send(pack); //发送消息包
    }
    public  void HandleResponse(MainPack pack)
    {
        requestManager.HandleResponse(pack); //处理响应消息包
    }
    public void AddRequest(BaseRequest request)
    {
        requestManager.AddRequest(request);
    }
    public void RemoveRequest(ActionCode actionCode)
    {
        requestManager.RemoveRequest(actionCode); //移除请求
    }

    public void ShowMessage(string str,bool sync = false)
    {
        uiManager.ShowMessage(str,sync); //显示消息
    }
    public void SetSelfID(string id)
    {
        playerManager.CurPlayerID = id;
    }
    public void AddPlayer(MainPack packs)
    {
        playerManager.AddPlayer(packs);
    }
    public void RemovePlayer(string id)
    {
        playerManager.RemovePlayer(id);
    }
    public void GameExit()
    {
        playerManager.GameExit();
        uiManager.PopPanel();
        uiManager.PopPanel();
    }
    public void UpPos(MainPack pack)
    {
        playerManager.UpPos(pack);
    }
    public void SpawnBullet(MainPack pack)
    {
        playerManager.SpawnBullet(pack);
    }
}
