using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Events;

public sealed class EventHandler
{
    private static Dictionary<EventHandlerType, Action<object>> dic = new Dictionary<EventHandlerType, Action<object>>();

    public static void RegisterEvnet(EventHandlerType type, Action<object> action)
    {
        Action<object> act = GetRegAction(type);
        act += action;
        dic[type] = act;
    }
    public static void UnRegisterEvent(EventHandlerType type, Action<object> action)
    {
        Action<object> act = GetRegAction(type);
        if (act != null)
        {
            act -= action;
            dic[type] = act;
        }
    }

    public static void ExcuteEvent(EventHandlerType type, object obj)
    {
        Action<object> act = GetRegAction(type);
        if (act != null)
            act(obj);
    }

    private static Action<object> GetRegAction(EventHandlerType type)
    {
        Action<object> act = null;
        dic.TryGetValue(type, out act);
        return act;
    }

    public static void Clear()
    {
        List<Action<object>> temp = dic.Values.ToList();
        for (int i = 0; i < temp.Count; i++)
        {
            temp[i] = null;
        }
        dic.Clear();
    }


}
