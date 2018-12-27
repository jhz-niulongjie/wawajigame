using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class UIDataBase : Base
{
    public string DlgName
    {
        get { return gameObject.name; }
    }
    public abstract UIShowPos ShowPos
    {
        get;
    }

    public abstract HidePage hidePage
    {
        get;
    }
    public abstract AssetFolder assetFolder
    {
        get;
    }
    //是否打开dotween动画
    public bool isOpenDoTween { get; protected set; }

    private void Awake()
    {
        Action<GameObject> action = obj =>
          {
              obj.transform.parent = gameObject.transform;
              obj.transform.localPosition = Vector3.zero;
              obj.transform.localScale = Vector3.one;
              obj.transform.localRotation = Quaternion.identity;
              InitCodeData();
              Init();
              OnOpen();
          };

        StartCoroutine(LoadGameObj(action));
    }
    IEnumerator LoadGameObj(Action<GameObject> act)
    {
        if (!string.IsNullOrEmpty(DlgName))
        {
            Debug.Log("----实列化ui预制物体--"+DlgName);
            string uipath = "UIPrefab/"+assetFolder+"/" + DlgName;
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
                Debug.LogError(DlgName+"----不存在");
        }
        else
        {
            Debug.LogError("DlgName是空");
        }
        yield break;
    }

    public virtual void Init()
    {
       
    }
    public virtual void OnOpen()
    {

    }
    public virtual void OnShow(object data)
    {
    }
    public virtual void OnHide()
    {
    }
    public virtual void HideSelf()
    {
        UIManager.Instance.ShowUI(DlgName, false, null, null);
    }

}
