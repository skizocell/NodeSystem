using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeProcessStatus { Waiting, Running, Done }

[Serializable]
public abstract class NodeComponent : ScriptableObject
{
    public Rect rect;
    [NonSerialized]
    public NodeProcessStatus processStatus;

    public abstract void Process();
}

