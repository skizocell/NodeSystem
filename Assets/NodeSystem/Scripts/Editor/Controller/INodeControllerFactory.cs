using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INodeControllerFactory
{
    NodeControllerComponent Build(NodeComponent node, Action<NodeControllerComponent> OnRemove, Action<NodeControllerComponent> OnSelect);
}
