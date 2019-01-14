﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public sealed class UIManager : Singleton<UIManager>
{
    private Transform normalUI;
    private Transform hideUI;
    private Transform tiptopUI;
    public GameObject canves { get; private set; }
    private UIManager()
    {
        canves = GameObject.Find("UICanvas");
        if (canves)
        {
            normalUI = canves.transform.Find("uiRoot/normalUI");
            hideUI = canves.transform.Find("uiRoot/hideUI");
            tiptopUI = canves.transform.Find("uiRoot/tiptopUI");
        }
    }
    private Dictionary<string, UIDataBase> dicDlg = new Dictionary<string, UIDataBase>();

    private UIDataBase GetDigLog(string dlgName)
    {
        if (string.IsNullOrEmpty(dlgName))
            return null;
        UIDataBase dlg = null;
        GameObject uiRoot = null;
        if (dicDlg.TryGetValue(dlgName, out dlg))
        {
            uiRoot = dlg.gameObject;
        }
        else
        {
            dlg = RegisterDlgScripte(dlgName, out uiRoot);
        }
        SetUIRootParent(uiRoot, dlg.ShowPos);
        return dlg;
    }
    private void SetUIRootParent(GameObject uiRoot, UIShowPos type)
    {
        Transform Parent = null;
        if (type == UIShowPos.Normal)
            Parent = normalUI;
        else if (type == UIShowPos.TipTop)
            Parent = tiptopUI;
        else
            Parent = hideUI;
        uiRoot.transform.SetParent(Parent);
        uiRoot.transform.localPosition = Vector3.zero;
        uiRoot.transform.localScale = Vector3.one;
        uiRoot.transform.localRotation = Quaternion.identity;
    }

    private void SaveUIRoot(string dlgName, UIDataBase dlg)
    {
        if (dlg)
            dicDlg.Add(dlgName, dlg);
        else
            GameObject.Destroy(dlg.gameObject);
    }
    //需要手动注册脚本
    private UIDataBase RegisterDlgScripte(string dlgName, out GameObject uiRoot)
    {
        UIDataBase dlg = null;
        uiRoot = null;
        if (!string.IsNullOrEmpty(dlgName))
        {
            uiRoot = SetRootPro(dlgName);
            switch (dlgName)
            {
                case UIMovePage.NAME: dlg = uiRoot.AddComponent<UIMovePage>(); break;
                case UITimePage.NAME: dlg = uiRoot.AddComponent<UITimePage>(); break;
                case UIPromptPage.NAME: dlg = uiRoot.AddComponent<UIPromptPage>(); break;
                case UIMovieQRCodePage.NAME: dlg = uiRoot.AddComponent<UIMovieQRCodePage>(); break;
                case UIMessagePage.NAME: dlg = uiRoot.AddComponent<UIMessagePage>(); break;
                case UIBgPage.NAME: dlg = uiRoot.AddComponent<UIBgPage>(); break;
                case UIQuestionPage.NAME: dlg = uiRoot.AddComponent<UIQuestionPage>(); break;
                case UILoadingPage.NAME: dlg = uiRoot.AddComponent<UILoadingPage>(); break;
                case UITurnTablePage.NAME: dlg = uiRoot.AddComponent<UITurnTablePage>(); break;
                case UITurnResultPage.NAME: dlg = uiRoot.AddComponent<UITurnResultPage>(); break;
                case UITurnSplashPage.NAME: dlg = uiRoot.AddComponent<UITurnSplashPage>(); break;
                case UITurnCodePage.NAME: dlg = uiRoot.AddComponent<UITurnCodePage>(); break;
                case UIPhoneCodePage.NAME: dlg = uiRoot.AddComponent<UIPhoneCodePage>(); break;

            }
            SaveUIRoot(dlgName, dlg);
        }
        return dlg;
    }


    private GameObject SetRootPro(string dlgName)
    {
        GameObject uiRoot = new GameObject(dlgName);
        uiRoot.SetActive(true);
        uiRoot.layer = LayerMask.NameToLayer("UI");
        return uiRoot;
    }
    public void ShowUI(string dlgName, bool isShow, object data = null, Action<GameObject> act = null)
    {
        UIDataBase dlg;
        dlg = GetDigLog(dlgName);
        if (dlg)
        {
            if (isShow)
            {
                dlg.gameObject.SetActive(true);
                dlg.OnShow(data);
                //if (dlg.isOpenDoTween)
                //    dlg.transform.DOPunchScale(Vector3.one,2,5,0);
            }
            else
            {
                dlg.gameObject.SetActive(false);
                if (dlg.hidePage == HidePage.Hide)
                {
                    dlg.OnHide();
                    SetUIRootParent(dlg.gameObject, UIShowPos.Hide);
                }
                else
                {
                    GameObject.Destroy(dlg.gameObject);
                    dicDlg.Remove(dlgName);
                    return;
                }
            }
            if (act != null)
                act(dlg.gameObject);
        }
    }

    public void Clear()
    {
        canves.SetActive(false);
        foreach (var item in dicDlg)
        {
            if (item.Value != null)
                GameObject.Destroy(item.Value.gameObject);
        }
        dicDlg.Clear();
        Dispose();
    }

}
