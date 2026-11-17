using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocol; //引入协议类
using System.Reflection; //引入反射类
using SocketMultiplayerGameServer.Servers;

namespace SocketMultiplayerGameServer.Controller
{
    internal class ControllerManager
    {
        private Dictionary<RequestCode, BaseController> controllerDict = new Dictionary<RequestCode, BaseController>();
        private Server server; //服务端对象

        /// <summary>
        /// 构造函数，初始化控制器字典
        /// </summary>
        public ControllerManager(Server sever)
        {
            this.server = sever;

            //初始化控制器字典
            UserController  userController= new UserController();
            controllerDict.Add(userController.GetRequestCode, userController);
            RoomController roomController = new RoomController();
            controllerDict.Add(roomController.GetRequestCode, roomController);
            GameController gameController = new GameController();
            controllerDict.Add(gameController.GetRequestCode,gameController);
        }
        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="pack"></param>
        public void HandleRequest(MainPack pack,Client client)
        {
            //从控制器字典中获取对应的控制器对象
            if(controllerDict.TryGetValue(pack.Requestcode, out BaseController controller))
            {
                string methodName = pack.Actioncode.ToString();
                MethodInfo method = controller.GetType().GetMethod(methodName);
                if(method == null)
                {
                    Console.WriteLine("没有找到对应的action处理");
                    return;
                }
                object[] obj = new object[] { server,client,pack};
                object ret = method.Invoke(controller, obj);
                if(ret != null)
                {
                    client.Send(ret as MainPack);
                }
            }
            else
            {
                Console.WriteLine("没有找到对应的controller处理");
            }
        }
    }
}
