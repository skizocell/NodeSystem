using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DSGame.GraphSystem
{
    //Node Controller Factory
    public class NodeControllerFactory
    {
        protected GraphControllerBase graphController;

        public NodeControllerFactory(GraphControllerBase graphController)
        {
            this.graphController = graphController;
        }
    
        //Dynamicaly get the good controller for the node provided
        public NodeControllerComponent BuildNodeControllerComponent<N>(N node) where N : Node
        {
            Type nodeType = node.GetType();
            Type graphControllerType = graphController.GetType();
            foreach (Type type in System.Reflection.Assembly.GetExecutingAssembly()
                                    .GetTypes().Where(type => typeof(NodeControllerComponent)
                                    .IsAssignableFrom(type) && type.IsClass && !type.IsAbstract))
            {
                if (type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(NodeControllerBase<>))
                {
                    Type[] typeParameters = type.BaseType.GetGenericArguments();
                    if (typeParameters[0] == node.GetType())
                    {
                        return (NodeControllerComponent)Activator.CreateInstance(type, new System.Object[] { graphController, node });
                    }
                }
            }
            //If no custom Controller found return the generic one
            return new NodeControllerGeneric<N>(graphController, node); 
        }
    }
}
