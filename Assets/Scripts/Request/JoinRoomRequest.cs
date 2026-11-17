using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinRoomRequest : BaseRequest
{
    public RoomListPanel roomListPanel;
    private MainPack pack = null;
    public override void Awake()
    {
        requestCode = RequestCode.Room;
        actionCode = ActionCode.JoinRoom;
        base.Awake();
        if (roomListPanel == null)
            roomListPanel = GetComponent<RoomListPanel>();
    }
    public void SendRequest(string roomname)
    {
        MainPack pack = new MainPack();
        pack.Requestcode = requestCode;
        pack.Actioncode = actionCode;
        pack.Str = roomname;
        base.SendRequest(pack);
    }
    private void Update()
    {
        if (pack != null)
        {
            try
            {
                roomListPanel.JoinRoomResponse(pack);
            }
            finally
            {
                pack = null;
            }
        }
    }
    public override void OnResponse(MainPack pack)
    {
        this.pack = pack;
        base.OnResponse(pack);
    }
}
