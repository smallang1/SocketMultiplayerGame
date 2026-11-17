using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketGameProtocol;

public class RoomExitRequest : BaseRequest
{
    private bool isExit = false;
    public RoomPanel roomPanel;
    public override void Awake()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.Exit;
        base.Awake();
        if (roomPanel == null)
            roomPanel = GetComponent<RoomPanel>();
    }
    private void OnEnable()
    {
        isExit = false;
    }
    public void SendRequest()
    {
        MainPack pack = new MainPack();
        pack.Requestcode = requestCode;
        pack.Actioncode = actionCode;
        pack.Str = "r";
        base.SendRequest(pack);
    }

    private void Update()
    {
        if (isExit)
        {
            roomPanel.ExitRooomResponse();
            isExit = false;
        }
    }
    public override void OnResponse(MainPack pack)
    {
        if (pack.Returncode == ReturnCode.Succeed)
        {
            // 自己退出房间的响应，OnBackClick 已经 PopPanel 了，忽略
            return;
        }
        // 房主退出，服务器广播通知其他客户端退出
        isExit = true;
        Debug.Log("离开房间");
    }
}
