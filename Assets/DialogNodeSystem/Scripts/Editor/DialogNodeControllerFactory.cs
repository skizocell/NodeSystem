using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogNodeControllerFactory : NodeControllerFactory<DialogGraphController>
{
    public DialogNodeControllerFactory(DialogGraphController graphController) : base(graphController)
    {
    }

    public override NodeControllerComponent Build(NodeComponent node, Action<NodeControllerComponent> OnRemove, Action<NodeControllerComponent> OnSelect)
    {
        if (node is NodeDialog)
        {
            return new DialogNodeController(graphController, (NodeDialog)node, OnRemove, OnSelect);
        }
        return null;
    }
}
