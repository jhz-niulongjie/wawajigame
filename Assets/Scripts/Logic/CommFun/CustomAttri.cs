using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field)]
public sealed class CustomAttri : Attribute {

    public CustomAttri(string _content)
    {
        this.Content = _content;
    }
    public string Content { get; private set; }
}
