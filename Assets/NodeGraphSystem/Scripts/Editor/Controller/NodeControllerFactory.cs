using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeControllerFactory
{
    protected GraphControllerBase graphController;

    public NodeControllerFactory(GraphControllerBase graphController)
    {
        this.graphController = graphController;
    }

    public NodeControllerComponent BuildNodeControllerComponent(NodeComponent node)
    {
        Type nodeType = node.GetType();
        Type graphControllerType = graphController.GetType();
        foreach (Type type in System.Reflection.Assembly.GetExecutingAssembly()
                                .GetTypes().Where(type => typeof(NodeControllerComponent)
                                .IsAssignableFrom(type) && type.IsClass && !type.IsAbstract))
        {
            if (type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(NodeControllerBase<,>))
            {
                Type[] typeParameters = type.BaseType.GetGenericArguments();
                if (typeParameters[0] == node.GetType() && typeParameters[1] == graphController.GetType())
                {
                    return (NodeControllerComponent)Activator.CreateInstance(type, new System.Object[] { graphController, node });
                }
            }
        }
        return null;
    }
}
