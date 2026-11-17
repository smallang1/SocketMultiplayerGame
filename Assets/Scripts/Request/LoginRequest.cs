using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginRequest : BaseRequest
{
    public LoginPanel loginPanel;

    private MainPack pack = null;
    public override void Awake()
    {
        requestCode = RequestCode.User;
        actionCode = ActionCode.Login;
        base.Awake();
        if (loginPanel == null)
            loginPanel = GetComponent<LoginPanel>();
    }
    private void Update()
    {
        if(pack != null)
        {
            try
            {
                loginPanel.OnResponse(pack);
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
    public void SendRequest(string user, string pass)
    {
        MainPack pack = new MainPack();
        pack.Requestcode = requestCode;
        pack.Actioncode = actionCode;
        LoginPack loginPack = new LoginPack();
        loginPack.Username = user;
        loginPack.Password = pass;
        pack.Loginpack = loginPack;
        base.SendRequest(pack);
    }
}
