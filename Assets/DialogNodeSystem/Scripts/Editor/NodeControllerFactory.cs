using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeControllerFactory
{
    public static NodeControllerComponent Build(NodeComponent node, Action<NodeControllerComponent> OnRemove, Action<NodeControllerComponent> OnSelect)
    {
        if (node is NodeDialog)
        {
            return new DialogNodeController((NodeDialog)node, OnRemove, OnSelect);
        }
        return null;
    }
}
