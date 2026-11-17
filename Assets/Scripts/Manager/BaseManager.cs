using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager
{
    protected GameFace face;

    public BaseManager(GameFace face)
    {
        this.face = face;
    }
    /// <summary>
    /// 初始化方法
    /// </summary>
    public virtual void OnInit()
    {

    }
    public virtual void OnDestroy()
    {

    }
}
