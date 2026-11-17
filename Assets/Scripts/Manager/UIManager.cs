using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : BaseManager
{
    public UIManager(GameFace face) : base(face){}

    private Dictionary<PanelType,BasePanel> panelDict = new Dictionary<PanelType, BasePanel>();
    private Dictionary<PanelType, string> panelPath = new Dictionary<PanelType, string>();
   
    private Stack<BasePanel> panelStack = new Stack<BasePanel>(); //栈用于管理UI面板

    private Transform canvasTransform;
    private MessagePanel messagePanel;
    override public void OnInit()
    {
        base.OnInit();
        InitPanel(); 
        canvasTransform = GameObject.Find("Canvas").transform; //获取Canvas组件
        PushPanel(PanelType.Message);
        PushPanel(PanelType.Start);
    }

    /// <summary>
    /// 把UI显示在界面上
    /// </summary>
    public BasePanel PushPanel(PanelType panelType)
    {
        if(panelDict.TryGetValue(panelType,out BasePanel panel))
        {
            if(panelStack.Count > 0) //如果栈中有UI面板{
            {
                BasePanel topPanel = panelStack.Peek();
                topPanel.OnPause();
            }
            panelStack.Push(panel); //将UI面板压入栈中
            panel.OnEnter();
            return panel;
        }
        else
        {
            BasePanel panel1 = SpawnPanel(panelType); //实例化UI面板
            if(panelStack.Count > 0)
            {
                BasePanel topPanel = panelStack.Peek();
                topPanel.OnPause();
            }
            panelStack.Push(panel1); //将UI面板压入栈中
            panel1.OnEnter();
            return panel1;
        }
    }
    /// <summary>
    /// 关闭当前UI
    /// </summary>
    public void PopPanel()
    {
        if(panelStack.Count == 0) return;

        BasePanel topPanel = panelStack.Pop();
        topPanel.OnExit();

        BasePanel Panel = panelStack.Peek();
        Panel.OnRecovery(); //恢复UI面板
    }
    /// <summary>
    /// 实例化对应UI面板
    /// </summary>
    private BasePanel SpawnPanel(PanelType panelType)
    {
        if(panelPath.TryGetValue(panelType,out string path))
        {
            GameObject g = GameObject.Instantiate(Resources.Load(path)) as GameObject;
            g.transform.SetParent(canvasTransform, false);
            BasePanel panel = g.GetComponent<BasePanel>();
        if (panel == null)
        {
            Debug.LogError("预制体 " + path + " 上缺少 BasePanel 组件");
            GameObject.Destroy(g);
            return null;
        }
            panel.SetUIMag = this;
            panelDict.Add(panelType, panel);
            return panel; //返回UI面板
        }
        else
        {
            Debug.Log("UI面板不存在");
            return null;
        }
    }
    /// <summary>
    /// 初始化UI面板路径
    /// </summary>
    private void InitPanel()
    {
        const string prefix = "Panel/";
        foreach (PanelType type in System.Enum.GetValues(typeof(PanelType)))
        {
            panelPath[type] = prefix + type + "Panel";
        }
    }

    public void SetMessagePanel(MessagePanel message) 
    {
        messagePanel = message;
    }
    public void ShowMessage(string str,bool sync = false)
    {
        messagePanel.ShowMessage(str,sync);
    }

}
