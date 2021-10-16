using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeLink
{
    public NodeComponent from;
    public NodeComponent to;
    public string fromPinId;
    public string toPinId;
}
