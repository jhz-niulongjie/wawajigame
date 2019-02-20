using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;
using System.Linq;

public sealed class TransferData
{
    public LuckyTurnVoiceType uiType;
    public List<VoiceContent> voice;
    public List<VoiceContent> joke;
    public Action contiunAction;
    public Vector3 pos;
}

public sealed class UITurnTablePage : UIDataBase
{
    public const string NAME = "UITurnTablePage";
    public override UIShowPos ShowPos
    {
        get { return UIShowPos.Normal; }
    }

    public override HidePage hidePage
    {
        get { return HidePage.Destory; }
    }
    public override AssetFolder assetFolder
    {
        get { return AssetFolder.LuckyTurn; }
    }
    private GameObject owngiftPart;
    private Transform arrow;
    private Transform arrow_up;
    private Transform gift_part;
    private Transform item_parent;
    private Transform logo;
    private Text time_count;
    private Text still_times;
    private Image pat_head;
    private CanvasGroup canvasGrp;
    private Tweener pat_head_tweener;
    private Image bg;
    private Sprite[] bgSprites;

    //特效数据
    private Transform mask;
    private GameObject mask_effect;
    private GameObject mask_bg;
    private Transform camera_effect;
    private IEnumerator ie_effect;

    private float speed = 0;
    private LuckyTurnVoiceType angle_Num = LuckyTurnVoiceType.OnceAgain;
    private float angle;
    private float angleDistance;//相隔角度
    private float offsetAngle;//偏移角度
    private float twoPress_time;//二次按压头部
    private bool isSlowSpeed = false;
    private bool isRatate = false;
    private bool pauseIe = false;//暂停携程
    private int stillTimes = 2;//默认剩余2次
    private int partNum = 0;//已拥有碎片数量
    private bool onSale = false;//是否有优惠券
    private bool codeEnter = true;//支付进入
    private int turnIndex = 0;
    private TurnStatus turnStatus = TurnStatus.NotOperation;
    private readonly Color grey = new Color(115 / 255.0f, 115 / 255.0f, 115 / 255.0f);
    private List<VoiceContent> jokelist = null;
    private List<VoiceContent> currentVC = null;
    private List<VoiceContent> listOnSale;//礼品卷
    private List<GameObject> listObjs = new List<GameObject>();
    private List<GameObject> effect_objs = new List<GameObject>();
    private TransferData transferData = new TransferData();//传输数据
    private Dictionary<LuckyTurnVoiceType, List<VoiceContent>> luckyturnDic;
    private Dictionary<LuckyTurnVoiceType, float> angleDic;

    public override void Init()
    {
        owngiftPart = CommTool.FindObjForName(gameObject, "own");
        arrow = CommTool.FindObjForName(gameObject, "arrow").transform;
        arrow_up = CommTool.FindObjForName(arrow.gameObject, "arrow_up").transform;
        item_parent = CommTool.FindObjForName(gameObject, "Items").transform;
        logo = CommTool.FindObjForName(gameObject, "logo").transform;
        gift_part = CommTool.FindObjForName(gameObject, "giftpart").transform;
        time_count = CommTool.GetCompentCustom<Text>(gameObject, "time");
        still_times = CommTool.GetCompentCustom<Text>(gameObject, "times");
        pat_head = CommTool.GetCompentCustom<Image>(gameObject, "introduction");
        canvasGrp = CommTool.GetCompentCustom<CanvasGroup>(gameObject, "introduction");
        bg = CommTool.GetCompentCustom<Image>(gameObject, "bg");
        bgSprites = bg.GetComponent<ChangeBg>().bgsprites;
        mask = CommTool.FindObjForName(gameObject, "mask").transform;
        mask_effect = CommTool.FindObjForName(mask.gameObject, "mask_effect");
        mask_bg = CommTool.FindObjForName(mask.gameObject, "mask_bg");
        camera_effect = CommTool.FindObjForName(gameObject, "Effect_Camera").transform;
        var go = CommTool.FindObjForName(gameObject, "pressDowm");
        go.SetActive(false);
        EventDispatcher.AddListener<Action<string>>(EventHandlerType.ShowGiftPart, ShowGiftPart);

        if (GameCtr.test)
        {
            bg.raycastTarget = true;
            go.SetActive(true);
            UIEventLisener.Get(go).OnClick += g =>
            {
                GameCtr.Instance.AndroidCall(CallParameter.HeadDown.ToString());
            };
        }
    }

