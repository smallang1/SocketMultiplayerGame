using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpPosRequest : BaseRequest
{

    private MainPack pack = null;
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.UpPos;
        base.Awake();
    }
    /// <summary>
    /// 发送数据
    /// </summary>
    public void SendRequest(Vector2 pos,float characterRot,float GunRot)
    {
        MainPack pack = new MainPack();
        PosPack posPack = new PosPack();
        PlayerPack playerPack = new PlayerPack();
        posPack.PosX = pos.x;
        posPack.PosY = pos.y;
        posPack.RotZ = characterRot;
        posPack.GunPosZ = GunRot;
        playerPack.Playername = face.UserName;
        playerPack.Pospack = posPack;
        pack.Playerpack.Add(playerPack);
        pack.Requestcode = requestCode;
        pack.Actioncode = actionCode;
        base.SendRequest(pack);
    }
    private void Update()
    {
        if (pack != null)
        {
            face.UpPos(pack);
            pack = null;
        }
    }
    public override void OnResponse(MainPack pack)
    {
        this.pack = pack;
    }
}
