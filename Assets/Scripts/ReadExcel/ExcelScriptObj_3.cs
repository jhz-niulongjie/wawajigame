using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public sealed class ExcelScriptObj_3 : ScriptableObject
{
    public List<VoiceContentType_3> voiceTypes;
}


[Serializable]
public sealed class VoiceContentType_3: ExtendContent
{
    //public string Id;
    //public string PayType;
    //public string Type;  
    //public string Time;
    //public VoiceContent Content;
    //public VoiceContent Winning;
    //public VoiceContent Winafter;
    //public VoiceContent NoDouDong;
    //public VoiceContent DouDong;
    //public VoiceContent ShootDrop;
    //public VoiceContent ShootDropWin;
}


[Serializable]
public sealed class VoiceContent
{
    public string Id;
    public string Type;
    public string Content;
    public string Time;
    public string[] Contents;
}

[Serializable]
public  class ExtendContent
{
    public string Id;
    public string PayType;
    public string Type;
    public string Time;
    public VoiceContent Content;
    public VoiceContent Winning;
    public VoiceContent Winafter;
    public VoiceContent NoDouDong;
    public VoiceContent DouDong;
    public VoiceContent ShootDrop;
    public VoiceContent ShootDropWin;
    public VoiceContent DouDong_4DD;
    public VoiceContent ShootDrop_3DD;
}