    public override void OnOpen()
    {
        LoadVoice();
        InitBasicData();
        InitTransferData();
        InitGirtPartColor();
        InitAngleValue();
        GenerateItems();
        GenerateEffect();
        SuccessProbability();
    }

    public override void OnShow(object data)
    {
        AudioManager.Instance.PlayByName(AssetFolder.LuckyTurn, AudioType.Fixed, AudioNams.TurnOpen, false);
        AudioManager.Instance.PlayByName(AssetFolder.LuckyTurn, AudioType.BackGround, AudioNams.BGMTurn, true);
        ie_effect = ShowEffect();
        StartCoroutine(Ie_Effect());
        StartTimeCount();
    }
    //开始计时
    void StartTimeCount()
    {
        StartCoroutine(CommTool.TimeFun(30, 1f, (ref float t) =>
        {
            if (!pauseIe) time_count.text = t.ToString();//没暂停才复制
            if (turnStatus == TurnStatus.NotOperation)
            {
                Speak(30 - t);
                if (t == 29)
                    GameCtr.Instance.RegHeadAction(() => HeadPress());
                if (t == 0)//自动转
                {
                    HeadPress(true);
                    t = 20;
                }
            }
            else if (turnStatus == TurnStatus.HeadDown)
            {
                pauseIe = false;
                turnIndex = 0;
                turnStatus = TurnStatus.Run;
                GameCtr.Instance.RegHeadAction(() => HeadPress());
                t = 20;
            }
            else if (turnStatus == TurnStatus.Run)
            {
                Speak(20 - t);
                if (t == 0)// 自动减速停止
                {
                    HeadPress(true);
                    t = 3;
                }
            }
            else  //slowDown
            {
                t = 30;//重新设为 起始时间
            }
            return false;
        },
       null,
       () => pauseIe));
    }



