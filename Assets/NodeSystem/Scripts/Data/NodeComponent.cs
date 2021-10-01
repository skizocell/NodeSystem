using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class NodeComponent : ScriptableObject
{
    public Rect rect;
    public abstract void Init();
}
