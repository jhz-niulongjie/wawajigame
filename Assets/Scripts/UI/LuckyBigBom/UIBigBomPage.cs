using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
using System.Linq;

public sealed class UIBigBomPage : UIViewBase
{
    public const string NAME = "UIBigBomPage";
    public override UIShowPos _showPos { get { return UIShowPos.Normal; } }
    public override HidePage _hidePage { get { return HidePage.Destory; } }
    public override AssetFolder _assetFolder { get { return AssetFolder.LuckyBigBom; } }

    private Transform cannon;
    private Transform bullet;
    private Rigidbody2D bullet_rigid2d;
    private Slider forceSlider;
    private Slider rotateSlider;
    private GameObject try_play;
    private GameObject level_num;
    private GameObject level_1;
    private GameObject level_2;
    private GameObject level_3;
    private GameObject level_bg;
    private Image level_img_1;
    private Image level_img_2;
    private Image level_img_3;
    private Text timeTex;

    private GameObject level_show;
    private Text level_text;
    private Text level_task;

    #region 抛物线 参数    https://blog.csdn.net/caojianhua1993/article/details/52278787
    private float power = 0;//   15最大值 默认 这个代表发射时的速度/力度等，可以通过此来模拟不同的力大小
    private float shoot_angle = 0;// 45 默认  发射的角度，这个就不用解释了吧
    private float gravity = -15; // -10 默认 这个代表重力加速度

    private Vector3 moveSpeed;//初速度方向
    private Vector3 gritySpeed = Vector3.zero;//重力的速度向量 t时为0
    private float dtiem;//已过去时间
    private Vector3 currentAngle;//当前角度
    #endregion

    private bool isTryPlay = false;//是否试玩
    private bool isShoot = false;//是否发射
    private bool isPress = false; //是否按下
    private bool pauseIe = true; //是否暂停
    private bool isSuccess = false;// 这关是否成功
    private RorateForce curRorateForce = RorateForce.Force;//处于 那种模式
    private float basicRotate = 10f; //大炮旋转 基数
    private float basicForce = 10f; //大炮蓄力 基数
    private int currentLevel = 1;
    private int timeIndex = 0;
    private LuckyTurnVoiceType ltv;//是否获得礼物
    private readonly Color colorBlack = new Color(87 / 255.0f, 87 / 255.0f, 87 / 255.0f);
    private readonly Color colorActive = new Color(1, 1, 1);
    private Dictionary<BigBomVoiceType, Dictionary<BigBomOperType, List<VoiceContent>>> bigbomOperDic;


    protected override void OnInit()
    {
        base.OnInit();
        cannon = CommTool.FindObjForName(gameObject, "_cannon").transform;
        bullet = CommTool.FindObjForName(cannon.gameObject, "_bullet").transform;
        forceSlider = CommTool.GetCompentCustom<Slider>(gameObject, "forceSlider");
        rotateSlider = CommTool.GetCompentCustom<Slider>(gameObject, "rotateSlider");
        try_play = CommTool.FindObjForName(gameObject, "try_play");
        level_num = CommTool.FindObjForName(gameObject, "level_num");
        level_1 = CommTool.FindObjForName(gameObject, "level_1");
        level_2 = CommTool.FindObjForName(gameObject, "level_2");
        level_3 = CommTool.FindObjForName(gameObject, "level_3");
        level_bg = CommTool.FindObjForName(gameObject, "level_bg");
        level_img_1 = CommTool.GetCompentCustom<Image>(level_num, "_1");
        level_img_2 = CommTool.GetCompentCustom<Image>(level_num, "_2");
        level_img_3 = CommTool.GetCompentCustom<Image>(level_num, "_3");
        timeTex = CommTool.GetCompentCustom<Text>(gameObject, "time");

        level_show = CommTool.FindObjForName(gameObject, "level_show");
        level_text = CommTool.GetCompentCustom<Text>(level_show, "level_text");
        level_task = CommTool.GetCompentCustom<Text>(gameObject, "level_task");
        EventHandler.RegisterEvnet(EventHandlerType.GameEndStart, StartNextLevel);
        EventHandler.RegisterEvnet(EventHandlerType.Success, ShowResultTable);
        if (GameCtr.test)
        {
            GameObject _press = CommTool.FindObjForName(gameObject, "press");
            UIEventLisener.Get(_press).OnPress += HeadDown;
            UIEventLisener.Get(_press).OnUp += HeadUp;
        }
    }