    //初始化基本数据
    void InitBasicData()
    {
        partNum = GetPartNum();
        Debug.Log("礼品碎片数量--::" + partNum);
        //是true支付进入  false答题进入
        codeEnter = GameCtr.Instance.ChangeType<LuckyTurnMgr>().codeEnter;
        listOnSale = GameCtr.Instance.ChangeType<LuckyTurnMgr>().listOnSaleNumber;//优惠券
        onSale = listOnSale != null && listOnSale.Count > 0 ? true : false;
        Debug.Log("扫码进入::" + codeEnter + "---有优惠券::" + onSale);

        stillTimes = GameCtr.Instance.gameStatus.remainGameRound;
        still_times.text = stillTimes.ToString();
        currentVC = GetCurrentTimesVoice("1");//1 是无操作 2是转动中
        twoPress_time = AudioManager.Instance.GetClipLength(AssetFolder.LuckyTurn, AudioNams.Two_pressHead);
        GameCtr.Instance.RegHeadAction(() => HeadPress());
    }
    //初始化传输数据
    void InitTransferData()
    {
        transferData.contiunAction = ContiunePaly;
        transferData.joke = jokelist;
        transferData.pos = gift_part.position;
    }
    //初始化礼品碎片
    void InitGirtPartColor()
    {
        owngiftPart.SetActive(codeEnter);
        if (codeEnter)
        {
            if (partNum == 0)//没有拥有礼品碎片
            {
                for (int i = 1; i <= 4; i++)
                {
                    gift_part.GetChild(i - 1).GetComponent<Image>().color = grey;
                }
            }
            else
            {
                for (int i = 1; i <= partNum; i++)
                {
                    gift_part.GetChild(i - 1).GetComponent<Image>().color = new Color(1, 1, 1);
                }
            }
        }

    }
    //初始化转盘元素角度  参数为是否还有优惠券
    void InitAngleValue(bool hasSale = true)
    {
        angleDic = new Dictionary<LuckyTurnVoiceType, float>();
        int len = 0;
        if (codeEnter)
        {
            if (onSale && hasSale)
            {
                bg.sprite = bgSprites[2];
                angleDistance = 360 / 7.0f;
                len = 11;
                offsetAngle = 38;
            }
            else
            {
                bg.sprite = bgSprites[1];
                angleDistance = 360 / 6.0f;
                len = 10;
                offsetAngle = 0; //  0 是6个盘
            }
            for (int i = 4; i < len; i++)//4开始是转盘项数据
            {
                angleDic.Add((LuckyTurnVoiceType)i, 0);
            }
        }
        else
        {
            if (onSale && hasSale)
            {
                bg.sprite = bgSprites[1];
                angleDistance = 360 / 6.0f;
                len = 11;
                offsetAngle = 0; //  0 是6个盘
            }
            else
            {
                bg.sprite = bgSprites[0];
                angleDistance = 360 / 5.0f;
                len = 10;
                offsetAngle = 18;
            }
            for (int i = 4; i < len; i++)//4开始是转盘项数据
            {
                if (i != (int)LuckyTurnVoiceType.GiftPart)
                    angleDic.Add((LuckyTurnVoiceType)i, 0);
            }
        }
        Debug.Log("angleDistance--::" + angleDistance);
    }
    //创建转盘数据
    void GenerateItems()
    {
        if (!item_parent) return;
        Transform item = item_parent.Find("item");
        GameObject go = null;
        listObjs.Clear();
        var listKeys = angleDic.Keys.ToList();
        for (int i = 0; i < listKeys.Count; i++)
        {
            go = Instantiate<GameObject>(item.gameObject);
            go.transform.SetParent(item_parent);
            go.transform.localPosition = new Vector3(0, 184, 0);
            go.transform.localRotation = Quaternion.Euler(Vector3.zero);
            go.transform.localScale = Vector3.one;
            Transform content = go.transform.Find("content");
            content.GetComponent<Text>().text = listKeys[i].GetEnumContent();
            Image img = go.transform.Find("img").GetComponent<Image>();
            img.sprite = UIAtlasManager.Instance.LoadSprite(UIAtlasName.UILuckyTurn, listKeys[i].ToString());
            img.SetNativeSize();
            go.transform.RotateAround(logo.position, Vector3.forward, (i * angleDistance) + offsetAngle);
            img.transform.DORotate(Vector3.zero, 0.01f);
            if (i >= 2 && i <= listKeys.Count - 2)
                content.DOLocalRotate(new Vector3(0, 0, 180), 0.01f);
            go.SetActive(true);
            angleDic[listKeys[i]] = (i * angleDistance) + offsetAngle;//对应角度
            listObjs.Add(go);
        }
        item.gameObject.SetActive(false);
    }

    //生成特效物体
    void GenerateEffect()
    {
        float noraml_angle = 30;
        for (int i = 0; i < camera_effect.childCount; i++)
        {
            effect_objs.Add(camera_effect.GetChild(i).gameObject);
            effect_objs[i].transform.RotateAround(logo.position, Vector3.forward, noraml_angle * i);
        }
        effect_objs.Reverse();

    }
    //检测特效
    IEnumerator Ie_Effect()
    {
        while (true)
        {
            if (ie_effect == null || !ie_effect.MoveNext())
                yield return null;
            else
                yield return ie_effect.Current;
        }
    }

    //展示特效数据
    IEnumerator ShowEffect()
    {
        while (true)
        {
            for (int i = 0; i < effect_objs.Count; i++)
            {
                effect_objs[i].layer = LayerMask.NameToLayer(GameCtr.layer_Light_effect); //显示灯特效
                yield return new WaitForSeconds(0.15f);
            }
            yield return new WaitForSeconds(0.5f);
            int _count = 2;
            while (_count-- >= 0)
            {
                effect_objs.ForEach(o => o.layer = LayerMask.NameToLayer(GameCtr.layer_Water));
                yield return new WaitForSeconds(0.8f);
                effect_objs.ForEach(o => o.layer = LayerMask.NameToLayer(GameCtr.layer_Light_effect));
                yield return new WaitForSeconds(0.8f);
            }
            effect_objs.ForEach(o => o.layer = LayerMask.NameToLayer(GameCtr.layer_Water));
        }
    }

