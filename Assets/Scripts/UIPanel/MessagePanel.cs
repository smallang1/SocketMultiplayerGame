using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagePanel : BasePanel
{
    public Text text;
    string msg = null;

    public override void OnEnter()
    {
        base.OnEnter();
        text.CrossFadeAlpha(0, 0.1f, false);
        uiMag.SetMessagePanel(this);
    }

    private void Update()
    {
        if(msg != null)
        {
            ShowText(msg);
            msg = null; //清空消息
        }
    }
    public void ShowMessage(string str,bool issync = false)
    {
        if (issync)
        {
            //异步显示
            msg = str;
        }
        else
        {
            ShowText(str); //同步显示
        }      
    }
    private void ShowText(string str)
    {
        CancelInvoke("HideText");
        text.text = str;
        text.CrossFadeAlpha(1, 0.1f, false);
        Invoke("HideText", 1);
    }
    private void HideText()
    {
        text.CrossFadeAlpha(0, 0.1f, false);
    }
}
