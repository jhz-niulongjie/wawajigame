using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Excel;
using System;
//using OfficeOpenXml;
using System.IO;

public sealed class ExcelAccess
{
    private static string[] SheetNames = { "Sheet1" };

    public static List<VoiceContentType_3> ReadType_3()
    {
        DataRowCollection collect = ReadExcel(EditorTool.VoiceType_3, SheetNames[0]);
        List<VoiceContentType_3> list = new List<VoiceContentType_3>();
        List<VoiceContent> listContent = ReadContent(EditorTool.VoiceContent_3);
        for (int i = 1; i < collect.Count; i++)
        {
            if (i > 1)
            {  
                if (collect[i][0].ToString() == "") continue;
                VoiceContentType_3 e = new VoiceContentType_3();
                e.Id = collect[i][0].ToString();
                e.PayType = collect[i][1].ToString();
                e.Type = collect[i][2].ToString();
                e.Time = collect[i][3].ToString();
                e.Content = listContent.Find(c => c.Type == collect[i][4].ToString());
                e.Winning = listContent.Find(c => c.Type == collect[i][5].ToString());
                e.Winafter = listContent.Find(c => c.Type == collect[i][6].ToString());
                e.NoDouDong = listContent.Find(c => c.Type == collect[i][7].ToString());
                e.DouDong = listContent.Find(c => c.Type == collect[i][8].ToString());
                e.ShootDrop = listContent.Find(c => c.Type == collect[i][9].ToString());
                e.ShootDropWin = listContent.Find(c => c.Type == collect[i][10].ToString());
                list.Add(e);
            }

        }
        return list;
    }

    public static List<VoiceContentType_5> ReadType_5()
    {
        DataRowCollection collect = ReadExcel(EditorTool.VoiceType_5, SheetNames[0]);
        List<VoiceContentType_5> list = new List<VoiceContentType_5>();
        List<VoiceContent> listContent = ReadContent(EditorTool.VoiceContent_5);
        for (int i = 1; i < collect.Count; i++)
        {
            if (i > 1)
            {
                if (collect[i][0].ToString() == "") continue;
                VoiceContentType_5 e = new VoiceContentType_5();
                e.Id = collect[i][0].ToString();
                e.PayType = collect[i][1].ToString();
                e.Type = collect[i][2].ToString();
                e.Time = collect[i][3].ToString();
                e.Content = listContent.Find(c => c.Type == collect[i][4].ToString());
                e.Winning = listContent.Find(c => c.Type == collect[i][5].ToString());
                e.Winafter = listContent.Find(c => c.Type == collect[i][6].ToString());
                e.NoDouDong = listContent.Find(c => c.Type == collect[i][7].ToString());
                e.DouDong = listContent.Find(c => c.Type == collect[i][8].ToString());
                e.DouDong_4DD = listContent.Find(c => c.Type == collect[i][9].ToString());
                e.ShootDrop = listContent.Find(c => c.Type == collect[i][10].ToString());
                e.ShootDrop_3DD = listContent.Find(c => c.Type == collect[i][11].ToString());
                list.Add(e);
            }

        }
        return list;
    }


   public static List<VoiceContent> ReadContent(string contentType)
    {
        DataRowCollection collect = ExcelAccess.ReadExcel(contentType, SheetNames[0]);
        List<VoiceContent> list = new List<VoiceContent>();
        for (int i = 0; i < collect.Count; i++)
        {
            if (i==0||collect[i][0].ToString() == "") continue;
            VoiceContent vc = new VoiceContent
            {
                 Id= collect[i][0].ToString(),
                Type = collect[i][1].ToString(),
                Content = collect[i][2].ToString(),
                Time = collect[i][3].ToString(),
            };
            list.Add(vc);
        }
        return list;
    }

   public static List<Q_Question> Q_ReadContent()
    {
        DataRowCollection collect = ExcelAccess.ReadExcel(EditorTool.QuestionLibrary, SheetNames[0]);
        List<Q_Question> list = new List<Q_Question>();
        for (int i = 0; i < collect.Count; i++)
        {
            if (i==0||collect[i][0].ToString() == "") continue;
            Q_Question vc = new Q_Question
            {
                question = collect[i][1].ToString(),
                rightAnswer = collect[i][2].ToString(),
                wrongAnswer = collect[i][3].ToString(),
            };
            list.Add(vc);
        }
        return list;
    }


   


    public static List<VoiceContent> ReadContentByName(string excelName)
    {
        DataRowCollection collect = ExcelAccess.ReadExcel(excelName, SheetNames[0]);
        List<VoiceContent> list = new List<VoiceContent>();
        for (int i = 0; i < collect.Count; i++)
        {
            if (i == 0 || collect[i][0].ToString() == "") continue;
            VoiceContent vc = new VoiceContent
            {
                Id = collect[i][1].ToString(),
                Type = collect[i][2].ToString(),
                Time = collect[i][3].ToString(),
                Content = collect[i][4].ToString(),
            };
            list.Add(vc);
        }
        return list;
    }


    static DataRowCollection ReadExcel(string name, string sheet)
    {
        FileStream fs = File.Open(FilePath(name), FileMode.Open, FileAccess.Read, FileShare.Read);
        IExcelDataReader ereader = ExcelReaderFactory.CreateOpenXmlReader(fs);
        DataSet result = ereader.AsDataSet();
        //var table = result.Tables[sheet];
        //var list = result.Tables[sheet].Columns;
        return result.Tables[sheet].Rows;
    }

    public static string FilePath(string name)
    {
        return Application.dataPath + "/" + name;
    }














}
