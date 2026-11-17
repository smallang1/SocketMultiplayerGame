using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LogonPanel : BasePanel
{
    public LogonRequest logonRequest;
    public InputField user;
    public InputField pass;
    public Button logonBtn;
    public Button switchBtn;


    public void Start()
    {
        if (logonRequest == null)
            logonRequest = GetComponent<LogonRequest>();
        logonBtn.onClick.AddListener(OnLogonClick); //绑定按钮点击事件
        switchBtn.onClick.AddListener(switchLogin);
    }
    public void OnLogonClick()
    {
        if(user.text == "" && pass.text == "")
        {
            Debug.LogWarning("用户名或密码不能为空");
            return;
        }
        logonRequest.SendRequest(user.text,pass.text);
    }
    private void switchLogin()
    {
        uiMag.PopPanel();
    }
    public override void OnEnter()
    {
        base.OnEnter();
        Enter();
        
    }
    public override void OnPause()
    {
        base.OnPause();
        Exit();
    }
    public override void OnRecovery()
    {
        base.OnRecovery();
        Enter();
    }
    public override void OnExit()
    {
        base.OnExit();
        Exit();
    }
    private void Enter()
    {
        gameObject.SetActive(true);
    }
    private void Exit()
    {
        gameObject.SetActive(false);
    }
    public void OnResponse(MainPack pack)
    {
        switch (pack.Returncode)
        {
            case ReturnCode.Succeed:
                uiMag.ShowMessage("注册成功");
                user.text = "";
                pass.text = "";
                uiMag.PushPanel(PanelType.Login);
                break;
            case ReturnCode.Fail:
                uiMag.ShowMessage("注册失败");
                break;
            default:
                Debug.LogWarning("未知返回码");
                break;
        }
    }
}
