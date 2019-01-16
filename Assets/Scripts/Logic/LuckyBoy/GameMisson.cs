using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class GameMisson
{
    public int _timesPay { get; private set; }
    public GameLevel _gameLevel { get; private set; }
    public Action policeMove { get; private set; }
    public Action<bool> policeShoot { get; private set; }
    public Action playAction { get; private set; }
    public Tweener tween { get; private set; }
    public GameCtr sdk { get; private set; }
    public int _round { get; protected set; }
    public bool _isWin { get; protected set; }//是否抓中过
    public int _Count { get;  set; }

    // 标记次数
    public int signTimes { get; set; }

    protected GameMisson(GameCtr _sdk)
    {
        sdk = _sdk;
    }

    #region 实现函数
    /// <summary>
    /// 初始玩家第几次扫码计入游戏
    /// </summary>
    /// <param name="timesPaly"></param>
    public void IntiPayTimes(int timesPaly)
    {
        _timesPay = timesPaly;
        sdk.gameStatus.SetPayTime(_timesPay);//记录第几次支付
    }
    /// <summary>
    /// 初始化委托
    /// </summary>
    /// <param name="move"></param>
    /// <param name="shoot"></param>
    public void InitAction(Action move, Action<bool> shoot)
    {
        policeMove = move;
        policeShoot = shoot;
        playAction = PlayMove;
    }

   
   
    /// <summary>
    /// 警察射击
    /// </summary>
    public void PoliceShoot()
    {
        if (_gameLevel == GameLevel.Nan)
        {
            if (policeShoot != null)
                policeShoot(sdk.gameXP.catchty == CatchTy.Catch);
        }
    }
    /// <summary>
    /// 开始抓 暂停爪子移动
    /// </summary>
    public void SetCatchTweenStop()
    {
        if (tween != null)
        {
            tween.timeScale = 0;
        }
    }

    public void KillTween()
    {
        if (tween != null)
        {
            tween.Kill();
            tween = null;
        }
    }
    //最难玩法
    public void NormalPaly(GameObject police, Transform catchMove)
    {
        _gameLevel = GameLevel.Nan;
        if (police && catchMove)
        {
            police.SetActive(true);
            catchMove.transform.localPosition = new Vector3(55, 366, 0);
            tween = catchMove.DOLocalMoveX(181, 2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).Pause().SetAutoKill(false);
        }
    }
    //第二次玩  没有警察
    public void NoPolicePlay(GameObject police, Transform catchMove)
    {
        _gameLevel = GameLevel.Zhong;
        if (police && catchMove)
        {
            police.SetActive(false);
            catchMove.transform.localPosition = new Vector3(-240, 366, 0);
            tween = catchMove.DOLocalMoveX(240, 5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).Pause().SetAutoKill(false);
        }
    }
    //最简单玩法 没有警察且爪子不移动
    public void EasyPlay(GameObject police, Transform catchMove)
    {
        _gameLevel = GameLevel.Yi;
        if (police && catchMove)
        {
            police.SetActive(false);
            catchMove.transform.localPosition = new Vector3(0, 366, 0);
        }
    }
    //播放动画
    public void PlayMove()
    {
        EventHandler.ExcuteEvent(EventHandlerType.GameEndStart, false);
        sdk.RegHeadAction(StartCatchBoy);//注册抓娃娃
        PoliceMove();
        if (tween != null)
            tween.Play();
    }
    /// <summary>
    /// 警察移动
    /// </summary>
    public void PoliceMove()
    {
        if (_gameLevel == GameLevel.Nan)
        {
            if (policeMove != null)
                policeMove();
        }
    }

    //开始抓娃娃
    public void StartCatchBoy()
    {
        AudioManager.Instance.PlayByName(AssetFolder.LuckyBoy, AudioType.Fixed, AudioNams.downing2, false);
        EventHandler.ExcuteEvent(EventHandlerType.HeadPress, true);
    }

    #endregion

    #region  虚函数

    /// <summary>
    /// 设置掉落概率
    /// </summary>
    public virtual void SetDropProbability(bool isReach, ref CatchTy catchty)
    {
        if (_gameLevel == GameLevel.Nan)
        {
            //是否掉
            int num = UnityEngine.Random.Range(1, 101);
            Debug.Log("概率值----" + num);
            if (isReach || sdk.gameStatus.status == 1 || num > sdk.probability)//已抓中过 必掉
            {
                catchty = CatchTy.Drop;
            }
        }
        else
        {
            if (isReach || sdk.gameStatus.status == 1)//已抓中过 必掉 没有概率
            {
                catchty = CatchTy.Drop;
            }
        }
    }

    /// <summary>
    /// 玩家第几局进入游戏
    /// </summary>
    /// <param name="police">警察</param>
    /// <param name="catchMove">爪子</param>
    public virtual void StartGame(GameObject police, Transform catchMove) { }
    /// <summary>
    /// 获得当前局数
    /// </summary>
    public virtual int GetRound() { return 0; }
    /// <summary>
    /// 更新每局语音数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public virtual void UpdateRoundVoice() { }

    /// <summary>
    /// 更新特殊语音数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public virtual void UpdateSpecialVoice(VoiceType vtype) { }
    /// <summary>
    /// 获得特殊语音
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="vtype"></param>
    /// <param name="type">索引值</param>
    /// <returns></returns>
    public virtual ExtendContent GetSpecialVoice(VoiceType vtype, int type) { return null; }
    /// <summary>
    /// 通过index获得数据
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual ExtendContent GetVoiceContent(int index) { return null; }

    public virtual List<VoiceContent> GetVoiceContentBy(int statueType, int operType) { return null; }

    /// <summary>
    /// 没抓中
    /// </summary>
    /// <param name="cat"></param>
    /// <param name="voiceContent"></param>
    /// <param name="delytime"></param>
    /// <param name="contents"></param>
    public virtual void NoZhuaZhong(CatchTy cat, ExtendContent voiceContent, out float delytime, out string[] contents)
    {
        delytime = 0;
        contents = null;
    }
    /// <summary>
    /// 抖动 或掉落 降低难度提示
    /// </summary>
    /// <param name="prompt"></param>
    public virtual void DropShowPrompt(GameObject prompt,GameObject drop)
    {
    }
    /// <summary>
    /// 清空
    /// </summary>
    public virtual void Clear()
    {
        policeMove = null;
        policeShoot = null;
        playAction = null;
        KillTween();
    }

    #endregion
}
