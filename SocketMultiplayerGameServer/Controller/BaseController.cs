using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketGameProtocol; //引入协议类

namespace SocketMultiplayerGameServer.Controller
{
    internal abstract class BaseController
    {
        protected RequestCode requestCode = RequestCode.RequestNone;
        public RequestCode GetRequestCode
        {
            get{ return requestCode;}
        }
    }
}
