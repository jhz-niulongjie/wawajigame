using UnityEngine;
using System.Collections;

public class UnityPing : MonoBehaviour
{
    private static string s_ip = "";
    private static System.Action s_finish = null;

    private static System.Action  s_fail = null;

    private static UnityPing s_unityPing = null;

    private static int s_timeout = 2;

    public static void CreatePing(string ip,int _timeout, System.Action  _finish,System.Action _fail)
    {
        if (string.IsNullOrEmpty(ip)) return;
        if (_finish == null) return;
        if (s_unityPing != null) return;

        s_ip = ip;
        s_timeout = _timeout;
        s_finish = _finish;
        s_fail = _fail;

        GameObject go = new GameObject("UnityPing");
        DontDestroyOnLoad(go);
        s_unityPing = go.AddComponent<UnityPing>();
    }

    private void Start()
    {
        switch (Application.internetReachability)
        {
            case NetworkReachability.ReachableViaCarrierDataNetwork: // 3G/4G
            case NetworkReachability.ReachableViaLocalAreaNetwork: // WIFI
                {
                    StopCoroutine(this.PingConnect());
                    StartCoroutine(this.PingConnect());
                }
                break;
            case NetworkReachability.NotReachable: // 网络不可用
            default:
                {
                    if (s_fail != null)
                    {
                        s_fail();
                        Destroy(this.gameObject);
                    }
                }
                break;
        }
    }

    private void OnDestroy()
    {
        s_ip = "";
        s_timeout = 20;
        s_fail = null;
        s_finish = null;

        if (s_unityPing != null)
        {
            s_unityPing = null;
        }
    }

    IEnumerator PingConnect()
    {
        // Ping網站 
        Ping ping = new Ping(s_ip);

        int addTime = 0;
        int requestCount = s_timeout * 10; // 0.1秒 请求 1 次，所以请求次数是 n秒 x 10

        // 等待请求返回
        while (!ping.isDone)
        {
            yield return new WaitForSeconds(0.1f);

            // 链接失败
            if (addTime > requestCount)
            {
                addTime = 0;
                Debug.Log("网络ping失败："+ping.time);
                if (s_fail != null)
                {    
                    s_fail();
                    Destroy(this.gameObject);
                }
                yield break;
            }
            addTime++;
        }

        // 链接成功
        if (ping.isDone)
        {
            Debug.Log("网络ping成功：" + ping.time);
            if (s_finish != null)
            {
                s_finish();
                Destroy(this.gameObject);
            }
            yield return null;
        }
    }
}