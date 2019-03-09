using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using DG.Tweening;

public sealed class UIQuestionPage : UIDataBase
{
    public const string NAME = "UIQuestionPage";
    private const string e_text = "✘";
    private const string r_text = "✔";
    private readonly Color e_color = new Color(229 / 255f, 34 / 255f, 0);
    private readonly Color r_color = new Color(136 / 255f, 229 / 255f, 49 / 255f);
    private readonly Vector3 left_ratate = new Vector3(0, 0, 30);
    private readonly Vector3 right_ratate = new Vector3(0, 0, -30);
    private bool isPlayGame = true;
    public override UIShowPos ShowPos
    {
        get { return UIShowPos.TipTop; }
    }
    public override HidePage hidePage
    {
        get { return HidePage.Destory; }
    }
    public override AssetFolder assetFolder
    {
        get { return AssetFolder.LuckyBoy; }
    }
    private GameObject question;
    private GameObject startup;
    private GameObject question_start;
    private GameObject question_end;
    private GameObject s_welcome;
    private GameObject s_rule;
    private GameObject q_start_right;
    private GameObject q_start_error;
    private GameObject q_start_last_right;
    private GameObject q_start_last_error;
    private GameObject q_end_fail;
    private GameObject q_end_success;
    private GameObject q_end_fail_nogame;
    private GameObject q_end_success_nogame;
    private GameObject left_wing_parent;
    private GameObject left_wing;
    private GameObject right_wing;
    private GameObject right_wing_parent;
    private GameObject shake_wing_bg;

    private Text s_rule_in_all;
    private Text s_rule_in_all_right;
    private Text s_rule_game;
    private Text q_start_right_count;
    private Text q_start_whichone_number;
    private Text q_start_content;
    private Text q_start_a_answer;
    private Text q_start_b_answer;
    private Text a_gou;
    private Text b_gou;
    private Text q_end_in_all_count;
    private Text q_end_right_count;
    private Text q_start_time;

    private int index = 0;  
    private int rightNum = 0;
    private int select_time = 0;//播放  答对或答错的时间
    private float timer = 30;//计时
    private Answer_Question answer_Status = Answer_Question.Welcome;//是否处于答题状态
    private Q_Question currentMQ = null;
    private Action startGameOrQuit = null;//开始游戏
    private Action showRightErrorUI = null;//显示对错面板
    private GameCtr sdk;
    private List<Q_Question> q_library_list = new List<Q_Question>();
    private List<KeyValuePair<float, string[]>> q_voice_kvp = new List<KeyValuePair<float, string[]>>();

    private bool left_winging = false;
    private bool right_winging = false;

    public override void Init()
    {
        base.Init();
        sdk = LuckyBoyMgr.Instance;
        question = CommTool.FindObjForName(gameObject, "question");
        startup = CommTool.FindObjForName(question, "StartUp");
        s_welcome = CommTool.FindObjForName(startup, "Welcome");
        s_rule = CommTool.FindObjForName(startup, "Rule");
        s_rule_in_all = CommTool.GetCompentCustom<Text>(s_rule, "in_all");
        s_rule_in_all_right = CommTool.GetCompentCustom<Text>(s_rule, "in_all_right");
        s_rule_game = CommTool.GetCompentCustom<Text>(s_rule, "game");
        question_start = CommTool.FindObjForName(question, "Question_Start");
        left_wing_parent = CommTool.FindObjForName(question_start, "left_wing");
        left_wing = CommTool.FindObjForName(left_wing_parent, "left_w");
        right_wing_parent = CommTool.FindObjForName(question_start, "right_wing");
        right_wing = CommTool.FindObjForName(right_wing_parent, "right_w");
        shake_wing_bg = CommTool.FindObjForName(question_start, "shake_wing_bg");
        q_start_right_count = CommTool.GetCompentCustom<Text>(question_start, "count");
        q_start_time = CommTool.GetCompentCustom<Text>(question_start, "time");
        q_start_whichone_number = CommTool.GetCompentCustom<Text>(question_start, "number");
        q_start_right = CommTool.FindObjForName(question_start, "right");
        q_start_error = CommTool.FindObjForName(question_start, "error");
        q_start_last_right = CommTool.FindObjForName(question_start, "last_right");
        q_start_last_error = CommTool.FindObjForName(question_start, "last_error");
        q_start_content = CommTool.GetCompentCustom<Text>(question_start, "Content");
        q_start_a_answer = CommTool.GetCompentCustom<Text>(question_start, "A_answer");
        q_start_b_answer = CommTool.GetCompentCustom<Text>(question_start, "B_answer");
        a_gou = CommTool.GetCompentCustom<Text>(question_start, "A_gou");
        b_gou = CommTool.GetCompentCustom<Text>(question_start, "B_gou");
        question_end = CommTool.FindObjForName(question, "Question_End");
        q_end_success = CommTool.FindObjForName(question_end, "success");
        q_end_fail = CommTool.FindObjForName(question_end, "fail");
        q_end_success_nogame = CommTool.FindObjForName(question_end, "success_nogame");
        q_end_fail_nogame = CommTool.FindObjForName(question_end, "fail_nogame");
        q_end_in_all_count = CommTool.GetCompentCustom<Text>(question_end, "in_all_count");
        q_end_right_count = CommTool.GetCompentCustom<Text>(question_end, "right_count");

        if (LuckyBoyMgr.test)
        {
            UIEventLisener.Get(q_start_a_answer.gameObject).OnClick += o => Question_Wing("0");
            UIEventLisener.Get(q_start_b_answer.gameObject).OnClick += o => Question_Wing("1");
            UIEventLisener.Get(s_rule).OnClick += o => HeadPress();
        }

    }

