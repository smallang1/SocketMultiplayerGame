using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketGameProtocol;
using System.Globalization;
public class PlayerManager : BaseManager
{
    public PlayerManager(GameFace face) : base(face)
    {
    }

    //存放玩家的容器
    Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

    private GameObject character;
    private GameObject bullet;
    private Transform spawnPos;
    public override void OnInit()
    {
        base.OnInit();
        character = Resources.Load("Prefab/Character") as GameObject;
        bullet = Resources.Load("Prefab/bullet") as GameObject;
    }

    public string CurPlayerID
    {
        get;set;
    }
    public void AddPlayer(MainPack pack)
    {
        spawnPos = GameObject.Find("SpawnPos").transform;
        foreach(var p in pack.Playerpack)
        {
            Debug.Log("添加角色" + p.Playername);
            GameObject g = GameObject.Instantiate(character, spawnPos.position, Quaternion.identity);
            g.name = p.Playername;
            if (p.Playername.Equals(face.UserName))
            {
                //创建本地角色 添加脚本
                g.AddComponent<Rigidbody2D>().gravityScale = 3;
                g.AddComponent<UpPosRequest>();
                g.AddComponent<UpPos>();
                g.AddComponent<CharacterController>();
                g.transform.Find("HandGun").gameObject.AddComponent<FireRequest>();
                g.transform.Find("HandGun").gameObject.AddComponent<GunController>();
            }
            else
            {
                //创建其他客户端的角色
            } 
            players.Add(p.Playername, g);
        }
    }
    public void RemovePlayer(string id)
    {
        if(players.TryGetValue(id, out GameObject g))
        {
            GameObject.Destroy(g);
            players.Remove(id);
        }
        else
        {
            Debug.Log("移除角色出错");
        }
    }
    public void GameExit()
    {
        foreach(var VARIABLE in players.Values)
        {
            //清理角色列表
            GameObject.Destroy(VARIABLE);
        }
        players.Clear();
    }
    public void UpPos(MainPack pack)
    {
        PosPack posPack = pack.Playerpack[0].Pospack;
        if(players.TryGetValue(pack.Playerpack[0].Playername,out GameObject g))
        {
            Vector2 pos = new Vector2(posPack.PosX,posPack.PosY);//位置信息
            float characterRot = posPack.RotZ;
            float gunRot = posPack.GunPosZ;

            g.transform.position = pos;
            g.transform.eulerAngles = new Vector3(0, 0, characterRot);
            var handGun = g.transform.Find("HandGun");
            if (handGun != null)
            {
                handGun.eulerAngles = new Vector3(0, 0, gunRot);
                // 和 GunController.LookAt 一致的 flipY 逻辑
                var sr = handGun.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    float normalized = ((gunRot % 360) + 360) % 360;
                    sr.flipY = (normalized >= 90 && normalized <= 270);
                }
            }
        }
    }
    public void SpawnBullet(MainPack pack)
    {
        Vector3 pos = new Vector3(pack.BulletPack.PosX,pack.BulletPack.PosY,0);
        float rot = pack.BulletPack.RotZ;
        Vector3 mousePos = new Vector3(pack.BulletPack.MousePosX, pack.BulletPack.MousePosY, 0);
        Vector3 velocity = (mousePos - pos).normalized * 20;
        GameObject g= GameObject.Instantiate(bullet, pos, Quaternion.identity);
        g.transform.eulerAngles = new Vector3(0, 0, rot);
        g.GetComponent<Rigidbody2D>().velocity = velocity;
    }
}
