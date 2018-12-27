using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System;

public class EditorTool
{

    //asset name
    const string threeVoice_Path = "Assets/Resources/voiceThreeNames.asset";
    const string fiveVoice_Path = "Assets/Resources/voiceFiveNames.asset";
    const string question_l_Path = "Assets/Resources/QuestionLibrary.asset";
    const string question_v_Path = "Assets/Resources/QuestionVoice.asset";
    const string give_up_on_Game_Path = "Assets/Resources/Give_Up_On_Game.asset";
    const string jokeVoice_Path = "Assets/Resources/jokeVoice.asset";
    const string luckyTurn_Path = "Assets/Resources/luckyTurn.asset";
    const string bigbom_Path = "Assets/Resources/bigbom.asset";


    //xlsx name
    public const string VoiceContent_3 = "VoiceContent_3.xlsx";
    public const string VoiceType_3 = "VoiceType_3.xlsx";
    public const string VoiceContent_5 = "VoiceContent_5.xlsx";
    public const string VoiceType_5 = "VoiceType_5.xlsx";
    public const string QuestionLibrary = "QuestionLibrary.xlsx";
    public const string QustionVoice = "VoiceContent_Question.xlsx";
    public const string Give_Up_On_Game = "Give_Up_On_Game.xlsx";
    public const string Joke_Voice = "JokeVoice.xlsx";
    public const string LuckyTurnTurn = "LuckyTurnTurn.xlsx";
    public const string BigBom = "BigBom.xlsx";

    #region  设置animator 动画片段播放速度
    static string path = "Assets/Animation/fishhook.controller";
    static AnimatorController animator = AssetDatabase.LoadAssetAtPath(path, typeof(AnimatorController)) as AnimatorController;

    [MenuItem("Tools/ChangeAnimSpeed/fishhook_down")]
    static void fishhook_down()
    {
        SetAniamtionSpeed(animator, AnimationName.down, 0.5f);
        SetAniamtionSpeed(animator, AnimationName.up, 0.5f);
    }
    [MenuItem("Tools/ChangeAnimSpeed/fishhook_get")]
    static void fishhook_get()
    {
        SetAniamtionSpeed(animator, AnimationName.catchs, 1f);
        SetAniamtionSpeed(animator, AnimationName.release, 2f);
    }

    static void SetAniamtionSpeed(AnimatorController ac, AnimationName name, float speed)
    {
        //AnimatorController ac = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
        AnimatorControllerLayer[] layers = ac.layers;
        AnimatorStateMachine state = layers[0].stateMachine;
        ChildAnimatorState[] sts = state.states;
        for (int i = 0; i < sts.Length; i++)
        {
            if (name.ToString() == sts[i].state.name)
            {
                sts[i].state.speed = speed;
                Debug.Log(name + "------修改成功");
                break;
            }
        }
    }

    #endregion

    #region 制作语音数据资源
    [MenuItem("Tools/BuildAssetScripteObj")]
    static void BuildScriptObjAssets()
    {
        BuildScripteObjBysExcell_3();
        BuildScripteObjBysExcell_5();
        BuildScriptObj_Question_Library_Default();
        BuildScriptObj_Question_Voice();
        BuildScriptObj_JokeVoice();
        BuildScriptObj_LuckyTurnTurn();
        BuildScriptObj_BigBom();
       // BuildScriptObj_GiveUpGame();
    }

    //打包语音数据包
    //[MenuItem("Tools/BuildAssetScripteObj/threeRound")]
    static void BuildScripteObjBysExcell_3()
    {
        ExcelScriptObj_3 es = ScriptableObject.CreateInstance<ExcelScriptObj_3>();
        es.voiceTypes = ExcelAccess.ReadType_3();
        if (File.Exists(threeVoice_Path))
            File.Delete(threeVoice_Path);
        AssetDatabase.CreateAsset(es, threeVoice_Path);
        AssetDatabase.SaveAssets();
        //EditorUtility.FocusProjectWindow();
        Selection.activeObject = es;
        Debug.Log("Build ScripteObj_Audio_3 Success");
        AssetDatabase.Refresh();
    }

    //打包语音数据包
    //[MenuItem("Tools/BuildAssetScripteObj/fiveRound")]
    static void BuildScripteObjBysExcell_5()
    {
        ExcelScriptObj_5 es = ScriptableObject.CreateInstance<ExcelScriptObj_5>();
        es.voiceTypes = ExcelAccess.ReadType_5();
        if (File.Exists(fiveVoice_Path))
            File.Delete(fiveVoice_Path);
        AssetDatabase.CreateAsset(es, fiveVoice_Path);
        AssetDatabase.SaveAssets();
        //EditorUtility.FocusProjectWindow();
        Selection.activeObject = es;
        Debug.Log("Build ScripteObj_Audio_5 Success");
        AssetDatabase.Refresh();
    }