    public override void OnOpen()
    {
        InitVoice();
    }

    public override void OnShow(object data)
    {
        object[] datas = (object[])data;
        sdk.gameTryStatus = -100;//答题模式
        q_library_list = datas[0] as List<Q_Question>;
        startGameOrQuit = datas[1] as Action;
        isPlayGame = (bool)datas[2];
        startup.SetActive(true);
        GetRandomQuestion();
        AudioManager.Instance.PlayByName(AssetFolder.Common, AudioType.Fixed, AudioNams.Loading_voice, false);//播放loading音效
        string speak = isPlayGame ? Get_Randow_Voice(QuestionVoiceType.Loading_Game) : Get_Randow_Voice(QuestionVoiceType.Loading_Gift);
        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords,speak);
        Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, true);//摆动翅膀闪光带
        StartTimeCount();
    }
    //初始化语音数据
    private void InitVoice()
    {
        List<VoiceContent> lv = sdk.gameMode.qvs.q_v_list;
        lv.Sort((t1, t2) => Convert.ToInt32(t1.Id) - Convert.ToInt32(t2.Id));//索引值 对应枚举值 QuestionVoiceType
        KeyValuePair<float, string[]> kvp;
        string[] cs = null;
        for (int i = 0; i < lv.Count; i++)
        {
            cs = lv[i].Content.Split('|');
            kvp = new KeyValuePair<float, string[]>(Convert.ToSingle(lv[i].Time), cs);
            q_voice_kvp.Add(kvp);
        }
    }

    //开始倒计时
    private void StartTimeCount()
    {
        float ruleTime = timer - q_voice_kvp[(int)QuestionVoiceType.Rule].Key;
        float welcomeTime = timer - q_voice_kvp[(int)QuestionVoiceType.Loading_Gift].Key;
        int ex_type = 0;
        bool isEnd = false;
        StartCoroutine(CommTool.TimeFun(timer, 0.5f, (ref float t) =>
        {
            if (answer_Status == Answer_Question.Welcome)
            {
                if (t == welcomeTime)
                {
                    t = timer;
                    s_welcome.SetActive(false);
                    s_rule.SetActive(true);
                    s_rule_in_all.text = sdk.question + "";
                    s_rule_in_all_right.text = sdk.pass + "";
                    if (isPlayGame)
                        s_rule_game.text = "玩游戏赢礼品的机会。";
                    else
                        s_rule_game.text = "的礼品哦";
                    answer_Status = Answer_Question.Rule;
                    sdk.RegHeadAction(HeadPress);//注册头部事件
                    Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords,
                       Get_Randow_Voice(QuestionVoiceType.Rule));
                    Android_Call.UnityCallAndroidHasParameter<int>(AndroidMethod.ShakeWave, 5000);
                    Android_Call.UnityCallAndroidHasParameter<bool, int>(AndroidMethod.OpenLight, true, 5000);
                }
            }
            else if (answer_Status == Answer_Question.Rule)//答题规则页面
            {
                if (ex_type == 0)//规则语音播放完毕
                {
                    if (t == ruleTime)
                    {
                        ex_type++;
                        Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, false);
                    }
                }
                else if (ex_type == 1)//游戏结束 语音
                {
                    if (t == 0)
                    {
                        ex_type++;
                        t = Get_Q_Time(QuestionVoiceType.Auto_Quit);
                        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords,
                        Get_Randow_Voice(QuestionVoiceType.Auto_Quit));
                        Android_Call.UnityCallAndroidHasParameter<bool, int>(AndroidMethod.OpenLight, false, (int)t * 1000);
                    }
                }
                else if(ex_type==2)//游戏退出
                {
                    if (t <= 4.5f) //防止 进入游戏后 不能游戏（摇动左右翅膀需要时间）
                        sdk.Q_AppQuit();
                }
            }
            else if (answer_Status == Answer_Question.HeadDown)//规则页面头部按下
            {
                Debug.Log("answer_Status::"+ answer_Status);
                t = timer;//重新计时
                answer_Status = Answer_Question.Answering;
            }
            else if (answer_Status == Answer_Question.Answering)//答题中
            {
                ShowTime(t);
                if (t == 29)//可以答题啦  注册答题
                    EventDispatcher.AddListener<string>(EventHandlerType.Question_Wing, Question_Wing);
                if (t == 15)//无操作15s
                {
                    Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords,
                      Get_Randow_Voice(QuestionVoiceType.No_operation_10));
                    ShakeWingHintAnswer(false);
                }
                else if (t == 3) // 自动答题语音
                { 
                    Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords,
                         Get_Randow_Voice(QuestionVoiceType.No_operation_30));
                }
                else if (t == 0)//自动答题
                {
                    t = timer;
                    int a_b = UnityEngine.Random.Range(0, 2);//随机答题
                    Debug.Log("随机答题---" + a_b);
                    Question_Wing(a_b.ToString());
                }
            }
            else if (answer_Status == Answer_Question.Answered)  //已答题
            {
                t = select_time;
                answer_Status = Answer_Question.PlayAnswerVoice;
            }
            else if (answer_Status == Answer_Question.PlayAnswerVoice)//播放答题语音
            {
                if (t == select_time - 0.5f)
                {
                    if (showRightErrorUI != null) showRightErrorUI();
                }
                else if (t == 0)//语音播放完成  下一题
                {
                    index++;//下一题
                    t = timer;
                    Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, false);
                    answer_Status = Answer_Question.Answering;
                    ShowTime(t);
                    HideRightError();
                    WriteQuestion();
                }
            }
            else if (answer_Status == Answer_Question.AnswerFinish)  //完成答题
            {
                t = select_time;
                answer_Status = Answer_Question.PlayFinishVoice;
                sdk.RegHeadAction(() => isEnd = true);//注册拍头 结束 或开始游戏
            }
            else //播放完成语音
            {
                if (t == 0 || isEnd)
                {
                    q_end_success_nogame.SetActive(false);
                    startGameOrQuit();//进入游戏或退出
                    return true;
                }
            }
            return false;

        }, null));

    }

    //显示倒计时
    private void ShowTime(float t)
    {
        int tp_t;
        string temp = t.ToString();
        if (int.TryParse(temp, out tp_t))
            q_start_time.text = temp;
        temp = null;
    }

    //获得随机题目
    private void GetRandomQuestion()
    {
        int _q_count = sdk.question;
        if (q_library_list.Count < _q_count)
            _q_count = q_library_list.Count;
        List<Q_Question> _list = new List<Q_Question>();
        for (int i = 0; i < _q_count; i++)
        {
            int r_index = UnityEngine.Random.Range(0, q_library_list.Count);
            _list.Add(q_library_list[r_index]);
            q_library_list.RemoveAt(r_index);
        }
        q_library_list = _list;
        _list = null;
    }


    //开始出题
    private void WriteQuestion()
    {
        if (q_library_list.Count > index)
        {
            PlayQuestionVoice();
            currentMQ = q_library_list[index];
            q_start_whichone_number.text = (index + 1).ToString();
            q_start_content.text = currentMQ.question;
            int r_num = UnityEngine.Random.Range(0, 11);
            if (currentMQ.rightAnswer.Length > 16)
                currentMQ.rightAnswer = currentMQ.rightAnswer.Substring(0, 16);
            if (currentMQ.wrongAnswer.Length > 16)
                currentMQ.wrongAnswer = currentMQ.wrongAnswer.Substring(0, 16);
            if (r_num >= 5)
            {
                q_start_a_answer.text = currentMQ.rightAnswer;
                q_start_b_answer.text = currentMQ.wrongAnswer;
                currentMQ.right_wing = 0;
            }
            else
            {
                q_start_a_answer.text = currentMQ.wrongAnswer;
                q_start_b_answer.text = currentMQ.rightAnswer;
                currentMQ.right_wing = 1;
            }
            q_start_right_count.text = rightNum.ToString();
        }
        else  //结束答题
        {
            //解除翅膀监听
            Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.AnswerStartOrEnd, false);
            answer_Status = Answer_Question.AnswerFinish;
            question_end.SetActive(true);
            q_end_success.SetActive(false);
            q_end_fail.SetActive(false);
            q_end_fail_nogame.SetActive(false);
            q_end_success_nogame.SetActive(false);
            q_end_in_all_count.text = sdk.question + "";
            q_end_right_count.text = rightNum.ToString();
            if (rightNum >= sdk.pass)//通过数量
            {
                //答题过关 进入游戏

                if (isPlayGame)
                {
                    q_end_success.SetActive(true);
                    Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords,
                       Get_Randow_Voice(QuestionVoiceType.Answer_Success));
                    select_time = Get_Q_Time(QuestionVoiceType.Answer_Success);
                }
                else
                {
                    q_end_success_nogame.SetActive(true);
                    Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, Get_Randow_Voice(QuestionVoiceType.Pass_Exam));
                    select_time = Get_Q_Time(QuestionVoiceType.Pass_Exam);
                }
            }
            else
            {

                startGameOrQuit = sdk.Q_AppQuit;
                if (isPlayGame)
                {
                    q_end_fail.SetActive(true);
                    Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords,
                        Get_Randow_Voice(QuestionVoiceType.Answer_Fail));
                    select_time = Get_Q_Time(QuestionVoiceType.Answer_Fail);
                }
                else
                {
                    q_end_fail_nogame.SetActive(true);
                    Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords,
                        Get_Randow_Voice(QuestionVoiceType.No_Present));
                    select_time = Get_Q_Time(QuestionVoiceType.No_Present);
                }
            }
        }
    }
    //隐藏对错
    private void HideRightError()
    {
        q_start_error.SetActive(false);
        q_start_right.SetActive(false);
        q_start_last_error.SetActive(false);
        q_start_last_right.SetActive(false);
        a_gou.gameObject.SetActive(false);
        b_gou.gameObject.SetActive(false);
    }

    //显示对于错
    private void ShowRightOrError(int selectWing)
    {
        select_time = 0;
        showRightErrorUI = null;
        ShakeWing(selectWing);
        if (selectWing == currentMQ.right_wing)//答对
        {
            select_time = Get_Q_Time(QuestionVoiceType.Select_Right);
            AudioManager.Instance.PlayByName(AssetFolder.Common, AudioType.Fixed, AudioNams.right_voice2, false);//播放答对音效
            Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords,
                      Get_Randow_Voice(QuestionVoiceType.Select_Right));
            Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, true);
            rightNum++;
            if (selectWing == 0)
            {
                a_gou.gameObject.SetActive(true);
                a_gou.text = r_text;
                a_gou.color = r_color;

            }
            else
            {
                b_gou.gameObject.SetActive(true);
                b_gou.text = r_text;
                b_gou.color = r_color;
            }

            showRightErrorUI = () =>
            {
                if (q_library_list.Count - 1 == index)
                    q_start_last_right.SetActive(true);
                else
                    q_start_right.SetActive(true);
            };
        }
        else  //答错
        {
            select_time = Get_Q_Time(QuestionVoiceType.Select_Error);
            AudioManager.Instance.PlayByName(AssetFolder.Common, AudioType.Fixed, AudioNams.error_voice, false);//播放答错音效
            Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords,
                    Get_Randow_Voice(QuestionVoiceType.Select_Error));
            if (selectWing == 0)
            {
                a_gou.gameObject.SetActive(true);
                a_gou.text = e_text;
                a_gou.color = e_color;
            }
            else
            {
                b_gou.gameObject.SetActive(true);
                b_gou.text = e_text;
                b_gou.color = e_color;
            }

            showRightErrorUI = () =>
            {
                if (q_library_list.Count - 1 == index)//最后一题不显示
                    q_start_last_error.SetActive(true);
                else
                    q_start_error.SetActive(true);
            };
        }
    }



    //获得问题语音
    private string Get_Randow_Voice(QuestionVoiceType qvt)
    {
        if ((int)qvt < q_voice_kvp.Count)
        {
            string[] vs = q_voice_kvp[(int)qvt].Value;
            int index = UnityEngine.Random.Range(0, vs.Length);
            return vs[index];
        }
        return null;
    }

    //获得问题语音时间
    private int Get_Q_Time(QuestionVoiceType qvt)
    {
        return Convert.ToInt32(q_voice_kvp[(int)qvt].Key);
    }

    //摇翅膀 答题
    private void Question_Wing(string data)
    {
        if (answer_Status == Answer_Question.Answering)
        {
            EventDispatcher.RemoveListener<string>(EventHandlerType.Question_Wing, Question_Wing);//取消注册
            answer_Status = Answer_Question.Answered;
            ShowRightOrError(Convert.ToInt32(data));
        }
    }

    //播放题目
    private void PlayQuestionVoice()
    {
        string _qv = "";
        int _time = Get_Q_Time(QuestionVoiceType.One_Question_Enter);
        Android_Call.UnityCallAndroidHasParameter<bool, int>(AndroidMethod.OpenLight, false,_time);
        if (index == 0)//第一题
        {
            _qv = string.Format("我们先来看一下第一题,{0},{1}", q_library_list[index].question,
                Get_Randow_Voice(QuestionVoiceType.One_Question_Enter));
           // Android_Call.Speak(_qv);
        }
        else if (index == q_voice_kvp.Count - 1)//最后一题
        {
            _qv = string.Format("最后一道喽,{0},{1}", q_library_list[index].question,
             Get_Randow_Voice(QuestionVoiceType.The_Rest_Question_Enter));
           // Android_Call.Speak(_qv);
        }
        else
        {
            _qv = string.Format("第{0}题,{1},{2}", index + 1, q_library_list[index].question,
            Get_Randow_Voice(QuestionVoiceType.The_Rest_Question_Enter));
            //Android_Call.Speak(_qv);
        }
        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, _qv);

    }


    public void HeadPress()
    {
        if (answer_Status == Answer_Question.Rule)
        {
            if (index == 0)
            //进入答题模式
            {
                //answer_Status = Answer_Question.HeadDown;
                Android_Call.UnityCallAndroidHasParameter<bool>(AndroidMethod.ShakeWaveLight, false);
                startup.SetActive(false);
                question_start.SetActive(true);
                HideRightError();
                //WriteQuestion();
                ShakeWingHintAnswer(true);
            }
        }
    }
    //摇动翅膀提示 玩家选择
    private void ShakeWingHintAnswer(bool isFirstEnter)
    {
        SetWingDefualtRatate();
        shake_wing_bg.SetActive(isFirstEnter);
        right_wing_parent.SetActive(!isFirstEnter);
        if(isFirstEnter)
        Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, "摇左翅膀选A");
        ShakeWing(0, () =>
         {
             left_wing.transform.localRotation = Quaternion.Euler(left_ratate);
             left_wing_parent.SetActive(!isFirstEnter);
             right_wing_parent.SetActive(true);
             if(isFirstEnter)
                 Android_Call.UnityCallAndroidHasParameter<string>(AndroidMethod.SpeakWords, "摇右翅膀选B");
             ShakeWing(1, () =>
              {
                  if (isFirstEnter)
                  {
                      answer_Status = Answer_Question.HeadDown;
                      WriteQuestion();
                      shake_wing_bg.SetActive(false);
                  }
              });
         });
    }
    /// <summary>
    /// 摇动翅膀
    /// </summary>
    /// <param name="leftright">0 左翅膀  1 右翅膀</param>
    /// <param name="action"></param>
    private void ShakeWing(int leftright, Action action = null)
    {
        Transform tran = null;
        if (leftright == 0)
        {
            if (left_winging)
                return;
            else
                left_winging = true;
            tran = left_wing.transform;
        }
        else
        {
            if (right_winging)
                return;
            else
                right_winging = true;
            tran = right_wing.transform;
        }
        tran.DOLocalRotate(new Vector3(0, 0, 0), 0.3f).SetLoops(6, LoopType.Yoyo)
        .OnComplete(() =>
        {
            SetWingDefualtRatate();
            if (action != null)
                action();
        });
    }

    //设置翅膀为默认角度
    private void SetWingDefualtRatate()
    {
        left_winging = false;
        right_winging = false;
        left_wing_parent.SetActive(true);
        left_wing.transform.localRotation = Quaternion.Euler(left_ratate);
        right_wing.transform.localRotation = Quaternion.Euler(right_ratate);
    }

    private void OnDestroy()
    {
        EventDispatcher.RemoveListener<string>(EventHandlerType.Question_Wing, Question_Wing);//取消注册
        startGameOrQuit = null;
        showRightErrorUI = null;
        q_library_list.Clear();
        q_library_list = null;
        q_voice_kvp.Clear();
        q_voice_kvp = null;
    }
}
