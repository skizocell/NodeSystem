using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NodeControllerFactory<G> : INodeControllerFactory where G : GraphControllerBase
{
    protected G graphController;

    public NodeControllerFactory(G graphController)
    {
        this.graphController = graphController;
    }

    public abstract NodeControllerComponent Build(NodeComponent node, Action<NodeControllerComponent> OnRemove, Action<NodeControllerComponent> OnSelect);
}
