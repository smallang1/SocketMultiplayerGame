using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameExitRequest : BaseRequest
{
    private MainPack pack = null;
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.ExitGame;
        base.Awake();
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
        if (pack != null)
        {
            // 如果 GamePanel 正在执行游戏结束倒计时，跳过（由 GameOverCoroutine 处理退出）
            var gp = GetComponent<GamePanel>();
            if (gp != null && gp.IsGameOver)
            {
                pack = null;
                return;
            }
            face.GameExit();
            pack = null;
        }
    }
    public override void OnResponse(MainPack pack)
    {
        this.pack = pack;
    }
}
