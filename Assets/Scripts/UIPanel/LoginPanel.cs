using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    public LoginRequest loginRequest;
    public InputField user;
    public InputField pass;
    public Button loginBtn;
    public Button switchBtn;

    public void Start()
    {
        if (loginRequest == null)
            loginRequest = GetComponent<LoginRequest>();
        loginBtn.onClick.AddListener(OnLoginClick);
        switchBtn.onClick.AddListener(SwitchLogon);
    }
    public void OnLoginClick()
    {
        if (user.text == "" && pass.text == "")
        {
            Debug.LogWarning("用户名或密码不能为空");
            return;
        }
        loginRequest.SendRequest(user.text, pass.text);
    }
    private void SwitchLogon()
    {
        uiMag.PushPanel(PanelType.Logon);
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
                uiMag.ShowMessage("登录成功");
                face.UserName = user.text;
                user.text = "";
                pass.text = "";
                uiMag.PushPanel(PanelType.RoomList);
                break;
            case ReturnCode.Fail:
                uiMag.ShowMessage("登录失败");
                break;
            default:
                Debug.LogWarning("未知返回码");
                break;
        }
    }
}
