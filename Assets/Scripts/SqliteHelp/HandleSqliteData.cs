using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class HandleSqliteData
{
    private static SQLiteHelper sql;
    private GameCtr sdk;
    public const string recordTable = "record";
    public const string giftpartTable = "giftPart";
    // Use this for initialization
    public HandleSqliteData(GameCtr _sdk)
    {
        //创建名为sqlite4unity的数据库
        // sql = new SQLiteHelper();
        sdk = _sdk;
        //创建名为record的数据表
        //sql.CreateTable("record", new string[] { "RobotId", "OpenId", "ApplyRechargeId", "Status", "ReportTime" }, new string[] { "TEXT", "TEXT", "TEXT", "TEXT", "TEXT" });
        //创建名为giftPart的数据表
        // sql.CreateTable("giftPart", new string[] { "RobotId", "OpenId", "PartNum","RegTime" }, new string[] { "TEXT", "TEXT", "TEXT","TEXT" });
        // sql.CloseConnection();
        CreateTable();
    }

    /// <summary>
    /// 没表则创建
    /// </summary>
    public void CreateTable()
    {
        int num=TableIsExist("record");
        if (num == 0)
        {
            Debug.Log("record 表不存在 开始 创建");
            sql = new SQLiteHelper();
            sql.CreateTable("record", new string[] { "RobotId", "OpenId", "ApplyRechargeId", "Status", "ReportTime" }, new string[] { "TEXT", "TEXT", "TEXT", "TEXT", "TEXT" });
            sql.CloseConnection();
        }
        num = TableIsExist("giftPart");
        if (num == 0)
        {
            Debug.Log("giftPart 表不存在 开始 创建");
            sql = new SQLiteHelper();
            sql.CreateTable("giftPart", new string[] { "RobotId", "OpenId", "PartNum", "RegTime" }, new string[] { "TEXT", "TEXT", "TEXT", "TEXT" });
            sql.CloseConnection();
        }
    }
    //表是否存在 大于0存在
    private int TableIsExist(string _tablename)
    {
        sql = new SQLiteHelper();
        SqliteDataReader reader = sql.TableIsExist(_tablename);
        int num = 0;
        while (reader.Read())
        {
            num++;
        }
        sql.CloseConnection();
        return num;
    }
    #region 读取抓取记录
    public void InsertData()
    {
        var sql = new SQLiteHelper();
        string robotId = string.Format("{0}{1}{2}", "'",sdk.gameStatus.robotId, "'");
        string openId = string.Format("{0}{1}{2}", "'",sdk.gameStatus.openId, "'");
        string applyId = string.Format("{0}{1}{2}", "'",sdk.gameStatus.applyRechargeId, "'");
        string status = string.Format("{0}{1}{2}", "'",sdk.gameMode.lastRoundIsSuccess?1:0, "'");
        string reportTime = string.Format("{0}{1}{2}", "'",sdk.gameStatus.reportTime, "'");
        sql.InsertValues(recordTable, new string[] { robotId, openId, applyId, status, reportTime });
        sql.CloseConnection();
    }


    public void DeleteData(string tableName)
    {
        var sql = new SQLiteHelper();
        sql.ExecuteQuery("DELETE FROM " + tableName);
        sql.CloseConnection();
        Debug.Log(tableName + " 表数据已清空");
    }
    public List<C_RecordData> C_ReadData()
    {
        var sql = new SQLiteHelper();
        SqliteDataReader reader = sql.ReadFullTable(recordTable);
        C_RecordData rd = null;
        List<C_RecordData> list = new List<C_RecordData>();
        while (reader.Read())
        {
            rd = new C_RecordData();
            rd.robotId = reader.GetString(reader.GetOrdinal("RobotId"));
            rd.openId = reader.GetString(reader.GetOrdinal("OpenId"));
            rd.applyRechargeId = reader.GetString(reader.GetOrdinal("ApplyRechargeId"));
            rd.status = reader.GetString(reader.GetOrdinal("Status"));
            rd.reportTime = reader.GetString(reader.GetOrdinal("ReportTime"));
            list.Add(rd);
        }
        sql.CloseConnection();
        if (list.Count == 0) list = null;
        return list;
    }

    public List<C_RecordData> Q_ReadData()
    {
        var sql = new SQLiteHelper();
        SqliteDataReader reader = sql.ReadFullTable(recordTable);
        C_RecordData rd = null;
        List<C_RecordData> list = new List<C_RecordData>();
        string tmep = "ANS" + sdk.Q_startCarwTime;
        while (reader.Read())
        {
            rd = new C_RecordData();
            rd.robotId = reader.GetString(reader.GetOrdinal("RobotId"));
            rd.openId = tmep;
            rd.applyRechargeId = tmep;
            rd.status = reader.GetString(reader.GetOrdinal("Status"));
            rd.reportTime = reader.GetString(reader.GetOrdinal("ReportTime"));
            list.Add(rd);
        }
        sql.CloseConnection();
        if (list.Count == 0) list = null;
        return list;
    }
    #endregion

    #region 读取礼品碎片

    //读取碎片数量
    public string ReadDataGiftPart(GiftPartTable filedValue)
    {
        var sql = new SQLiteHelper();
        string openId = string.Format("{0}{1}{2}", "'",sdk.gameStatus.openId, "'");
        SqliteDataReader reader = sql.ReadTable(giftpartTable, new string[] { "*" }, new string[] { "OpenId" }, new string[] { "=" }, new string[] { openId });
        string num = "";
        if (reader.Read())
        {
            num = reader.GetString(reader.GetOrdinal(filedValue.ToString()));
        }
        sql.CloseConnection();
        return num;
    }
    //更新碎片数量
    public void UpdateGiftPart(int partNum)
    {
        string _partNum = ReadDataGiftPart(GiftPartTable.PartNum);
        if (string.IsNullOrEmpty(_partNum))//没数据
            InsertGiftPartData(partNum);
        else
        {
            string partNumStr = string.Format("{0}{1}{2}", "'",partNum, "'");
            string openId = string.Format("{0}{1}{2}", "'",sdk.gameStatus.openId, "'");
            var sql = new SQLiteHelper();
            sql.UpdateValues(giftpartTable, new string[] { "PartNum" }, new string[] { partNumStr }, "OpenId", "=",openId);
            sql.CloseConnection();
        }

    }

    public void InsertGiftPartData(int partNum)
    {
        var sql = new SQLiteHelper();
        string robotId = string.Format("{0}{1}{2}", "'",sdk.gameStatus.robotId, "'");
        string openId = string.Format("{0}{1}{2}", "'",sdk.gameStatus.openId, "'");
        string partNumStr = string.Format("{0}{1}{2}", "'",partNum, "'");
        string regTime = string.Format("{0}{1}{2}", "'",CommTool.GetTimeStamp(), "'");//时间戳
        sql.InsertValues(giftpartTable, new string[] { robotId, openId, partNumStr, regTime });
        sql.CloseConnection();
    }
    //
    private bool FindRegTime(string regTime)
    {
        if (!string.IsNullOrEmpty(regTime))//
        {
            DateTime reg=CommTool.GetTimeByStamp(regTime);
            DateTime now = DateTime.Now.ToLocalTime();
            TimeSpan span= now - reg;
            Debug.Log(" reg  TotalDays---" + span.TotalDays);
            if (span.TotalDays >= 3)
            {
                return true;
            }
        }
        return false;
    }

    //删除超过时间的用户信息
    public void DelOverTimeUserFromDataBase()
    {
        var sql = new SQLiteHelper();
        SqliteDataReader reader = sql.ReadFullTable(giftpartTable);
        List<string> openIdList = new List<string>();
        while (reader.Read())
        {
            string regTime= reader.GetString(reader.GetOrdinal(GiftPartTable.RegTime.ToString()));
            if (FindRegTime(regTime))
            {
                string openId= reader.GetString(reader.GetOrdinal(GiftPartTable.OpenId.ToString()));
                openId= string.Format("{0}{1}{2}", "'",openId, "'");
                openIdList.Add(openId);
            }
        }
        for (int i = 0; i < openIdList.Count; i++)
        {
            sql.DeleteValuesOR(giftpartTable, new string[] { "OpenId" }, new string[] { "=" }, new string[] {openIdList[i]});
            Debug.Log("用户：" + openIdList[i] + "已删除");
        }
        sql.CloseConnection();
    }
  
    //所有
    public List<string> FindAllGiftPartData()
    {
        var sql = new SQLiteHelper();
        SqliteDataReader reader = sql.ReadFullTable(giftpartTable);
        List<string> list = new List<string>();
        while (reader.Read())
        {
            list.Add(reader.GetString(reader.GetOrdinal("OpenId")));
        }
        sql.CloseConnection();
        return list;
    }
    #endregion
}
