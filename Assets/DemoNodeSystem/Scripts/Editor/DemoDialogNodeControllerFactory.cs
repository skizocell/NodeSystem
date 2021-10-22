using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoDialogNodeControllerFactory : NodeControllerFactory<DemoDialogGraphController>
{
    public DemoDialogNodeControllerFactory(DemoDialogGraphController graphController) : base(graphController)
    {
    }

    public override NodeControllerComponent Build(NodeComponent node)
    {
        if (node is DemoNodeDialog)
        {
            return new DemoDialogNodeController(graphController, (DemoNodeDialog)node);
        }
        return null;
    }
}
