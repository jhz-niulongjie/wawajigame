using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class AppConst   {

    public const bool test = false;

    //状态健值
    public const string statusKey = "GameLocalData";
    //特效层
    public const string layer_Light_effect = "Light_Effect";
    //水层
    public const string layer_Water = "Water";
    //语音时间
    public const float speakTime = 0.265f;

    //原测试网址
    //private static  final String Test_IP="http://39.106.250.170:8083/api/v2/interface/doll/";
    //原正试网址
    //private static  final  String IP="http://backend.efrobot.com/api/v2/interface/doll/";
    //新测试地址
    public const string Test_IP="http://net.wxservice.efrobot.com/v1/";
    //新正式地址
    public const string IP ="http://doll.game.efrobot.com/v1/";
    //获得支付状态
    public const string LuckPayStatus ="wxluckDraw/getLuckDrawStatus";
    //获得二维码
    public const string LuckDrawQrCode = "wxluckDraw/getLuckDrawQrCode";
    //获得抓中概率值
    public const string LuckCatchProbability ="intranet_grab/queryGrabProbability";
    //上报抓取记录
    public const string LuckCatchRecord ="intranet_grab/insertGrabRecord";
    //批量上报抓取记录
    public const string LuckCatchRecordList ="intranet_grab/insertBatchGrabRecord";

    //送平板ip
    public const string Phone_IP ="http://doll.game.efrobot.com/";
    public const string TestPhone_IP ="http://net.wxservice.efrobot.com/";

    //兑换码
    public const string TestPhone_GetPhoneCode ="win-record/get-redeem-code";
    // 支付状态
    public const string TestPhone_PayStatus ="v1/wxluckDraw/getLuckDrawStatusNew";













}
