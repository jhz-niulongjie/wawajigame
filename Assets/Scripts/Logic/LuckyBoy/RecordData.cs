using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 记录上报失败
/// </summary>
public sealed class C_RecordData {

    public string status { get; set; }
    public string applyRechargeId { get; set; }//账单号
    public string openId { get; set; }//openId
    public string robotId { get; set; }//小胖Id
    public string reportTime { get; set; }//抓取时间戳
}


