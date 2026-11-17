using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using SocketMultiplayerGameServer.Controller; //引入控制器管理类
using SocketGameProtocol;


//服务端
namespace SocketMultiplayerGameServer.Servers
{
    internal class Server
    {
        private Socket socket;

        private List<Client> clientList = new List<Client>(); //客户端列表
        private List<Room> roomList = new List<Room>();

        private ControllerManager controllerManager; //控制器管理对象

        public Server(int port)
        {
            //初始化控制器管理对象
            controllerManager = new ControllerManager(this); 
            //创建一个Socket对象，指定地址族、套接字类型和协议类型
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //绑定到指定的端口上
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Listen(0); //开始监听
            StartAccept(); //开始接受连接请求

        }
        /// <summary>
        /// 开始接受连接请求
        /// </summary>
        void StartAccept()
        {
            //开始接受连接请求，当有连接请求时，调用AcceptCallback方法
            socket.BeginAccept(AcceptCallback,null);
        }
        void AcceptCallback(IAsyncResult iar)
        {
            //结束接受连接请求 
            Socket client = socket.EndAccept(iar);
            //将客户端Socket对象添加到客户端列表中
            clientList.Add(new Client(client,this));
            StartAccept(); //继续接受连接请求
        }
        

        public void HandleRequest(MainPack pack,Client client)
        {
            controllerManager.HandleRequest(pack, client);
        }

        public void RemoveClient(Client client)
        {
            clientList.Remove(client); //从客户端列表中移除客户端
        }

        public MainPack CreateRoom(Client client,MainPack pack)
        {
            try
            {
                Room room = new Room(client, pack.Roompack[0],this);
                roomList.Add(room);
                foreach(PlayerPack p in room.GetPlayerInFo())
                {
                    pack.Playerpack.Add(p);
                }
                pack.Returncode = ReturnCode.Succeed;
                return pack;
            }
            catch
            {
                pack.Returncode = ReturnCode.Fail;
                return pack;
            }
            
        }
        public MainPack FindRoom()
        {
            MainPack pack = new MainPack();
            pack.Actioncode = ActionCode.FindRoom;
            try
            {
                if (roomList.Count == 0)
                {
                    pack.Returncode = ReturnCode.NotRoom;
                    return pack;
                }
                foreach (Room room in roomList)
                {           
                    pack.Roompack.Add(room.GetRoomInFo);
                }
                pack.Returncode = ReturnCode.Succeed;
            }
            catch
            {
                pack.Returncode = ReturnCode.Fail;
            }
            return pack;
        }
        public MainPack JoinRoom(Client client,MainPack pack)
        {
            foreach(Room r in roomList)
            {
                if (r.GetRoomInFo.Roomname.Equals(pack.Str))
                {
                    //有房间 等待加入
                    if(r.GetRoomInFo.Status == 0)
                    {
                        //可以加入房间
                        r.Join(client);
                        pack.Roompack.Add(r.GetRoomInFo);
                        foreach (PlayerPack p in r.GetPlayerInFo())
                        {
                            pack.Playerpack.Add(p);
                        }
                        pack.Returncode = ReturnCode.Succeed;
                        return pack;
                    }
                    else
                    {
                        //不可以加入房间
                        pack.Returncode = ReturnCode.Fail;
                        return pack;
                    }
                }
            }
            //没有此房间
            pack.Returncode = ReturnCode.NotRoom;
            return pack;
        }
        public MainPack ExitRoom(Client client,MainPack pack)
        {
            if (client.GetRoom == null)
            {
                pack.Returncode = ReturnCode.Succeed;
                return null;
            }
            client.GetRoom.Exit(this,client);
            pack.Returncode = ReturnCode.Succeed;
            return pack;
        }
        public void RemoveRoom(Room room)
        {
            roomList.Remove(room);
        }
        public void Chat(Client client,MainPack pack)
        {
            pack.Str = client.GetUserInFo.UserName + ":" + pack.Str;
            client.GetRoom.Broadcast(client, pack);
        }
    }
}
