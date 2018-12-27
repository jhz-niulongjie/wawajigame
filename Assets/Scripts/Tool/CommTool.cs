using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using System.Reflection;


public static class CommTool
{
    public static GameObject FindObjForName(GameObject uiRoot, string name)
    {
        if (uiRoot.name == name)
            return uiRoot;
        Queue<GameObject> queue = new Queue<GameObject>();
        queue.Enqueue(uiRoot);
        GameObject temp = null;
        while (queue.Count > 0)
        {
            temp = queue.Dequeue();
            if (temp.name == name)
            {
                queue = null;
                return temp;
            }
            int count = temp.transform.childCount;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    queue.Enqueue(temp.transform.GetChild(i).gameObject);
                }
            }
        }
        queue = null;
        return null;
    }

    public static T GetCompentCustom<T>(GameObject uiRoot, string name)
    {
        T t = default(T);
        GameObject obj = FindObjForName(uiRoot, name);
        if (obj)
        {
            t = obj.GetComponent<T>();
        }
        return t;
    }

    public static GameObject InstantiateObj(GameObject model, GameObject parent, Vector3 pos, Vector3 scal, string name)
    {
        GameObject temp = null;
        temp = GameObject.Instantiate<GameObject>(model);
        temp.name = name;
        temp.transform.SetParent(parent.transform);
        temp.transform.localPosition = pos;
        temp.transform.localScale = scal;
        temp.transform.localRotation = Quaternion.identity;
        temp.SetActive(true);
        return temp;
    }

    public static void SaveIntData(string key)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetInt(key, 1);
        }
        else
        {
            int catCount = PlayerPrefs.GetInt(key);
            PlayerPrefs.SetInt(key, ++catCount);
        }
    }

    public static int GetSaveIntData(string key)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            return 0;
        }
        else
        {
            return PlayerPrefs.GetInt(key);
        }
    }
    public static void ClearSaveData()
    {
        PlayerPrefs.DeleteAll();
    }

    public static void SaveClass<T>(string key, T source)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StringWriter sw = new StringWriter();
        serializer.Serialize(sw, source);
        //PlayerPrefs.DeleteKey(key);
        PlayerPrefs.SetString(key, sw.ToString());
        PlayerPrefs.Save();
    }
    public static T LoadClass<T>(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringReader reader = new StringReader(PlayerPrefs.GetString(key));
            return (T)serializer.Deserialize(reader);
        }
        return default(T);
    }

    //获得时间戳
    public static string GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalMilliseconds).ToString();
    }
    //时间戳转换为时间
    public static DateTime GetTimeByStamp(string longDateTime)
    {
        //用来格式化long类型时间的,声明的变量
        long unixDate;
        DateTime start;
        DateTime date;
        //ENd

        unixDate = long.Parse(longDateTime);
        start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        date = start.AddMilliseconds(unixDate).ToLocalTime();
        return date;
    }

    /// <summary>
    /// 时间到做一些事情
    /// </summary>
    /// <param name="deltime">延时时间</param>
    /// <param name="spacetime">几秒检测一次</param>
    /// <param name="func">指定的时间要做什么</param>
    /// <param name="finish">时间到要做什么</param>
    ///  <param name="pause">暂停携程</param>
    /// <returns></returns>
    public static IEnumerator TimeFun(float deltime, float spacetime, MyFuncPerSecond func = null, Action finish = null, Func<bool> pause = null)
    {
        while (deltime >=0)
        {
            if (func != null)
            {
                if (func(ref deltime))
                    yield break;
            }
            if (deltime > 0)
            {
                yield return new WaitForSeconds(spacetime);
            }
            if (pause == null || pause != null && !pause())
                deltime -= spacetime;
        }
        if (finish != null)//
            finish();
    }


    /// <summary>
    /// 获得转换语音
    /// </summary>
    /// <param name="sign">标记 # 金额 *次数</param>
    /// <param name="voice"></param>
    /// <param name="target"></param>
    /// <returns></returns>
   public static  string TransformPayVoice(string sign, string voice, string target)
    {
        int sinx = voice.IndexOf(sign);
        if (sinx < 0) return voice;
        voice = voice.Insert(sinx, target);
        sinx = voice.IndexOf(sign);
        voice = voice.Remove(sinx, 1);
        voice = TransformPayVoice(sign, voice, target);
        return voice;
    }


    //--------------------------扩展方法--------------

    public static object GetFiledValue_S(this ScriptableObject so, string field)
    {
        return so.GetType().GetField(field).GetValue(so);
    }
    //获得枚举特性值
    public static string GetEnumContent(this Enum e)
    {
        var t_type = e.GetType();
        var fieldName = Enum.GetName(t_type, e);
        var attributes = t_type.GetField(fieldName).GetCustomAttributes(false);
        var customattri = attributes.FirstOrDefault(p => p.GetType().Equals(typeof(CustomAttri))) as CustomAttri;
        return customattri == null ? fieldName : customattri.Content;
    }
}
