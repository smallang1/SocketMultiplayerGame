using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    public GameObject item;
    public Transform listTransform;
    public Text Timetext;
    public Button exitBtn;
    public GameExitRequest gameExitRequest;

    private float starttime;
    private bool isGameOver = false;
    public bool IsGameOver { get { return isGameOver; } }

    private Dictionary<string, PlayerInFoItem> itemList = new Dictionary<string, PlayerInFoItem>();

    private void Start()
    {
        starttime = Time.time;
        exitBtn.onClick.AddListener(OnExitClick);
    }


    public void UpdateList(MainPack packs)
    {
        for (int i = 0; i < listTransform.childCount; i++)
        {
            GameObject.Destroy(listTransform.GetChild(i).gameObject);
        }
        itemList.Clear();
        foreach(var p in packs.Playerpack)
        {
            GameObject g = Instantiate(item, Vector3.zero, Quaternion.identity);
            g.transform.SetParent(listTransform);
            PlayerInFoItem playinfo = g.GetComponent<PlayerInFoItem>();
            playinfo.Set(p.Playername, p.Hp);
            itemList.Add(p.Playername,playinfo);
        }
    }
    public void UpdateValue(string id,int v)
    {
        if(itemList.TryGetValue(id, out PlayerInFoItem playerinfo))
        {
            playerinfo.Up(v);
        }
        else
        {
            Debug.Log("获取不到对应得角色信息");
        }
    }
    private void FixedUpdate()
    {
        if (!isGameOver)
            Timetext.text = Mathf.Clamp((int)(Time.time - starttime), 0, 300).ToString();
    }
    private void OnExitClick()
    {
        gameExitRequest.SendRequest();
        face.GameExit();
    }
    public void GameOver(bool iWon)
    {
        isGameOver = true;
        StartCoroutine(GameOverCoroutine(iWon));
    }
    private IEnumerator GameOverCoroutine(bool iWon)
    {
        Timetext.text = iWon ? "你赢了！" : "你输了！";
        yield return new WaitForSeconds(1);
        for (int i = 3; i > 0; i--)
        {
            Timetext.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        gameExitRequest.SendRequest();
        face.GameExit();
    }
    public override void OnEnter()
    {
        base.OnEnter();
        gameObject.SetActive(true);
    }
    public override void OnPause()
    {
        base.OnPause();
        gameObject.SetActive(false);
    }
    public override void OnRecovery()
    {
        base.OnRecovery();
        gameObject.SetActive(true);
    }
    public override void OnExit()
    {
        base.OnExit();
        gameObject.SetActive(false);
    }
}
