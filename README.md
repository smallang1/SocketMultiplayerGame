# SocketMultiplayerGame

基于 TCP Socket 的 Unity 2D 双人对战游戏，客户端使用 Unity + C# 开发，服务端使用 .NET 8 搭建，通信协议采用 Protobuf，数据持久化使用 MySQL。

## 功能概览

- 用户注册 / 登录（MySQL 数据库验证）
- 房间系统（创建、查询、加入、退出）
- 房内实时聊天
- 实时位置同步（角色移动、枪口朝向）
- 射击对战（子弹同步、伤害计算、血量显示）
- 胜负判定（血量归零 -> 显示结果 -> 倒计时返回房间列表）

## 技术栈

| 层级 | 技术 |
|------|------|
| 客户端 | Unity 2022.3、C# |
| 服务端 | .NET 8、C# |
| 通信协议 | Protobuf |
| 网络通信 | TCP Socket（异步收发） |
| 数据库 | MySQL |

## 项目结构

客户端（Unity）:
- Assets/Scripts/GameFace.cs — 客户端核心入口，管理所有 Manager
- Assets/Scripts/Manager/ — ClientManager、UIManager、RequestManager、PlayerManager
- Assets/Scripts/Request/ — 各功能模块的网络请求组件（登录、房间、游戏等）
- Assets/Scripts/UIPanel/ — UI 面板（Start、Login、RoomList、Room、Game 等）
- Assets/Scripts/Player/ — 角色控制、枪控制、子弹
- Assets/Scripts/Message.cs — TCP 消息缓冲区管理（粘包处理）
- Assets/Scripts/SocketGameProtocol.cs — Protobuf 生成的协议代码

服务端（.NET 8）:
- Server.cs — TCP 监听、房间管理
- Client.cs — 客户端连接、消息收发
- Servers/Room.cs — 房间逻辑：加入、退出、广播、伤害、游戏开始
- Controller/ — UserController、RoomController、GameController（反射路由）
- DAO/UserDate.cs — MySQL 数据访问
- Tool/Message.cs — TCP 消息缓冲区管理

## 通信协议

使用 Protobuf 定义统一消息格式 MainPack，客户端与服务端共享同一份生成的代码：

```protobuf
message MainPack {
    RequestCode requestcode = 1;
    ActionCode actioncode = 2;
    ReturnCode returncode = 3;
    LoginPack loginpack = 4;
    string str = 5;
    repeated RoomPack roompack = 6;
    repeated PlayerPack playerpack = 7;
    BulletPack bulletPack = 8;
}
```

## 系统架构

客户端:
```
GameFace (入口)
  ├── ClientManager    → Socket 连接、消息收发
  ├── UIManager        → 面板栈管理 UI 切换
  ├── RequestManager   → 按 ActionCode 分发响应到对应 Request
  └── PlayerManager    → 角色实例化、位置同步、子弹生成
```

服务端:
```
请求到达 → ControllerManager（反射查找方法）
  ├── UserController   → 注册 / 登录（MySQL）
  ├── RoomController   → 房间 CRUD、开始游戏
  └── GameController   → 位置/开火广播、伤害计算、退出游戏
```

## 核心技术点

### TCP 粘包处理

在 ReadBuffer 循环内每次重新从缓冲区头部读取消息长度，处理完一条消息后将剩余数据前移，确保 TCP 合包场景下每条消息都能正确解析。

### 位置同步

本地玩家每 0.05 秒（20 次/秒）发送 PosPack（位置 + 角色旋转 + 枪口角度）到服务器，服务器广播给同房间其他客户端，客户端更新远程角色的位置和枪口翻转。

### 控制隔离

角色预制体不挂控制脚本，运行时仅对本地玩家通过 AddComponent 添加 CharacterController 和 GunController，其他玩家的角色不响应本地输入。

### 射击命中与伤害同步

子弹在本地检测碰撞 -> 发送命中通知 -> 服务器扣血校验 -> 广播血量更新 -> 血量归零触发游戏结束。

## 快速开始

### 环境要求

- Unity 2022.3 LTS
- .NET 8 SDK
- MySQL 5.7+
- Protobuf 编译器（protoc，仅修改协议时需要）

### 数据库配置

在 MySQL 中创建数据库 sys，新建用户表：

```sql
CREATE TABLE `userdata` (
  `username` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL
);
```

修改服务端 Client.cs 中的数据库连接字符串（账号、密码、端口）。

### 编译协议（可选）

如需修改协议，编辑 SocketGameProtocol.proto 后重新生成：

```bash
protoc --csharp_out=. SocketGameProtocol.proto
```

将生成的 SocketGameProtocol.cs 分别复制到客户端和服务端项目。

### 运行

1. 编译并启动服务端 SocketMultiplayerGameServer
2. 用 Unity 打开客户端项目 SocketMultiplayerGame
3. 运行场景 SampleScene
4. 开两个客户端实例分别注册、登录、创建/加入房间、开始对战

## 操作说明

| 操作 | 按键 |
|------|------|
| 左右移动 | A / D |
| 跳跃 | W |
| 瞄准 | 鼠标移动 |
| 开火 | 鼠标左键 |
| 退出游戏 | 游戏界面退出按钮 |
