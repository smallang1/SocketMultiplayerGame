using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpCharacterListRequest : BaseRequest
{
    MainPack pack = null;
    public GamePanel gamePanel;
    public override void Awake()
    {
        requestCode = RequestCode.Game;
        actionCode = ActionCode.UpCharacterList;
        base.Awake();
    }
    private void Update()
    {
        if (pack != null)
        {
            try
            {
                if (pack.Returncode == ReturnCode.Fail)
                {
                    // 先更新血量（包括死者清零）
                    gamePanel.UpdateList(pack);
                    // 游戏结束，Str 是赢家名字
                    gamePanel.GameOver(pack.Str == GameFace.Face.UserName);
                }
                else
                {
                    gamePanel.UpdateList(pack);
                    if (pack.Returncode == ReturnCode.ReturnNone && !string.IsNullOrEmpty(pack.Str))
                    {
                        // 玩家退出游戏，移除角色
                        face.RemovePlayer(pack.Str);
                    }
                }
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
    }
}