    //根据指针转动位置点亮特效
    IEnumerator LightEffectRotateByAngle()
    {
        while (true)
        {
            for (int i = 0; i < effect_objs.Count; i++)
            {
                float dis = Vector3.Distance(arrow_up.transform.position, effect_objs[i].transform.position);
                if (dis < 3f)
                    effect_objs[i].layer = LayerMask.NameToLayer(GameCtr.layer_Light_effect);//显示灯特效
                if (dis > 5.3f)
                    effect_objs[i].layer = LayerMask.NameToLayer(GameCtr.layer_Water);//hide灯特效
            }
            yield return null;
        }
    }

    //加载语音
    void LoadVoice()
    {
        LuckyTurn joke = VoiceMrg<LuckyTurn, ExtendContent>.GetVoiceFromAsset("jokeVoice");
        if (joke != null)
        {
            jokelist = joke.luckyTurnList;
            for (int i = 0; i < jokelist.Count; i++)
            {
                jokelist[i].Contents = jokelist[i].Content.Split('|');
            }
            Debug.Log("jokeVoiceCount::" + jokelist.Count);
        }
        LuckyTurn turn = VoiceMrg<LuckyTurn, ExtendContent>.GetVoiceFromAsset("luckyTurn");
        if (turn != null)
        {
            luckyturnDic = turn.luckyTurnList.GroupBy(v => (LuckyTurnVoiceType)Convert.ToInt32(v.Id)).
                ToDictionary(g => g.Key, g => g.ToList());

            foreach (var item in luckyturnDic.Values)
            {
                for (int i = 0; i < item.Count; i++)
                {
                    item[i].Contents = item[i].Content.Split('|');
                }
            }
            Debug.Log("luckyturndicCount::" + luckyturnDic.Count);
        }
    }

    void Update()
    {
        StartTurn();
    }

    void StartTurn()
    {
        if (isRatate)
        {
            if (isSlowSpeed) //减速
            {
                if (speed > 150)
                {
                    speed -= 15f;
                    arrow.Rotate(Vector3.forward * speed * Time.deltaTime * -1, Space.Self);
                }
                else
                {
                    float eulerAngle = arrow.eulerAngles.z;
                    float moveTime = 0;
                    if (eulerAngle > 358 && eulerAngle < 360) //为了dotween 不反转
                    {
                        if (angle >= 180) //180中间点
                        {
                            moveTime = (Math.Abs(eulerAngle - angle) / 90.0f) * 3f; //转90度需要2秒
                            Debug.Log("eulerAngles:::" + eulerAngle + "---moveTime-::" + moveTime + "--speed--" + speed);
                            if (moveTime > 0.5f || speed < 60)
                            {
                                isRatate = false;
                                isSlowSpeed = false;
                                arrow.DORotate(new Vector3(0, 0, angle), moveTime, RotateMode.Fast).SetEase(Ease.OutQuad).OnComplete(ShowResultUI);
                                return;
                            }
                        }

                    }
                    else if (eulerAngle > 178 && eulerAngle < 180) //为了dotween 不反转
                    {
                        if (angle < 180)
                        {
                            moveTime = (Math.Abs(eulerAngle - angle) / 90.0f) * 3f; //转90度需要2秒
                            Debug.Log("eulerAngles:::" + eulerAngle + "---moveTime-::" + moveTime + "--speed--" + speed);
                            if (moveTime > 0.5f || speed < 60)
                            {
                                isRatate = false;
                                isSlowSpeed = false;
                                arrow.DORotate(new Vector3(0, 0, angle), moveTime, RotateMode.Fast).SetEase(Ease.OutQuad).OnComplete(ShowResultUI);
                                return;
                            }
                        }

                    }
                    if (speed >= 60)
                        speed -= 0.5f;
                    arrow.Rotate(Vector3.forward * speed * Time.deltaTime * -1, Space.Self);
                }
            }
            else  //加速
            {
                arrow.Rotate(Vector3.forward * -speed * Time.deltaTime, Space.Self);
                speed += 18f;
                if (speed > 2000)
                {
                    AudioManager.Instance.PlayByName(AssetFolder.LuckyTurn, AudioType.Continuous, AudioNams.TurnContiune, false);
                    speed = 2000;
                }
            }
        }
    }

