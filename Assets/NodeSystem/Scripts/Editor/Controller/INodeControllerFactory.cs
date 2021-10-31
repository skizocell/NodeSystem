using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INodeControllerFactory
{
    NodeControllerComponent BuildNodeControllerComponent(NodeComponent node);
}
