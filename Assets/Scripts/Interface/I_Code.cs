using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Video;


//支付接口
public interface I_Code   {

    GameObject anim { get; set; }
    GameObject code { get; set; }
    VideoPlayer vplayer { get; set; }
    GameObject loading { get; set; }
    RawImage rawImage { get; set; }
    Text money_text { get; set; }
    Text times_text { get; set; }
    //支付语音数据
    List<VoiceContent> vc_list { get; set; }

    void InitCodeData();
    //播放视频
    void PlayMovie();
    //视频播放完成
    void MovieOver(VideoPlayer p);
     //获得二维码数据
    void GetCodeData();
    //初始金钱
    void InitMoneyData();
    //二维码获得成功
    void GetQRCodeSuccess();
    //播放支付语音
    IEnumerator PlayVoiceIe();
}
