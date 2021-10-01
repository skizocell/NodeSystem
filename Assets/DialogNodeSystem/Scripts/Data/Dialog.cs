using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Dialog
{
    public string text="";

    public string GetFirstLine()
    {
        int carriageReturnIndex = text.IndexOf("\n");
        if (carriageReturnIndex > 0)
            return text.Substring(0, carriageReturnIndex);
        else
            return text;
    }
}
