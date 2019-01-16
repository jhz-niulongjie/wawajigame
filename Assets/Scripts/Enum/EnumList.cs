using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ui面板位置
/// </summary>
public enum UIShowPos
{
    Normal,
    Hide,
    TipTop,
}

/// <summary>
/// 隐藏类型
/// </summary>
public enum HidePage
{
    Hide,
    Destory
}


/// <summary>
/// 注册事件类型
/// </summary>
public enum EventHandlerType
{
    FishHookCheck,
    UpFinish,
    RestStart,
    PlayingAction,
    Success,
    ClosePage,
    HeadPress,
    TakeAway,
    QRCodeSuccess,
    GameEndStart,
    RoundStart,
    XiaoPang_incline,//小胖歪倒
    Question_Wing,
    StopCoroutine,
    //---------------------幸运转转转-------------//
    GiftPart_Fly,
    ShowGiftPart,
    GameOver,

    //-----------------sendPhone-----------------//
    RoundOver,
    TryPlayOver,
    MoviePlayOver,

}
/// <summary>
/// 游戏物体类型
/// </summary>
public enum EntityType
{
    Wheel,
    XiaoPang,
}
/// <summary>
/// 动画片段名称
/// </summary>
public enum AnimationName
{
    down,
    up,
    catchs,
    release,
}
/// <summary>
/// 支付类型
/// </summary>
public enum VoiceType
{
    Loading = 0,
    OnePay,
    TwoPay,
    ThreePay,
    FourPay,
    FivePay,
    Special,
}

/// <summary>
/// 协程类型
/// </summary>
public enum IeType
{
    Voice,//语音
    Time,//倒计时
}

public enum EffectType
{
    eff_yanhua,
    Bom_eff,
}

public enum AudioNams
{
    //抓娃娃模式
    shibai,
    shengli,
    downing2,
    shoot,
    help,
    backGround,

    //答题模式
    Loading_voice,
    error_voice,
    right_voice,
    right_voice2,

    //转盘模式
    M_1,
    M_2,
    M_3,
    M_4,
    M_5,
    M_6,
    M_7,
    M_8,
    BGMTurn,//北京
    JokeOver,//说我笑话后
    TurnContiune,//转盘持续
    TurnElseGift,//转到其他礼物
    TurnSlowDown,//减速
    TurnOpen,//开场
    TurnStop,//转盘停止
    TurnSurpriceGift,//转到什么礼品
    TurnThankyouJoin,//转到谢谢参与
    Two_pressHead,//第二次拍头 减速
}

public enum CallParameter
{
    NoPay = 0,
    HeadDown,
    HasBoy,
    NoHas,
    TakeAway,
    NoTakeAway,
    Error,
    NoBind,
    UpRecordFail,
    UpRecordListSuccess,
}

public enum CatchTy
{
    CatchErrorPos,
    NoCatch,
    Drop,
    Catch,
    HasBoy,
    GameEnd,
    GameEndGame,
    GameEndGift,
    GameOverOne,
    GameOverTwo,
    GameOverThree,
    GameOverTryPlay,
}

public enum CatchTimes
{
    PlayerCatch,
    Catch
}
//子弹 特效
public enum BullteType
{
    Bullte,
    BomEff,
}
//音频类型
public enum AudioType
{
    Fixed,
    Continuous,
    New,
    BackGround,
}

//游戏进行状态
public enum GameRunStatus
{
    QRCode,
    NoPay,
    InGame,
    GameEnd,
    Question,
}
//游戏难易程度
public enum GameLevel
{
    Nan,
    Zhong,
    Yi,
}

//答题 语音类型
public enum QuestionVoiceType
{
    Loading_Game = 0,
    Loading_Gift,
    Not_Net,
    Library_Empty,
    Rule,
    Auto_Quit,
    One_Question_Enter,
    No_operation_10,
    No_operation_30,
    Select_Right,
    Select_Error,
    The_Rest_Question_Enter,
    Answer_Success,//答题成功
    Answer_Fail,
    Get_Present,//获得礼品
    No_Present,
    Pass_Exam,
}

//不玩游戏
public enum GiveUpGame
{
    Pay_0 = 0,
    Pay_12 = 12,
    Pay_30 = 13,
    Pay_43 = 43,
    Pay_56 = 56,
    Pay_Loading,
    Pay_Out_Present,
    Not_Net,
}

//选择游戏的模式
public enum SelectGameMode
{
    Pay = 0,
    Question,
    Game,
    NoGame,
}

public enum Net_Type
{
    None,
    Net_No,
    Net_Has,
    Net_Checking,
}