    protected override void OnCreate()
    {
        LoadVoice();
    }

    public override void OnEnter()
    {
        base.OnEnter();
        isTryPlay = (bool)_Data;
        currentLevel = isTryPlay ? 0 : 1; //试玩模式 0
        ShowLevel();
        StartTimeCount();
    }

    public override void OnExit()
    {
        base.OnExit();
        bigbomOperDic.Clear();
    }

    //开始计时
    void StartTimeCount()
    {
        float tempTime = 0;
        StartCoroutine(CommTool.TimeFun(30, 1f, (ref float t) =>
        {
            if (!pauseIe)
            {
                tempTime++;
                timeTex.text = t.ToString();//没暂停才复制
                Speak(BigBomOperType._NoOper, tempTime);
                if (t == 0) //30秒内 发射
                {
                    t = 31;
                    tempTime = 0;
                    curRorateForce = RorateForce.Force;
                    HeadUp(null);
                }
            }
            else //暂停
            {
                t = 31;
                tempTime = 0;
            }
            return false;
        },
        null,
       () => pauseIe));
    }


    void Update()
    {
        if (isPress)
        {
            //if (curRorateForce == RorateForce.Rorate)
            //    StartRotate();
            //else if (curRorateForce == RorateForce.Force)
            //    StartForce();

            if (curRorateForce == RorateForce.Force)
            {
                StartRotate();
                StartForce();
            }
        }
    }


    void FixedUpdate()
    {
        if (!isShoot) return;
        //构建抛物线
        dtiem += Time.fixedDeltaTime;
        //计算物体重力加速度
        gritySpeed.y = gravity * dtiem;

        bullet.position += (moveSpeed + gritySpeed) * Time.fixedDeltaTime * 2;//移动速度
        currentAngle.z = Mathf.Atan((moveSpeed.y + gritySpeed.y) / moveSpeed.x) * Mathf.Rad2Deg;
        bullet.eulerAngles = currentAngle;
        if (bullet.localPosition.x >= 1100 && bullet.gameObject.activeSelf)
        {
            ShowResultTable(null);
        }
    }

    //加载语音
    void LoadVoice()
    {
        LuckyTurn bigbom = VoiceMrg<LuckyTurn, ExtendContent>.GetVoiceFromAsset("bigbom");
        bigbomOperDic = bigbom.luckyTurnList.GroupBy(v => (BigBomVoiceType)Convert.ToInt32(v.Id)).
              ToDictionary(g => g.Key, g => g.GroupBy(v => (BigBomOperType)Convert.ToInt32(v.Type)).ToDictionary(n => n.Key, n => n.ToList()));
    }

