using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketGameProtocol;

public class FireRequest : BaseRequest
{
    private MainPack pack = null;
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.Fire;
        base.Awake();
    }
    public void SendRequest(Vector2 pos,float rot,Vector2 mousepos)
    {
        MainPack pack = new MainPack();
        BulletPack bulletPack = new BulletPack();
        bulletPack.PosX = pos.x;
        bulletPack.PosY = pos.y;
        bulletPack.RotZ = rot;
        bulletPack.MousePosX = mousepos.x;
        bulletPack.MousePosY = mousepos.y;
        pack.BulletPack = bulletPack;
        pack.Requestcode = requestCode;
        pack.Actioncode = actionCode;
        base.SendRequest(pack);
    }
    private void Update()
    {
        if (pack != null)
        {
            face.SpawnBullet(pack);
            pack = null;
        }
    }
    public override void OnResponse(MainPack pack)
    {
        this.pack = pack;
    }
}
