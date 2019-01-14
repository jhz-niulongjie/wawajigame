using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

//获得随机概率
public sealed class RandomProbli
{
    //权重
    private static Dictionary<LuckyTurnVoiceType, float> dicWeight = new Dictionary<LuckyTurnVoiceType, float>
    {
        {LuckyTurnVoiceType.SupriseGift,2 },
        {LuckyTurnVoiceType.OnceAgain,1.96f },
        {LuckyTurnVoiceType.OnSale,1.96f },
        {LuckyTurnVoiceType.ThankYouJoin,2 },
        {LuckyTurnVoiceType.YoungWay,2 },
        {LuckyTurnVoiceType.HealthyWay,2},
        {LuckyTurnVoiceType.GiftPart,1.96f },
    };

    private static Dictionary<LuckyTurnVoiceType, float> dict = new Dictionary<LuckyTurnVoiceType, float>();

    private static Dictionary<LuckyTurnVoiceType, int> dicR = new Dictionary<LuckyTurnVoiceType, int>();


    /// <summary>
    /// 计算随机概率
    private static LuckyTurnVoiceType ComputeRandomPro()
    {
        dict.Clear();
        foreach (var item in dicWeight)
        {
            dict.Add(item.Key, UnityEngine.Random.Range(0, 100) * item.Value);
        }
        dict = dict.OrderByDescending(p => p.Value).ToDictionary(p => p.Key, o => o.Value); //降序
        return dict.ToArray()[0].Key;
    }

    /// <summary>
    /// 获得随机概率
    /// </summary>
    /// <returns></returns>
    private static LuckyTurnVoiceType ComputerPro()
    {
        dicR.Clear();
        for (int i = 0; i < 1000; i++)
        {
            var proVal = ComputeRandomPro();
            if (dicR.ContainsKey(proVal)) dicR[proVal] = dicR[proVal] + 1;
            else dicR.Add(proVal, 1);
        }
        dicR = dicR.OrderByDescending(p => p.Value).ToDictionary(p => p.Key, o => o.Value); //降序
        var key = dicR.ToArray()[0].Key;
        dicWeight[key] -= 0.1f;//出现一次就降低权重
        return key;
    }

    /// <summary>
    /// 获得随机概率
    /// </summary>
    /// <param name="success">是否抓中</param>
    /// <param name="pay">是否支付进入</param>
    /// <param name="onSale">是否有优惠券</param>
    /// <param name="partNum">礼品碎片数量</param>
    /// <returns></returns>
    public static LuckyTurnVoiceType GetRandomPro(bool success, bool pay, bool onSale, int partNum)
    {
        Debug.Log("probability----"+ GameCtr.Instance.probability);
        if (success)
            dicWeight[LuckyTurnVoiceType.SupriseGift] = 0;
        else
        {
            int number = UnityEngine.Random.Range(1, 101);
            if (number <= GameCtr.Instance.probability)
                dicWeight[LuckyTurnVoiceType.SupriseGift] = GameCtr.Instance.probability / 15.0f;  //神秘礼物权重
            else
                dicWeight[LuckyTurnVoiceType.SupriseGift] = 1.96f;
        }
        if (!pay || (success && partNum >= 2)) dicWeight[LuckyTurnVoiceType.GiftPart] = 0;//权重是0
        if (!onSale) dicWeight[LuckyTurnVoiceType.OnSale] = 0;//权重是0
        
        return ComputerPro();
    }

    /// <summary>
    /// 获得概率值
    /// </summary>
    /// <returns></returns>
    public static LuckyTurnVoiceType GetRandomPro()
    {
        int number=UnityEngine.Random.Range(1, 101);
        if (number <= GameCtr.Instance.probability)
            dicWeight[LuckyTurnVoiceType.SupriseGift] = GameCtr.Instance.probability / 15.0f;  //神秘礼物权重
        else
            dicWeight[LuckyTurnVoiceType.SupriseGift] = 1.96f;
        return ComputerPro();
    }

   

}

