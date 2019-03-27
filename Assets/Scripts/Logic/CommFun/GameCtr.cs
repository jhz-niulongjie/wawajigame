using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using LitJson;
using DG.Tweening;

public delegate bool MyFuncPerSecond(ref float time);
public class GameCtr : MonoBehaviour
{
    #region 设置私有
    public static GameCtr Instance { get; private set; }
    //库对象
    public HandleSqliteData handleSqlite { get; private set; }
    //头部按下事件
    public Action headDown_Action { get; private set; }
    //是否一直显示二维码
    public bool isNoDied { get; private set; }
    private bool isUnBind = false;

    #endregion

    #region 公共对象
    public RawImage raw { get; set; }
    //开始抓时间
    public string startCarwTime { get; set; }
    //开始抓时间
    public string Q_startCarwTime { get; set; }
    //曲线值
    public float randomQuXian { get; set; }
    //是中的小胖
    public GameEntity gameXP { get; set; }
    //当前玩家游戏状态
    public GameStatus gameStatus { get; set; }
    //游戏试玩状态
    public int gameTryStatus { get; set; }
    #endregion

    #region 赋值受保护
    //是否获得二维码
    public bool isGetCode { get; protected set; }
    //是否支付成功
    public bool isPaySucess { get; protected set; }
    //抓中不被打掉概率
    public float probability { get; protected set; }
    //金额
    public float money { get; protected set; }
    //第几次支付
    public int pay { get; protected set; }
    //openId
    public string openId { get; protected set; }
    //账单号
    public string orderNumber { get; protected set; }
    //游戏模式
    public GameMode gameMode { get; protected set; }

    //一百次能最多抓中几次
    public float winningTimes { get; protected set; }
    //概率基数
    public float carwBasicCount { get; protected set; }
    //玩家选择的游戏
    public int selectGame { get; protected set; }//0 抓娃娃 1转转
    //是否支付答题模式
    public SelectGameMode selectMode { get; protected set; }//选择模式  0是支付模式  1是答题模式
    //选择局数
    public int selectRound { get; protected set; }
    //是否进行游戏
    public bool isGame { get; protected set; }//是否进行游戏 0是进行 1不进行
    //问题数目
    public int question { get; protected set; }
    //通过数目
    public int pass { get; protected set; }
    //是否自动送礼品
    public bool autoSendGift { get; protected set; }//第三次支付是否自动送礼品 1是进行 0不进行

    public Texture2D texture { get; protected set; }//结束显示图片

    public string OverVoice { get; protected set; }//结束显示图片时的语音

    public float ShowTime { get; protected set; }//结束显示图片时间

    public bool isFirstGame { get; protected set; }//是否第一次进入游戏
    #endregion

