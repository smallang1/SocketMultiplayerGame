using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Net.Mail;
using SocketGameProtocol;

public class ClientManager : BaseManager
{
    private Socket socket;
    private Message message;
    public ClientManager(GameFace face) : base(face) { }

    override public void OnInit()
    {
        base.OnInit();
        message = new Message();
        InitSocket();
    }
    override public void OnDestroy()
    {
        base.OnDestroy();
        message = null;
        CloseSocket();
    }
    /// <summary>
    /// 初始化Socket连接
    /// </summary>
    private void InitSocket()
    {
        // 创建Socket实例
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            // 连接到服务器 成功
            socket.Connect("127.0.0.1", 6666);
            StartReceive();
            face.ShowMessage("连接成功");
        }
        catch(Exception e)
        {
            // 连接失败
            Debug.LogWarning(e.Message);
            face.ShowMessage("连接失败");
        }        
    }
    /// <summary>
    ///  关闭Socket连接
    /// </summary>
    private void CloseSocket()
    {
        if (socket.Connected && socket !=null)
        {
            socket.Close();
        }
    }
    private void StartReceive()
    {
        // 开始接收数据
        //参数含义 1.缓冲区 2.开始索引 3.剩余大小 4.标志 5.回调方法 6.回调参数
        socket.BeginReceive(message.Buffer, message.Startindex, message.Remsize, SocketFlags.None, ReceiveCallback, null);
    }

    private void ReceiveCallback(IAsyncResult ias)
    {
        try
        {
            if (socket == null || socket.Connected == false) return;
            int len = socket.EndReceive(ias);
            if (len == 0)
            {
                CloseSocket();
                return;
            }
            message.ReadBuffer(len, HandleResponse);
            StartReceive(); //继续接收数据

        }
        catch (Exception e)
        {
            Debug.LogWarning(e); //处理异常
            face.ShowMessage("连接失败", true);
        }
    }
    /// <summary>
    /// 处理消息包
    /// </summary>
    private void HandleResponse(MainPack pack)
    {
        face.HandleResponse(pack); //调用GameFace的HandleResponse方法处理消息包
    }

    public void Send(MainPack pack)
    {
        socket.Send(Message.PackData(pack));
    }
}
