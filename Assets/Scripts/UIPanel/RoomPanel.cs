using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketGameProtocol;
using Unity.VisualScripting;

public class RoomPanel : BasePanel
{
    public Button back, send, start;
    public InputField inputtext;
    public Scrollbar Scrollbar;

    public Text chattext;

    public Transform content;
    public GameObject UserItemObj;

    public ChatRequest chatRequest;
    public StartGameRequest startGameRequest;

    private void Start()
    {
        back.onClick.AddListener(OnBackClick);
        send.onClick.AddListener(OnSendClick);
        start.onClick.AddListener(OnStartClick);
    }
    public RoomExitRequest roomExitRequest;
    private void OnBackClick()
    {
        roomExitRequest.SendRequest();
        uiMag.PopPanel();
    }
    private  void OnSendClick()
    {
        if(inputtext.text == "")
        {
            uiMag.ShowMessage("发送内容不能为空");
            return;
        }
        chatRequest.SendRequest(inputtext.text);
        chattext.text += "我：" + inputtext.text+"\n";
        inputtext.text = "";
    }
    private void OnStartClick()
    {
        startGameRequest.SendRequest();
    }
    /// <summary>
    /// 刷新玩家列表
    /// </summary>
    /// <param name="pack"></param>
    public void UpdatePlayerList(MainPack pack)
    {
        //对子物体进行清空
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
        foreach (PlayerPack player in pack.Playerpack)
        {
            UserItem useritem = Instantiate(UserItemObj, Vector3.zero, Quaternion.identity).GetComponent<UserItem>();
            useritem.gameObject.transform.SetParent(content);
            useritem.SetPlayerInFo(player.Playername);
        }
    }
    public void ExitRooomResponse()
    {
        // 房主退出时服务器广播通知其他客户端退出，这里只需 PopPanel
        uiMag.PopPanel();
    }
    public void ChatResponse(string str)
    {
        chattext.text += str + "\n";
    }
    public void StartGameResponse(MainPack pack)
    {
        switch (pack.Returncode)
        {
            case ReturnCode.Fail:
                uiMag.ShowMessage("开始游戏失败！您不是房主");
                break;
            case ReturnCode.Succeed:
                uiMag.ShowMessage("游戏已启动！");
                break;

        }
    }
    public void GameStarting(MainPack packs)
    {
        GamePanel gamePanel = uiMag.PushPanel(PanelType.Game).GetComponent<GamePanel>();
        gamePanel.UpdateList(packs);
    }
    public override void OnEnter()
    {
        base.OnEnter();
        gameObject.SetActive(true);
        if (inputtext != null) inputtext.text = "";
        if (chattext != null) chattext.text = "";
    }
    public override void OnPause()
    {
        base.OnPause();
        gameObject.SetActive(false);
    }
    public override void OnRecovery()
    {
        base.OnRecovery();
        gameObject.SetActive(true);
        if (inputtext != null) inputtext.text = "";
        if (chattext != null) chattext.text = "";
    }
    public override void OnExit()
    {
        base.OnExit();
        gameObject.SetActive(false);
    }
}
