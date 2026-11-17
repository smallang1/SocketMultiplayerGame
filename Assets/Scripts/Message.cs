using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SocketGameProtocol;
using Google.Protobuf;

public class Message
{
    private byte[] buffer = new byte[1024];
    private int startindex;

    /// <summary>
    /// 获取消息缓冲区
    /// </summary>
    public byte[] Buffer
    {
        get
        {
            return buffer;
        }
    }
    /// <summary>
    /// 设置消息头
    /// </summary>
    public int Startindex
    {
        get
        {
            return startindex;
        }
    }

    /// <summary>
    /// 获取剩余消息长度
    /// </summary>
    public int Remsize
    {
        get
        {
            return buffer.Length - startindex;
        }
    }

    public void ReadBuffer(int len, Action<MainPack> HandleResponse)
    {
        startindex += len;
        while (true)
        {
            if (startindex <= 4) return;
            //BitConverter.ToInt32 将字节数组转换为Int32类型
            int count = BitConverter.ToInt32(buffer, 0);
            if (startindex >= (count + 4))
            {
                //解析消息包
                //MainPack.Descriptor.Parser.ParseFrom 方法用于解析消息包
                //Descriptor 是MainPack类的静态属性，用于获取消息包的描述符
                //Parser 是MainPack类的静态属性，用于获取消息包的解析器
                //ParseFrom 方法用于解析消息包
                MainPack pack = (MainPack)MainPack.Descriptor.Parser.ParseFrom(buffer, 4, count);
                HandleResponse(pack); //处理消息包

                //处理消息包 把后面的数据向前移动
                Array.Copy(buffer, count + 4, buffer, 0, startindex - (count + 4));
                startindex -= (count + 4); //更新startindex
            }
            else
            {
                break;
            }
        }
    }
    /// <summary>
    /// 打包数据
    /// </summary>
    /// <returns></returns>
    public static byte[] PackData(MainPack pack)
    {
        byte[] data = pack.ToByteArray(); //包体
        byte[] head = BitConverter.GetBytes(data.Length); //包头
        return head.Concat(data).ToArray(); //返回打包后的数据
    }
}
