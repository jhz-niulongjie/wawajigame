using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EventDispatcher {

    private static Dictionary<EventHandlerType, List<Delegate>> delegateDic = new Dictionary<EventHandlerType, List<Delegate>>();

    #region 添加监听
    public static void AddListener<T1, T2, T3, T4>(EventHandlerType eht,Action<T1, T2, T3, T4> callback)
    {
        AddListener(eht, (Delegate)callback);
    }
    public static void AddListener<T1, T2, T3>(EventHandlerType eht, Action<T1, T2, T3> callback)
    {
        AddListener(eht, (Delegate)callback);
    }
    public static void AddListener<T1, T2>(EventHandlerType eht, Action<T1, T2> callback)
    {
        AddListener(eht, (Delegate)callback);
    }
    public static void AddListener<T1>(EventHandlerType eht, Action<T1> callback)
    {
        AddListener(eht, (Delegate)callback);
    }

    public static void AddListener(EventHandlerType eht,Action callback)
    {
        AddListener(eht, (Delegate)callback);
    }

    private static void AddListener(EventHandlerType eht,Delegate callback)
    {
        List<Delegate> delList;
        if (delegateDic.TryGetValue(eht, out delList))
        {
            delList.Add(callback);
        }
        else
        {
            delegateDic.Add(eht, new List<Delegate> { callback });
        }
    }
    #endregion

    #region 移除监听
    public static void RemoveListener<T1, T2, T3, T4>(EventHandlerType eht, Action<T1, T2, T3, T4> callback)
    {
        RemoveListener(eht, (Delegate)callback);
    }
    public static void RemoveListener<T1, T2, T3>(EventHandlerType eht, Action<T1, T2, T3> callback)
    {
        RemoveListener(eht, (Delegate)callback);
    }
    public static void RemoveListener<T1, T2>(EventHandlerType eht, Action<T1, T2> callback)
    {
        RemoveListener(eht, (Delegate)callback);
    }
    public static void RemoveListener<T1>(EventHandlerType eht, Action<T1> callback)
    {
        RemoveListener(eht, (Delegate)callback);
    }
    public static void RemoveListener(EventHandlerType eht, Action callback)
    {
        RemoveListener(eht, (Delegate)callback);
    }
    private static void RemoveListener(EventHandlerType eht, Delegate callback)
    {
        List<Delegate> delList;
        if (delegateDic.TryGetValue(eht,out delList))
        {
            int _index= delList.FindIndex(d => d.Target == callback.Target && d.Method.Name == callback.Method.Name);
            if (_index >= 0)
                delList.RemoveAt(_index);
        }
    }
    #endregion


    #region 派发监听
    public static void Dispatch<T1, T2, T3, T4>(EventHandlerType eht, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        List<Delegate> methods = GetMethods(eht);
        if (methods != null)
        {
            Action<T1, T2, T3, T4> act = null;
            foreach (Delegate m in methods)
            {
                try
                {
                    act = m as Action<T1, T2, T3, T4>;
                    if (m.Target != null && act != null)
                        act(arg1, arg2, arg3, arg4);
                }
                catch (Exception e) { LogError(e); }
            }
        }
    }
    public static void Dispatch<T1, T2, T3 >(EventHandlerType eht, T1 arg1, T2 arg2, T3 arg3)
    {
        List<Delegate> methods = GetMethods(eht);
        if (methods != null)
        {
            Action<T1, T2, T3> act = null;
            foreach (Delegate m in methods)
            {
                try
                {
                     act = m as Action<T1, T2, T3>;
                    if (m.Target != null && act != null)
                        act(arg1, arg2, arg3);
                }
                catch (Exception e) { LogError(e); }
            }
        }
    }
    public static void Dispatch<T1, T2>(EventHandlerType eht, T1 arg1, T2 arg2)
    {
        List<Delegate> methods = GetMethods(eht);
        if (methods != null)
        {
            Action<T1, T2> act = null;
            foreach (Delegate m in methods)
            {
                try
                {
                    act = m as Action<T1, T2>;
                    if (m.Target != null && act != null)
                        act(arg1, arg2);
                }
                catch (Exception e) { LogError(e); }
            }
        }
    }
    public static void Dispatch<T>(EventHandlerType eht, T arg)
    {
        List<Delegate> methods = GetMethods(eht);
        if (methods != null)
        {
            Action<T> act = null;
            foreach (Delegate m in methods)
            {
                try
                {
                    act = m as Action<T>;
                    if (m.Target != null && act != null)
                        act(arg);
                }
                catch (Exception e) { LogError(e); }
            }
        }
    }
    public static void Dispatch(EventHandlerType eht)
    {
        List<Delegate> methods = GetMethods(eht);
        if (methods != null&&methods.Count>0)
        {
            Action act = null;
            foreach (Delegate m in methods)
            {
                try
                {
                    act = m as Action;
                    if (m.Target != null && act!=null)
                        act();
                }
                catch (Exception e) { LogError(e); }
            }
        }
    }
    private static List<Delegate> GetMethods(EventHandlerType eht)
    {
        List<Delegate> delList;
        if (delegateDic.TryGetValue(eht, out delList))
        {
            return delList;
        }
        return null;
    }
    private static void LogError(Exception e)
    {
        UnityEngine.Debug.LogError(e);
    }


    public static void Clear()
    {
        if (delegateDic == null || delegateDic.Count == 0) return;
        foreach (var item in delegateDic)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                item.Value[i] = null;
            }
        }
        delegateDic.Clear();
    }
    #endregion

}
