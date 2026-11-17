using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketGameProtocol;

public class Bullet : MonoBehaviour
{
    private float spawnTime;

    private void Start()
    {
        spawnTime = Time.time;
        Destroy(gameObject, 3f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Time.time - spawnTime < 0.1f)
            return;
        var root = collision.transform.root;
        string rootName = root.name;
        // 不打自己
        if (rootName == GameFace.Face.UserName)
            return;
        // 命中其他玩家（排除 land 等非玩家物体）
        if (root != transform.root && !rootName.StartsWith("land"))
        {
                MainPack pack = new MainPack();
                pack.Requestcode = RequestCode.Game;
                pack.Actioncode = ActionCode.UpCharacterList;
                pack.Str = root.name;
                GameFace.Face.Send(pack);
        }
        Destroy(gameObject);
    }
}