    #region 测试数据参数
    public float checkProperty { get; set; }
    #endregion

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        AndroidCallUnity.Instance.Init(HeadDonw, AndroidCall, QRCodeCall, GetProbabilityCall, PaySuccess, Question_Wing, GameOverImageInfo);
        Init();
        EnterGame();
    }

    protected virtual void Init()
    {
        randomQuXian = 1;
        checkProperty = 0.42f;//0.42f  //检测范围
        probability = 20;//百分之30不打掉
        carwBasicCount = 100;
        winningTimes = 6;//抓中百分六、
        money = 10;
        selectGame = 0;
        selectMode = SelectGameMode.Pay;
        selectRound = 3;
        question = 5;
        pass = 3;
        isGetCode = false;
        isPaySucess = false;
        isGame = true;
        autoSendGift = true;
        isNoDied = false;
        ShowTime = 0;
        isFirstGame = true;
        handleSqlite = new HandleSqliteData(this);
        //Android_Call.UnityCallAndroid(AndroidMethod.GetProbabilityValue);
        NetMrg.Instance.SendRequest(AndroidMethod.GetProbabilityValue);
    }
    //获得游戏模式
    protected virtual void EnterGame()
    {
        string _mode = Android_Call.UnityCallAndroidHasReturn<string>(AndroidMethod.GetGameModeData);
        if (!string.IsNullOrEmpty(_mode))
        {
            string[] contents = _mode.Split('|');
            Debug.Log("---游戏设置选项------" + _mode);
            if (contents.Length < 7)
            {
                Debug.LogError("请求模式数量不符");
                return;
            }
            selectMode = (SelectGameMode)Convert.ToInt32(contents[0]); //选择模式  0是支付模式  1是答题模式
            selectRound = Convert.ToInt32(contents[1]);//选择局数
            question = Convert.ToInt32(contents[2]);//几道题
            pass = Convert.ToInt32(contents[3]);//通过数量
            isGame = Convert.ToInt32(contents[4]) == 0 ? true : false;//是否进行游戏 0是进行 1不进行
            selectGame = Convert.ToInt32(contents[5]);
            autoSendGift = Convert.ToInt32(contents[6]) == 0 ? true : false; //开启礼品模式 为0 关闭 为1
            isNoDied = Convert.ToInt32(contents[7]) == 0 ? true : false;//开启一直显示二维码模式  
        }
        else
            Q_AppQuit();
    }

    //头部按下
    public void HeadDonw()
    {
        Debug.Log("头部按下");
        if (headDown_Action != null)
        {
            headDown_Action();
            headDown_Action = null;
        }
    }


    //Android 调用
    public virtual void AndroidCall(CallParameter cp)
    {
        switch (cp)
        {
            case CallParameter.NoPay:
                Debug.Log("未支付");
                gameMode.NoPay();
                break;
            case CallParameter.Error:
                UIManager.Instance.ShowUI(UIMessagePage.NAME, true, "网络异常，连不上服务器，2s后游戏自动关闭");
                break;
            case CallParameter.NoBind:
                if (isUnBind) return;
                isUnBind = true;
                Debug.Log("娃娃机未绑定");
                UIManager.Instance.ShowUI(UIMessagePage.NAME, true,
                    "礼品机器人还未绑定手机，请您关注公众号并绑定礼品机器人后才能进入游戏");
                break;
            case CallParameter.UpRecordFail:
                Debug.Log("上报抓取记录失败");
                handleSqlite.InsertData();
                break;
            case CallParameter.UpRecordListSuccess:
                Debug.Log("批量上报抓取记录成功");
                handleSqlite.DeleteData(HandleSqliteData.recordTable);
                break;
        }
    }
    //二维码获得成功
    protected virtual void QRCodeCall(JsonData result)
    {
        if (isGetCode) return;
        isGetCode = true;
        QRCode.ShowCode(raw, result["qrUrl"].ToString());
        orderNumber = result["orderNo"].ToString();
        gameStatus.SetOrderNoRobotId(orderNumber, NetMrg.Instance.robotId);
        gameStatus.SetRunStatus(GameRunStatus.NoPay);
        EventHandler.ExcuteEvent(EventHandlerType.QRCodeSuccess, null);
        JsonData jsondata = new JsonData();
        jsondata["orderNo"] = orderNumber;
        StartCoroutine(CommTool.TimeFun(2, 2, (ref float t) =>
        {
            if (!isPaySucess)
            {
                //检测是否支付
                // Android_Call.UnityCallAndroidHasParameter<string, bool>(AndroidMethod.GetPayStatus, orderNumber, false);
                NetMrg.Instance.SendRequest(AndroidMethod.GetPayStatus, jsondata);
            }
            if (t == 0) t = 2;
            return isPaySucess;
        }));
    }
    //获得概率值
    protected virtual void GetProbabilityCall(JsonData result)
    {
        //概率值
        probability = Convert.ToSingle(result["captureProb"].ToString());
        //中奖值
        winningTimes = Convert.ToSingle(result["captureStepNum"].ToString());
        //基数
        carwBasicCount = Convert.ToInt32(result["captureStepLenght"].ToString());
        money = Convert.ToSingle(result["qrAmount"].ToString());
        Debug.Log("概率值获得成功------- probability： " + probability + "  winningTimes： " + winningTimes + "  carwBasicCount:" + carwBasicCount + "  money:" + money);
    }

    //支付成功
    protected virtual void PaySuccess(JsonData result)
    {
        isPaySucess = true;
        pay = Convert.ToInt32(result["winningLevel"].ToString());
        openId = result["openId"].ToString();
        gameStatus.SetOpenId(openId);
        Debug.Log("支付成功--openId::" + openId);
    }

    //摆动翅膀回答问题
    protected virtual void Question_Wing(string result)
    {
        //0 是左翅膀 1是右翅膀
        EventDispatcher.Dispatch<string>(EventHandlerType.Question_Wing, result);//注册
    }
    //结束页面图片信息
    protected virtual void GameOverImageInfo(string result)
    {
        string[] contents = result.Split('|');
        OverVoice = contents[0];
        ShowTime = Convert.ToSingle(contents[1]);
        StartCoroutine(LoadImage(contents[2]));
    }

    //注册头部按下事件
    public void RegHeadAction(Action action)
    {
        Debug.Log("注册头部事件");
        if (headDown_Action != null)
            headDown_Action = null;
        headDown_Action = action;
    }
    //转换为子类
    public T ChangeType<T>() where T : GameCtr
    {
        if (Instance is T)
            return (T)Instance;
        return null;
    }

    //从android加载图片
    IEnumerator LoadImage(string imgName)
    {
        string path = Application.persistentDataPath + "/Images/" + imgName;
        Debug.Log("图片路径。。。" + path);
        WWW www = new WWW(path);
        yield return www;
        if (www != null && !string.IsNullOrEmpty(www.error))
        {
            texture = www.texture;
        }
        else
        {
            Debug.LogError("www 加载图片失败:::" + www.error);
        }
    }

    //重置数据
    public void ResetGame()
    {
        AndroidCallUnity.Instance.RestData();
        NetMrg.Instance.SendRequest(AndroidMethod.GetProbabilityValue);
        isGetCode = false;
        isPaySucess = false;
        isFirstGame = false;
    }
    //显示结束图片
    private void ShowOverImage()
    {
        if (!string.IsNullOrEmpty(OverVoice) && texture != null)
        {
            Debug.Log("******显示结束图片***********");
            UIManager.Instance.ShowUI(UIGameOverImagePage.NAME, true);
            Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, OverVoice);
        }
    }

    private void Dispose()
    {
        headDown_Action = null;
        if (gameStatus != null) gameStatus.SetProDefulat();
        if (gameMode != null) gameMode.Clear();
        Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, false);
        UIAtlasManager.Instance.Clear();
        AudioManager.Instance.Clear();
        EventHandler.Clear();
        EventDispatcher.Clear();
        EffectMrg.HideEffectNoraml();
        Resources.UnloadUnusedAssets();
        GC.Collect();
        string dlgName = null;
        if (isNoDied && selectMode == SelectGameMode.Pay)//游戏不死 并且是支付模式
        {
            ResetGame();
            EnterGame();
            if (selectGame == 0)
                dlgName = UIMovieQRCodePage.NAME;
            else if (selectGame == 1)
                dlgName = UITurnCodePage.NAME;
            UIManager.Instance.ClearExcludeCodePage(dlgName);
            UIManager.Instance.ShowUI(dlgName, true);
        }
        else
        {
            AndroidCallUnity.Instance.Dispose();
            UIManager.Instance.Clear();
            Debug.Log("退出游戏AppQuit");
            Application.Quit();
        }
    }
    /// <summary>
    /// 游戏推出
    /// </summary>
    public virtual void AppQuit()
    {
        ShowOverImage();
        DOVirtual.DelayedCall(ShowTime, Dispose);
    }
    /// <summary>
    /// 游戏推出  答问不及格
    /// </summary>
    public virtual void Q_AppQuit()
    {
        headDown_Action = null;
        if (gameMode != null) gameMode.Clear();
        AndroidCallUnity.Instance.Dispose();
        Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, false);
        UIManager.Instance.Clear();
        EventDispatcher.Clear();
        Resources.UnloadUnusedAssets();
        GC.Collect();
        Debug.Log("退出游戏Q_AppQuit");
        Application.Quit();
    }

}
