using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class NodePinController
{
    public enum Type { Emiter, Receiver }
    public Type type;

    public NodeControllerComponent linkedNodeConroller;
    public string nodePinId;//id can also be the method name depending of used method

    public bool canHaveManyLink;

    protected Rect rect;
    protected abstract Texture2D GetButtonTexture();
    protected abstract Rect GetButtonRect(Rect nodeRect, float yOffset);

    private float yOffset;

    public NodePinController(string id, NodeControllerComponent linkedNode, bool canHaveManyLink, float yOffset)
    {
        this.nodePinId = id;
        this.linkedNodeConroller = linkedNode;
        this.canHaveManyLink = canHaveManyLink;
        this.yOffset = yOffset;
    }

    public string GetNodePinId()
    {
        return nodePinId;
    }

    public void Draw(Rect nodeRect)
    {
        if (GUI.Button(GetButtonRect(nodeRect, yOffset), GetButtonTexture(), GUIStyle.none))
        {
            linkedNodeConroller.OnClickNodePin(this);
        }
    }

    public Rect GetRect()
    {
        return rect;
    }
}

public class NodePinCallerController : NodePinController
{
    private const float WIDTH=18;
    private const float HEIGHT=18;
    private const float XOFFSET=-4;

    //Pin Texture for pin button
    protected static Texture2D buttonTexture;

    public NodePinCallerController(string methodName, NodeControllerComponent node, bool canHaveManyLink, float yOffset) : base(methodName, node, canHaveManyLink, yOffset)
    {
        type = Type.Emiter;
        if (buttonTexture == null)
        {
            //Pin Icone Texture loading
            buttonTexture = Resources.Load<Texture2D>("Textures/Editor/arrow");
        }
    }

    protected override Rect GetButtonRect(Rect nodeRect, float yOffset)
    {
        rect = new Rect(nodeRect.x + nodeRect.width + XOFFSET, nodeRect.y + yOffset, WIDTH, HEIGHT);
        return rect;
    }

    protected override Texture2D GetButtonTexture()
    {
        return buttonTexture;
    }
}

public class NodePinCalledController : NodePinController
{
    private const float WIDTH = 18;
    private const float HEIGHT = 18;
    private const float XOFFSET = -13;

    //Pin Texture for pin button
    protected static Texture2D buttonTexture;

    public NodePinCalledController(string methodName, NodeControllerComponent node, bool canHaveManyLink, float yOffset) : base(methodName, node, canHaveManyLink, yOffset)
    {
        type = Type.Receiver;
        if (buttonTexture == null)
        {
            //Pin Icone Texture loading
            buttonTexture = Resources.Load<Texture2D>("Textures/Editor/target");
        }
    }

    protected override Rect GetButtonRect(Rect nodeRect, float yOffset)
    {
        rect = new Rect(nodeRect.x + XOFFSET, nodeRect.y + yOffset, WIDTH, HEIGHT);
        return rect;
    }

    protected override Texture2D GetButtonTexture()
    {
        return buttonTexture;
    }
}


//    //No Necessary to link with nodepin NodePin destined to be erased
//    public abstract class NodePinController<T> : INodePinController where T : NodePin
//{
//    string methodName;
//    protected Rect rect;
//    protected T nodePin;

//    public NodePinController(T nodePin, string methodName)
//    {
//        this.nodePin = nodePin;
//        this.methodName = methodName;
//    }

//    public string GetMethodName()
//    {
//        return methodName;
//    }

//    protected abstract Texture2D GetButtonTexture();
//    protected abstract Rect GetButtonRect(Rect nodeRect, float yOffset);
//    //protected abstract void Action();

//    //public abstract bool Link(NodePin pinTarget);
//    public NodePin GetNodePin()
//    {
//        return nodePin;
//    }

//    public void Draw(Rect nodeRect, float yOffset)
//    {
//        if (GUI.Button(GetButtonRect(nodeRect, yOffset), GetButtonTexture(), GUIStyle.none))
//        {
//            //Action();
//        }
//    }

//    public Rect GetRect()
//    {
//        return rect;
//    }
//}
