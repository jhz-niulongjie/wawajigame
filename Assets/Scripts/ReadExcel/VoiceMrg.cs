using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class VoiceMrg<S,V> where S:ScriptableObject where V: ExtendContent
{
    private static Dictionary<string, List<V>> dicKV;
    /// <summary>
    /// 获得语音数据
    /// </summary>
    public static void InitVoiceData(string voiceTableName)
    {
        if (VoiceMrg<S, V>.dicKV == null || VoiceMrg<S, V>.dicKV.Count == 0)
        {
            S s = Resources.Load(voiceTableName) as S;
            List<V> list = s.GetFiledValue_S("voiceTypes") as List<V>;
            VoiceMrg<S, V>.dicKV = list.GroupBy(c => c.PayType).ToDictionary(x => x.First().PayType,
                 y => y.ToList().OrderBy(v => Convert.ToInt32(v.Time)).ToList());
            Debug.Log("语音数据获得成功");
            s = null;
        }
    }
    /// <summary>
    /// 获得语音数据
    /// </summary>
    /// <param name="type"></param>
    /// <param name="round"></param>
    /// <returns></returns>
    public static List<V> GetRoundVoice(VoiceType type, int round)
    {
        List<V> vlist = GetVoiceForType(type);
        if (vlist != null)
        {
            if (type == VoiceType.ThreePay)
                return vlist;
            else
                return vlist.FindAll(e => e.Type == round.ToString());
        }
        return null;
    }


    /// <summary>
    /// 根据类型获得语音数据
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static List<V> GetVoiceForType(VoiceType type)
    {
        if (VoiceMrg<S,V>.dicKV.ContainsKey(((int)type).ToString()))
        {
            return VoiceMrg<S,V>.dicKV[((int)type).ToString()];
        }
        return null;
    }

    public static S GetVoiceFromAsset(string _assetName)
    {
        S qs = Resources.Load(_assetName) as S;
        return qs;
    }
    public static void Clear()
    {
        if (VoiceMrg<S, V>.dicKV != null)
        {
            VoiceMrg<S, V>.dicKV.Clear();
            VoiceMrg<S, V>.dicKV = null;
        }
    }
}
