using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class UIViewBase : AnimateRegBase
{
    public string _DlgName
    {
        get { return gameObject.name; }
    }

    //是否已加载
    public bool _isLoaded { get; private set; }

    #region 加载预制物体
    private void Awake()
    {
        //异步加载
        StartCoroutine(LoadGameObj((obj) =>
           {
               obj.transform.SetParent(gameObject.transform, false);
               InitAnimator(obj);
               RegAnimateEvent();
               InitCodeData();
               OnInit();
               OnCreate();
              // OnEnter();
               _isLoaded = true;
           }));
    }

    /// <summary>
    /// 注册动画事件
    /// </summary>
    private void RegAnimateEvent()
    {
        //默认传空
        RegEnterAnimateEvent(null);
        RegPauseAnimateEvent(null);
        RegResumeAnimateEvent(null);
        RegExitAnimateEvent();//不需要 给参数  重写时需要
    }

    /// <summary>
    /// 从resources加载
    /// </summary>
    /// <param name="act"></param>
    /// <returns></returns>
    IEnumerator LoadGameObj(Action<GameObject> act)
    {
        if (!string.IsNullOrEmpty(_DlgName))
        {
            Debug.Log("----加载ui预制物体--" + _DlgName);
            string uipath = "UIPrefab/" + _assetFolder + "/" + _DlgName;
            GameObject obj = Resources.Load<GameObject>(uipath);
            if (obj)
            {
                GameObject temp = Instantiate<GameObject>(obj);
                if (act != null && temp)
                {
                    act(temp);
                }
            }
            else
                Debug.LogError(_DlgName + "----不存在");
        }
        else
        {
            Debug.LogError("DlgName是空");
        }
        yield break;
    }

    //隐藏自己
    protected void HideSelf()
    {
        UIMgr.Instance.ShowUI(_DlgName, false);
    }


    //   #region 加载assetBundle
    ///// <summary>
    //   /// 异步从下载路径加载assetbundle
    //   /// </summary>
    //   /// <param name="act"></param>
    //   void LoadGameObjByAsync(Action<GameObject> act)
    //   {
    //       if (!string.IsNullOrEmpty(_DlgName))
    //       {
    //           LoadAssetMrg.Instance.LoadAssetAsync(_DlgName, bd =>
    //            {
    //                if (bd != null)
    //                {
    //                    GameObject obj = (GameObject)bd.mAsset;
    //                    if (obj)
    //                    {
    //                        GameObject temp = Instantiate<GameObject>(obj);
    //                        if (act != null && temp)
    //                            act(temp);
    //                    }
    //                }
    //                else
    //                    Debug.LogError(_DlgName + "----不存在");
    //            });
    //       }
    //       else
    //           Debug.LogError("DlgName是空");
    //   }
    //   /// <summary>
    //   /// 同步从下载路径加载assetbundle
    //   /// </summary>
    //   /// <param name="act"></param>
    //   void LoadGameObjBySync(Action<GameObject> act)
    //   {
    //       if (!string.IsNullOrEmpty(_DlgName))
    //       {
    //           Bundle bd = LoadAssetMrg.Instance.LoadAsset(_DlgName);
    //           if (bd != null)
    //           {
    //               GameObject obj = (GameObject)bd.mAsset;
    //               if (obj)
    //               {
    //                   GameObject temp = Instantiate<GameObject>(obj);
    //                   if (act != null && temp)
    //                       act(temp);
    //               }
    //           }
    //           else
    //               Debug.LogError(_DlgName + "----不存在");
    //       }
    //       else
    //           Debug.LogError("DlgName是空");

    //   }
    //#endregion


    #endregion

}