    //[MenuItem("Tools/BuildAssetScripteObj/Question_Library_Default")]
    static void BuildScriptObj_Question_Library_Default()
    {
        Q_Library_ScriptObj qs = ScriptableObject.CreateInstance<Q_Library_ScriptObj>();
        qs.question_list = ExcelAccess.Q_ReadContent();
        if (File.Exists(question_l_Path))
            File.Delete(question_l_Path);
        AssetDatabase.CreateAsset(qs, question_l_Path);
        AssetDatabase.SaveAssets();
        //EditorUtility.FocusProjectWindow();
        Selection.activeObject = qs;
        Debug.Log("Build ScripteObj_Question_Library_Default  Success");
        AssetDatabase.Refresh();

    }

    //[MenuItem("Tools/BuildAssetScripteObj/Question_Voice")]
    static void BuildScriptObj_Question_Voice()
    {
        Q_Voice_ScriptObj q_v_s = ScriptableObject.CreateInstance<Q_Voice_ScriptObj>();
        q_v_s.q_v_list = ExcelAccess.ReadContent(QustionVoice);
        if (File.Exists(question_v_Path))
            File.Delete(question_v_Path);
        AssetDatabase.CreateAsset(q_v_s, question_v_Path);
        AssetDatabase.SaveAssets();
        Selection.activeObject = q_v_s;
        Debug.Log("Build ScripteObj_Question_Voice  Success");
        AssetDatabase.Refresh();

    }

    //[MenuItem("Tools/BuildAssetScripteObj/GiveUpGame")]
    static void BuildScriptObj_GiveUpGame()
    {
        Q_Voice_ScriptObj q_v_s = ScriptableObject.CreateInstance<Q_Voice_ScriptObj>();
        q_v_s.q_v_list = ExcelAccess.ReadContent(Give_Up_On_Game);
        if (File.Exists(give_up_on_Game_Path))
            File.Delete(give_up_on_Game_Path);
        AssetDatabase.CreateAsset(q_v_s, give_up_on_Game_Path);
        AssetDatabase.SaveAssets();
        Selection.activeObject = q_v_s;
        Debug.Log("Build ScripteObj_GiveUpGame Success");
        AssetDatabase.Refresh();
    }

    static void BuildScriptObj_JokeVoice()
    {
        LuckyTurn jkv = ScriptableObject.CreateInstance<LuckyTurn>();
        jkv.luckyTurnList = ExcelAccess.ReadContent(Joke_Voice);
        if (File.Exists(jokeVoice_Path))
            File.Delete(jokeVoice_Path);
        AssetDatabase.CreateAsset(jkv, jokeVoice_Path);
        AssetDatabase.SaveAssets();
        Selection.activeObject = jkv;
        Debug.Log("Build ScripteObj_JokeVoice Success");
        AssetDatabase.Refresh();
    }

    static void BuildScriptObj_LuckyTurnTurn()
    {
        LuckyTurn lkt = ScriptableObject.CreateInstance<LuckyTurn>();
        lkt.luckyTurnList = ExcelAccess.ReadLuckyTurnContent();
        if (File.Exists(luckyTurn_Path))
            File.Delete(luckyTurn_Path);
        AssetDatabase.CreateAsset(lkt, luckyTurn_Path);
        AssetDatabase.SaveAssets();
        Selection.activeObject = lkt;
        Debug.Log("Build ScripteObj_LuckyTurnTurn Success");
        AssetDatabase.Refresh();
    }

    static void BuildScriptObj_BigBom()
    {
        LuckyTurn lkt = ScriptableObject.CreateInstance<LuckyTurn>();
        lkt.luckyTurnList = ExcelAccess.ReadBigBomContent();
        if (File.Exists(bigbom_Path))
            File.Delete(bigbom_Path);
        AssetDatabase.CreateAsset(lkt, bigbom_Path);
        AssetDatabase.SaveAssets();
        Selection.activeObject = lkt;
        Debug.Log("Build ScripteObj_BigBom Success");
        AssetDatabase.Refresh();
    }

    #endregion


    //打包android工程
    static void BuildProject()
    {
        PlayerSettings.bundleVersion = DateTime.Now.ToString("yyyyMMdd-HHmmss");//生成版本号
        string android_project_path = @"C:\Users\jhz\Desktop\UnityBao";
        BuildTarget target = BuildTarget.Android;
         string[] buildScences = new[] { "Assets/Scenes/Main.unity"};
        BuildPipeline.BuildPlayer(buildScences, android_project_path, target, 
            BuildOptions.AcceptExternalModificationsToPlayer);
    }

}