public enum Answer_Question
{
    Welcome,
    Rule,//规则页
    HeadDown,//头部拍下
    Answering,//答题中
    Answered,//已答题
    PlayAnswerVoice,//答题语音播放
    AnswerFinish,//答题完成
    PlayFinishVoice,//播放完成语音
}

//android端 方法
public enum AndroidMethod
{
    [CustomAttri("-获得游戏模式数据-")]
    GetGameModeData,
    [CustomAttri("-能否开始游戏-")]
    isCanPlay,
    [CustomAttri("-获得抓中概率值-")]
    GetProbabilityValue,
    [CustomAttri("-查询是否支付-")]
    GetPayStatus,
    [CustomAttri("-查询是否支付-sendPhone")]
    GetPayStatusSendPhone,
    [CustomAttri("-请求二维码-")]
    GetDrawQrCode,
    [CustomAttri("-向服务器传输记录抓取记录-")]
    SendCatchRecord,
    [CustomAttri("-向服务器传输记录抓取记录-")]
    Q_UpRecord,
    [CustomAttri("-批量上传记录-")]
    SendCatchRecordList,
    [CustomAttri("-播放语音-")]
    SpeakWords,
    [CustomAttri("-摆动翅膀-")]
    ShakeWave,
    [CustomAttri("-摆动翅膀闪灯带-")]
    ShakeWaveLight,
    [CustomAttri("-自定义退出-")]
    CustomQuit,
    [CustomAttri("-灯光闪烁啊-")]
    OpenLight,
    [CustomAttri("-自动送礼物-")]
    AutoPresent,
    [CustomAttri("-获得考题-")]
    GetQuestionAnswer,
    [CustomAttri("-获得支付页面语音-")]
    GetPayPageVoice,
    [CustomAttri("-开始结束答题-")]
    AnswerStartOrEnd,
    [CustomAttri("-Hide Splash-")]
    HideSplash,
    [CustomAttri("-选择要进入的游戏-")]
    SelectGame,
    [CustomAttri("-获得优惠券数据-")]
    GetOnSaleNumberData,
    [CustomAttri("-更新优惠券状态-")]
    UpdateOnSaleValue,
    [CustomAttri("-请求兑换码平板电脑-")]
    ResPhoneCode,
}

public enum LuckyTurnVoiceType
{
    Loading = -1,
    CodePage,
    LastTimes,
    MiddleTimes,
    OneTimes,
    [CustomAttri("再来一次")]
    OnceAgain,
    [CustomAttri("谢谢参与")]
    ThankYouJoin,
    [CustomAttri("年轻秘诀")]
    YoungWay,
    [CustomAttri("健康秘诀")]
    HealthyWay,
    [CustomAttri("礼品碎片")]
    GiftPart,
    [CustomAttri("神秘礼物")]
    SupriseGift,
    [CustomAttri("优惠券")]
    OnSale,
}

//旋转状态
public enum TurnStatus
{
    Run,
    SlowDown,
    NotOperation,
    HeadDown,
}
//游戏种类
public enum GameKind
{
    LuckyBoy,
    LuckyTurn,
    LuckyBigBom,
}

//资源文件路径
public enum AssetFolder
{
    Common,
    LuckyBoy,
    LuckyTurn,
    LuckyBigBom,
    LuckySendPhone,
}
//礼品碎片表 字段
public enum GiftPartTable
{
    OpenId,
    PartNum,
    RegTime,
}


//旋转蓄力
public enum RorateForce
{
   None,
   Rorate,
   Force,
}
//Tag 标记
public enum TagType
{
    None,
    Soldier,
    Wall,
    Tower,
    Floor,
}

public enum BigBomVoiceType
{
    Loading=-1,
    TryPlay,
    OneLevel,
    TwoLevel,
    ThreeLevel,
}

public enum BigBomOperType
{
    _Loading=0,//进入
    _NoOper=1,//无操作
    _PressHead=2,//按住头部
    _ShootTarget=3,//击中目标
    _NoShoot=4,//未击中
    _ShootWall=5,//击中城墙
    _ShootGameQuit=6, //击中目标 游戏退出
}

//送平板状态类型
public enum SendPhoneStatusType
{
    TryPlay = 0,
    Code,
    Common,
    OnePayEnter,
    TwoPayEnter,
    ThreePayEnter,
}

//送平板操作类型
public enum SendPhoneOperateType
{
    TryPlayEnter = 0,
    Code,
    NoOperateEnter,
    NoOperate,
    Catch,
    Drop,
    NoCatch,
    RoundEnter,
    GameEnd,
    GameOver,
}


