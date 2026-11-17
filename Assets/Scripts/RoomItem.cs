using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public Button join;
    public Text title, num, status;

    public RoomListPanel roomListPanel;

    private void Start()
    {
        join.onClick.AddListener(OnjoinClick);
    }
    private void OnjoinClick()
    {
        roomListPanel.JoinRoom(title.text);
    }
    public void SetRoomInfo(string title,int curnum,int maxnum,int status)
    {
        this.title.text = title;
        this.num.text = curnum + "/" + maxnum;
        switch (status)
        {
            case 0:
                this.status.text = "等待加入";
                break;
            case 1:
                this.status.text = "房间已满人";
                break;
            case 2:
                this.status.text = "游戏中";
                break;
            default:
                break;
        }
    }
}