    void Speak(float time)
    {
        if (turnIndex < currentVC.Count && time == Convert.ToSingle(currentVC[turnIndex].Time))
        {
            int contentIndex = UnityEngine.Random.Range(0, currentVC[turnIndex].Contents.Length);
            Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords,
                currentVC[turnIndex].Contents[contentIndex]);
            turnIndex++;
        }
    }
    void HeadPress(bool isAuto = false)
    {
        GameCtr.Instance.RegHeadAction(null);
        if (!isRatate)//开始转动
        {
            ie_effect = LightEffectRotateByAngle();
            Debug.Log("开始转动");
            if (!isAuto)
                Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, "开始喽！");
            pauseIe = true;
            isRatate = true;
            isSlowSpeed = false;
            currentVC = GetCurrentTimesVoice("2");
            pat_head.sprite = UIAtlasManager.Instance.LoadSprite(UIAtlasName.UILuckyTurn, "pressheadend");
            pat_head.SetNativeSize();
            pat_head_tweener = canvasGrp.DOFade(0.1f, 1.5f).SetLoops(-1, LoopType.Yoyo);
            turnStatus = TurnStatus.HeadDown;
        }
        else //开始减速
        {
            Debug.Log("开始减速");
            AudioManager.Instance.PlayByName(AssetFolder.LuckyTurn, AudioType.Fixed, AudioNams.Two_pressHead, false);
            DOVirtual.DelayedCall(twoPress_time, () =>
            {
                if (!isAuto)
                    Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, "哇，快看看能转到哪个奖励上吧");
                pauseIe = true;
                isSlowSpeed = true;
                pat_head_tweener.Kill();
                pat_head.gameObject.SetActive(false);
                turnStatus = TurnStatus.SlowDown;
                AudioManager.Instance.StopPlayAds(AudioType.Continuous);
                AudioManager.Instance.PlayByName(AssetFolder.LuckyTurn, AudioType.Fixed, AudioNams.TurnSlowDown, false);
            });
        }
    }

    //展示底部礼品碎片
    void ShowGiftPart(Action<string> combin)
    {
        Debug.Log("ShowGiftPart");
        partNum++;
        if (partNum == 4)
        {
            Debug.Log("集齐四个啦啦");
            InitGirtPartColor();
            DOVirtual.DelayedCall(0.6f, () =>
            {
                partNum = 0;
                InitGirtPartColor();
                UpdatePartNumToTable();//更新到数据库
                if (combin != null)
                {
                    string speak = luckyturnDic[LuckyTurnVoiceType.GiftPart].Find(v => v.Type == "2").Content;
                    combin(speak);
                }
            });
        }
        else
        {
            //继续游戏
            InitGirtPartColor();
            UpdatePartNumToTable();
            GameCtr.Instance.gameMode.UpRecord(false);//上报抓取记录
            UIManager.Instance.ShowUI(UITurnResultPage.NAME, false);
            ContiunePaly();
        }
    }

    //继续游戏  下一局
    void ContiunePaly()
    {
        if (angle_Num != LuckyTurnVoiceType.OnceAgain)
            stillTimes--;
        if (stillTimes < 0)
        {
            UIManager.Instance.ShowUI(UITurnResultPage.NAME, true, null);
            EventDispatcher.Dispatch(EventHandlerType.GameOver);//游戏结束
        }
        else
        {
            //下一局开始
            ResetTurnData();
            SuccessProbability();
            AudioManager.Instance.PlayByName(AssetFolder.LuckyTurn, AudioType.BackGround, AudioNams.BGMTurn, true);
            GameCtr.Instance.gameStatus.SetRemainRound(stillTimes);
            currentVC = GetCurrentTimesVoice("1");
            still_times.text = stillTimes.ToString();
            GameCtr.Instance.RegHeadAction(() => HeadPress());
            ie_effect = ShowEffect();
            pat_head.gameObject.SetActive(true);
            canvasGrp.DOFade(1f, 0.1f);
            pat_head.sprite = UIAtlasManager.Instance.LoadSprite(UIAtlasName.UILuckyTurn, "pressheadstart");
            pat_head.SetNativeSize();
            pauseIe = false;
            turnIndex = 0;
            turnStatus = TurnStatus.NotOperation;
        }
    }

    //展示特效 mask  结果ui
    void ShowResultUI()
    {
        if (!mask) return;
        speed = 0;
        ie_effect = null;
        mask_bg.SetActive(true);
        mask_effect.layer = LayerMask.NameToLayer(GameCtr.layer_Light_effect);//显示光圈特效
        effect_objs.ForEach(o => o.layer = LayerMask.NameToLayer(GameCtr.layer_Water));  //隐藏灯特效
        AudioManager.Instance.PlayByName(AssetFolder.LuckyTurn, AudioType.Fixed, AudioNams.TurnStop, false);
        int itemIdx = GetTurnItemIndex(angle_Num);
        if (itemIdx >= 0)
        {
            GameObject go = Instantiate<GameObject>(listObjs[itemIdx]);
            go.transform.SetParent(mask);
            go.transform.localScale = Vector3.one;
            go.transform.position = listObjs[itemIdx].transform.position;
            mask_effect.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, -150);
            DOVirtual.DelayedCall(2f, () =>
             {
                 Destroy(go);
                 mask_bg.SetActive(false);
                 mask_effect.layer = LayerMask.NameToLayer(GameCtr.layer_Water);//隐藏光圈特效
                 transferData.uiType = angle_Num;
                 transferData.voice = luckyturnDic[angle_Num];
                 UIManager.Instance.ShowUI(UITurnResultPage.NAME, true, transferData);
             });
        }
    }

    //中奖概率计数
    void SuccessProbability()
    {
        bool success = GameCtr.Instance.gameStatus.status == 1;
        angle_Num = RandomProbli.GetRandomPro(success, codeEnter, onSale, partNum);
        float max = angleDic[angle_Num] + 5;//制造自然效果
        float min = angleDic[angle_Num] - 15;
        angle = UnityEngine.Random.Range(min, max);
        if (angle >= 360 || angle < 0) angle = 350;
        Debug.Log("：：转到：：" + angle_Num.GetEnumContent() + "//--角度--：：" + angle);
    }

    //获得碎片数量
    int GetPartNum()
    {
        string number = GameCtr.Instance.handleSqlite.ReadDataGiftPart(GiftPartTable.PartNum);
        if (string.IsNullOrEmpty(number))//数据库没数据
            return 0;
        return Convert.ToInt32(number);
    }
    //更新碎片数量
    void UpdatePartNumToTable()
    {
        if (partNum <= 4)
            GameCtr.Instance.handleSqlite.UpdateGiftPart(partNum);
    }
    //获得当前局数语音
    List<VoiceContent> GetCurrentTimesVoice(string type)
    {
        int timesType = 0;
        if (GameCtr.Instance.selectRound == 5)//五局制
        {
            if (stillTimes == 4)
                timesType = 3;
            else if (stillTimes > 0 && stillTimes <= 3)
                timesType = 2;
            else
                timesType = 1;
        }
        else
        {
            timesType = stillTimes + 1; // stillTimes + 1 是因为语音库的每局语音类型是3 2 1
        }

        return luckyturnDic[(LuckyTurnVoiceType)timesType].FindAll(v => v.Type == type);//1 是无操作 2是转动中
    }
    //优惠券用完重置转盘数据
    void ResetTurnData()
    {
        if (angle_Num == LuckyTurnVoiceType.OnSale)//上次是优惠券 需要判断是否还有优惠劵
        {
            if (listOnSale != null && listOnSale.Count == 0)//优惠券使用完了
            {
                Debug.Log("--优惠券使用完了---");
                onSale = false;

                //下面代码是重新创建转盘元素
                //DestoryTurnItems();
                //InitAngleValue(false);
                //GenerateItems();
            }
        }
    }

    //获得转盘物体index
    int GetTurnItemIndex(LuckyTurnVoiceType vt)
    {
        var klist = angleDic.Keys.ToList();
        return klist.FindIndex(i => i == vt);
    }


    //删除转盘元素
    void DestoryTurnItems()
    {
        for (int i = 0; i < listObjs.Count; i++)
        {
            Destroy(listObjs[i]);
        }
        listObjs.Clear();
    }
}
