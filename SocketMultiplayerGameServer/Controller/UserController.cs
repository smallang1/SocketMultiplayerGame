using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocol; //引入协议类
using SocketMultiplayerGameServer.Servers;

namespace SocketMultiplayerGameServer.Controller
{
    internal class UserController : BaseController
    {
        public UserController()
        {
            requestCode = RequestCode.User;
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        public MainPack Logon(Server sever,Client client,MainPack pack)
        {
            if(client.GetUserData.Logon(pack,client.GetMysqlConnection))
            {
                pack.Returncode = ReturnCode.Succeed;
            }
            else
            {
                pack.Returncode = ReturnCode.Fail;
            }
            return pack;
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public MainPack Login(Server sever, Client client, MainPack pack)
        {
            if (client.GetUserData.Login(pack, client.GetMysqlConnection))
            {
                pack.Returncode = ReturnCode.Succeed;
                client.GetUserInFo.UserName = pack.Loginpack.Username;
            }
            else
            {
                pack.Returncode = ReturnCode.Fail;
            }
            return pack;
        }
    }
}