    //显示关卡
    void ShowLevel()
    {
        Speak(BigBomOperType._Loading);
        if (!isTryPlay) //普通模式
        {
            try_play.SetActive(false);
            level_1.SetActive(false);
            level_2.SetActive(false);
            level_3.SetActive(false);
            level_bg.SetActive(true);
            level_num.SetActive(true);
            level_show.SetActive(true);
            level_img_1.color = colorBlack;
            level_img_2.color = colorBlack;
            level_img_3.color = colorBlack;
            if (currentLevel == 1)
            {
                level_1.SetActive(true);
                level_img_1.color = colorActive;
                level_text.text = "第一关";
                level_task.text = "任务：击败守城士兵";
            }
            else if (currentLevel == 2)
            {
                level_2.SetActive(true);
                level_img_2.color = colorActive;
                level_text.text = "第二关";
                level_task.text = "任务：击败守卫金库的士兵";
            }
            else
            {
                level_3.SetActive(true);
                level_img_3.color = colorActive;
                level_text.text = "第三关";
                level_task.text = "任务：炸掉金库";
            }
        }
        else  //试玩模式
        {
            try_play.SetActive(true);
            level_num.SetActive(false);
            level_show.SetActive(true);
            level_1.SetActive(true);
            level_text.text = "试玩模式";
            level_task.text = "任务：击败守城士兵";
        }
        level_show.transform.DOLocalMoveY(0, 1.5f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            DOVirtual.DelayedCall(2, () =>
             {
                 level_bg.SetActive(false);
                 level_show.SetActive(false);
                 level_show.transform.localPosition = new Vector3(0, 512, 0);
                 pauseIe = false;
                 timeIndex = 0;
             });
        });

    }

    //开始下一局
    void StartNextLevel(object data)
    {
        if (isTryPlay || !isSuccess)//试玩 没过关直接退出
            GameCtr.Instance.AppQuit();
        else
        {
            currentLevel++;
            if (currentLevel <= 3)
            {
                SetBulletDefault();
                ShowLevel();
            }
            else  //游戏结束
            {
                if (ltv == LuckyTurnVoiceType.SupriseGift)
                {
                    Speak(BigBomOperType._ShootGameQuit);
                    DOVirtual.DelayedCall(3, GameCtr.Instance.AppQuit);
                }
                else
                    GameCtr.Instance.AppQuit();
            }
        }
    }

    //显示结果面板
    void ShowResultTable(object data)
    {
        isShoot = false;
        isSuccess = false;
        BigBomOperType bigtype;
        if (data != null)
        {
            object[] dt = data as object[];
            isSuccess = (bool)dt[0];
            TagType tag = (TagType)dt[1];
            if (isSuccess)
                bigtype = BigBomOperType._ShootTarget;
            else
            {
                if (tag == TagType.Wall)
                    bigtype = BigBomOperType._ShootWall;
                else
                    bigtype = BigBomOperType._NoShoot;
            }
        }
        else
        {
            bigtype = BigBomOperType._NoShoot;
        }
        ltv = currentLevel == 3 ? RandomProbli.GetRandomPro() : LuckyTurnVoiceType.ThankYouJoin;
        Speak(bigtype);
        object[] dts = { currentLevel, isSuccess, isTryPlay, ltv };
        UIMgr.Instance.ShowUI(UIBigBomResultPage.NAME, true, dts);
    }

    //设置子弹默认值
    void SetBulletDefault()
    {
        dtiem = 0;
        power = 0;
        bullet.gameObject.SetActive(false);
        bullet.transform.localPosition = Vector3.zero;
        bullet.transform.localRotation = Quaternion.Euler(Vector3.zero);
        currentAngle = Vector3.zero;
    }

    //开始旋转
    void StartRotate()
    {
        if (shoot_angle >= 45 || shoot_angle < 0)
        {
            basicRotate *= -1;
        }
        cannon.transform.Rotate(Vector3.forward, basicRotate * Time.deltaTime);
        shoot_angle = cannon.transform.eulerAngles.z;
        rotateSlider.value = shoot_angle / 45;
    }

    //开始蓄力
    void StartForce()
    {
        if (power >= 25 || power < 0)
        {
            basicForce *= -1;
        }
        power += (basicForce * Time.deltaTime);
        forceSlider.value = power / 25.0f;
    }
    //大炮发射
    void CannonShoot()
    {
        moveSpeed = Quaternion.Euler(new Vector3(0, 0, shoot_angle)) * Vector3.right * power;
        currentAngle = Vector3.zero;
        bullet.gameObject.SetActive(true);
        isShoot = true;
    }

    //说话
    void Speak(BigBomOperType oper, float _time = 0)
    {
        if (bigbomOperDic.ContainsKey((BigBomVoiceType)currentLevel))
        {
            if (bigbomOperDic[(BigBomVoiceType)currentLevel].ContainsKey(oper))
            {
                var templist = (bigbomOperDic[(BigBomVoiceType)currentLevel][oper]).OrderBy(v => Convert.ToInt32(v.Time)).ToList();
                if (oper == BigBomOperType._NoOper)//无操作
                {
                    if (timeIndex < templist.Count && Convert.ToSingle(templist[timeIndex].Time) == _time)
                    {
                        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, templist[timeIndex].Content);
                        timeIndex++;
                    }
                }
                else
                {
                    int _index = UnityEngine.Random.Range(0, templist.Count);
                    Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, templist[_index].Content);
                }
            }
        }
    }


    void HeadDown(GameObject go)
    {
        isPress = true;
        pauseIe = true;//暂停
        Speak(BigBomOperType._PressHead);
        if (curRorateForce == RorateForce.None)
            curRorateForce = RorateForce.Rorate;
        else if (curRorateForce == RorateForce.Rorate)
            curRorateForce = RorateForce.Force;
    }

    void HeadUp(GameObject go)
    {
        isPress = false;
        if (curRorateForce == RorateForce.Force)
        {
            curRorateForce = RorateForce.None;
            CannonShoot();
        }
    }
}
