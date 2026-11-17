using SocketGameProtocol;
using SocketMultiplayerGameServer.Servers;

namespace SocketMultiplayerGameServer.Controller
{
    internal class GameController : BaseController
    {
        public GameController()
        {
            requestCode = RequestCode.Game;
        }
        // 位置同步：广播给房间内其他客户端
        public MainPack UpPos(Server server, Client client, MainPack pack)
        {
            if (client.GetRoom != null)
                client.GetRoom.Broadcast(client, pack);
            return null;
        }
        // 开火同步：广播给房间内其他客户端
        public MainPack Fire(Server server, Client client, MainPack pack)
        {
            if (client.GetRoom != null)
                client.GetRoom.Broadcast(client, pack);
            return null;
        }
        // 命中/血量更新：扣血、检查死亡、广播
        public MainPack UpCharacterList(Server server, Client client, MainPack pack)
        {
            if (client.GetRoom == null) return null;
            MainPack result = client.GetRoom.TakeDamage(pack.Str, client);
            if (result != null)
                client.GetRoom.Broadcast(null, result);
            return null;
        }
        // 退出游戏
        public MainPack ExitGame(Server server, Client client, MainPack pack)
        {
            if (client.GetRoom != null)
                client.GetRoom.ExitGame(client);
            return null;
        }
    }
}
