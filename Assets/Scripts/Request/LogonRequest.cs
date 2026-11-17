using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketGameProtocol;

public class LogonRequest : BaseRequest
{
    public LogonPanel logonPanel;

    private MainPack pack = null;
    public override void Awake()
    {
        requestCode = RequestCode.User;
        actionCode = ActionCode.Logon;
        base.Awake(); //调用基类的Awake方法
        if (logonPanel == null)
            logonPanel = GetComponent<LogonPanel>();
    }
    private void Update()
    {
        if(pack != null)
        {
            try
            {
                logonPanel.OnResponse(pack);
            }
            finally
            {
                pack = null;
            }
        }
    }
    override public void OnResponse(MainPack pack)
    {
        this.pack = pack;
    }
    public void SendRequest(string user,string pass)
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
