using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocol;
using Google.Protobuf.Collections;
using System.Threading;

namespace SocketMultiplayerGameServer.Servers
{
    internal class Room
    {
        private RoomPack roominfo;//房间信息
        private Server server;

        private List<Client> clientList = new List<Client>(); //房间内所有的客户端

        /// <summary>
        /// 返回房间信息
        /// </summary>
        public RoomPack GetRoomInFo
        {
            get
            {
                roominfo.Curnum = clientList.Count;
                return roominfo;
            }
        }
        public Room(Client client,RoomPack pack,Server server)
        {
            roominfo = pack;
            clientList.Add(client);
            client.GetRoom = this;
            this.server = server;
            
        }      

        public RepeatedField<PlayerPack> GetPlayerInFo()
        {
            RepeatedField<PlayerPack> pack = new RepeatedField<PlayerPack>(); 
            foreach (Client c in clientList)
            {
                PlayerPack player = new PlayerPack();
                player.Playername = c.GetUserInFo.UserName;
                pack.Add(player);
            }
            return pack;
        }
        public void Broadcast(Client client,MainPack pack)
        {
            foreach(Client c in clientList)
            {
                if (c.Equals(client))
                {
                    continue;
                }
                c.Send(pack);
            }
        }
        public void Join(Client client)
        {
            clientList.Add(client);
            if (roominfo.Maxnum >= clientList.Count)
            {
                //满人了
                roominfo.Status = 1;
            }
            client.GetRoom = this;
            MainPack pack = new MainPack();
            pack.Actioncode = ActionCode.PlayerList;
            foreach(PlayerPack palyer in GetPlayerInFo())
            {
                pack.Playerpack.Add(palyer);
            }
            Broadcast(client, pack);
        }
        public void Exit(Server server,Client client)
        {
            MainPack pack = new MainPack();
            if (client == clientList[0])
            {
                //房主退出房间
                client.GetRoom = null;
                pack.Actioncode = ActionCode.Exit;
                Broadcast(client, pack);
                server.RemoveRoom(this);
                return;
            }
            clientList.Remove(client);
            roominfo.Status = 0;
            client.GetRoom = null;
            pack.Actioncode = ActionCode.PlayerList;
            foreach (PlayerPack palyer in GetPlayerInFo())
            {
                pack.Playerpack.Add(palyer);
            }
            Broadcast(client, pack);
        }
        public ReturnCode StartGame(Client client)
        {
            if (client != clientList[0])
            {
                return ReturnCode.Fail;
            }
            Thread starttime = new Thread(Time);
            starttime.Start();
            return ReturnCode.Succeed;
        }
        private void Time()
        {
            MainPack pack = new MainPack();
            pack.Actioncode = ActionCode.Chat;
            pack.Str = "房主已启动游戏...";
            Broadcast(null,pack);
            Thread.Sleep(1000);
            for (int i = 5; i > 0; i--)
            {
                pack.Str = i.ToString();
                Broadcast(null, pack);
                Thread.Sleep(1000);
            }
            pack.Actioncode = ActionCode.Starting;


            foreach(var VARIABLE in clientList)
            {
                PlayerPack player = new PlayerPack();
                VARIABLE.GetUserInFo.Hp = 100;
                player.Playername = VARIABLE.GetUserInFo.UserName;
                player.Hp = VARIABLE.GetUserInFo.Hp;
                pack.Playerpack.Add(player);
            }
            Broadcast(null,pack);
        }
        /// <summary>
        /// 处理伤害：扣血、检查死亡、返回更新包
        /// </summary>
        public MainPack TakeDamage(string targetName, Client attacker)
        {
            foreach (var c in clientList)
            {
                if (c.GetUserInFo.UserName == targetName && c.GetUserInFo.Hp > 0)
                {
                    c.GetUserInFo.Hp -= 20;
                    MainPack pack = new MainPack();
                    pack.Actioncode = ActionCode.UpCharacterList;
                    if (c.GetUserInFo.Hp <= 0)
                    {
                        c.GetUserInFo.Hp = 0;
                        pack.Returncode = ReturnCode.Fail; // 游戏结束
                        pack.Str = attacker.GetUserInFo.UserName; // 赢家
                    }
                    else
                    {
                        pack.Returncode = ReturnCode.Succeed; // 血量更新
                    }
                    foreach (var p in clientList)
                    {
                        PlayerPack pp = new PlayerPack();
                        pp.Playername = p.GetUserInFo.UserName;
                        pp.Hp = p.GetUserInFo.Hp;
                        pack.Playerpack.Add(pp);
                    }
                    return pack;
                }
            }
            return null;
        }
        public void ExitGame(Client client)
        {
            MainPack pack = new MainPack();
            if (client == clientList[0])
            {
                //房主退出
                pack.Actioncode = ActionCode.ExitGame;
                pack.Str = "r";
                Broadcast(client,pack);
                server.RemoveRoom(this);
            }
            else
            {
                //其他玩家退出
                clientList.Remove(client);
                pack.Actioncode = ActionCode.UpCharacterList;
                foreach(var VARIABLE in clientList)
                {
                    PlayerPack playerPack = new PlayerPack();
                    playerPack.Playername = VARIABLE.GetUserInFo.UserName;
                    playerPack.Hp = VARIABLE.GetUserInFo.Hp;
                    pack.Playerpack.Add(playerPack);
                }
                pack.Str = client.GetUserInFo.UserName;
                Broadcast(client, pack);
            }
        }
    }
}
