using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using SocketMultiplayerGameServer.Tool; //引入消息管理类
using SocketMultiplayerGameServer.DAO; //引入数据访问对象
using SocketGameProtocol;
using MySql.Data.MySqlClient; //引入协议类

namespace SocketMultiplayerGameServer.Servers
{
    internal class Client
    {
        private const string connstr = "database=sys;data source=localhost;username=root;password='906728498';pooling=false;charset=utf8;port=3306";

        private Socket socket; 
        private Message message;
        private UserDate userDate;
        private Server server;
        private MySqlConnection sqlConnection;

        public UserInFo GetUserInFo
        {
            get;
            set;
        }
        public class UserInFo
        {
            public string UserName
            {
                get; set;
            }
            public int Hp
            {
                get;
                set;
            }
            public PosPack pos
            {
                get;
                set;
            }
        }
        public Room GetRoom
        {
            get; set;
        }

        public UserDate GetUserData
        {
            get
            {
                return userDate;
            }
        }
        public MySqlConnection GetMysqlConnection
        {
            get
            {
                return sqlConnection;
            }
        }

        /// <summary>
        /// 构造函数，初始化Socket对象和消息管理对象
        /// </summary>
        public Client(Socket socket,Server server)
        {
            this.socket = socket;
            this.server = server;

            sqlConnection = new MySqlConnection(connstr);

            GetUserInFo = new UserInFo();

            sqlConnection.Open(); //打开数据库连接

            message = new Message();
            userDate = new UserDate();

            StartReceive(); //开始接收消息
        }
        void StartReceive()
        {
            //开始接收消息，当有消息时，调用ReceiveCallback方法
            //参数1：消息缓冲区
            //SocketFlags 套节字标志
            socket.BeginReceive(message.Buffer,message.Startindex,message.Remsize,SocketFlags.None, ReceiveCallback, null);
        }
        void ReceiveCallback(IAsyncResult iar)
        {
            try
            {
                if(socket == null || socket.Connected == false) return;
                //结束接收消息
                int len = socket.EndReceive(iar);
                if (len == 0)
                {
                    Close();
                    return;
                }    
                message.ReadBuffer(len,HandleRequest);
                StartReceive(); //继续接收消息
            }
            catch(Exception e)
            {
                Close();
            }
        }
        public void Send(MainPack pack)
        {
            socket.Send(Message.PackData(pack));
        }

        void HandleRequest(MainPack pack)
        {
            server.HandleRequest(pack,this);
        }

        public void Close()
        {
            if (GetRoom != null)
            {
                GetRoom.Exit(server,this);
            }
            server.RemoveClient(this); //从服务端移除客户端
            socket.Close(); //关闭Socket连接
            sqlConnection.Close(); //关闭数据库连接
        }   
    }
}
