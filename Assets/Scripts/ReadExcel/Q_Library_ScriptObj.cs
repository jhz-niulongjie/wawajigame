using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//考题实体

public sealed class Q_Library_ScriptObj : ScriptableObject
{
    public List<Q_Question> question_list;
}

[Serializable]
public sealed class Q_Question
{
    public string question;
    public string rightAnswer;
    public string wrongAnswer;
    public int right_wing;  //0 是左翅膀  1 是右翅膀
    public Q_Question() { }
    public Q_Question(string _content, string _right, string _error)
    {
        question = _content;
        rightAnswer = _right;
        wrongAnswer = _error;
        right_wing = 0;
    }
}
