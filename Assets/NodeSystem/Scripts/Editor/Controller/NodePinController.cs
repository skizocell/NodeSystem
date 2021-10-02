using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodePinCallerController : NodePinController<NodePinCaller>
{
    private const float WIDTH=18;
    private const float HEIGHT=18;
    private const float XOFFSET=-4;

    //Pin Texture for pin button
    protected static Texture2D buttonTexture;

    public NodePinCallerController(NodePinCaller nodePin) : base(nodePin)
    {
        if (buttonTexture == null)
        {
            //Pin Icone Texture loading
            buttonTexture = Resources.Load<Texture2D>("Textures/Editor/arrow");
        }
    }

    //public override bool Link(NodePin pinTarget)
    //{
    //    if (nodePin.GetType() == typeof(NodePinCalled))
    //    {
    //        nodePin.SetTarget((NodePinCalled) pinTarget);
    //        return true;
    //    }
    //    return false;
    //}

    //protected override void Action()
    //{
    //    Debug.Log("TEST Button caller");
    //}

    protected override Rect GetButtonRect(Rect nodeRect, float yOffset)
    {
        return new Rect(nodeRect.x + nodeRect.width + XOFFSET, nodeRect.y + yOffset, WIDTH, HEIGHT);
    }

    protected override Texture2D GetButtonTexture()
    {
        return buttonTexture;
    }
}

public class NodePinCalledController : NodePinController<NodePinCalled>
{
    private const float WIDTH = 18;
    private const float HEIGHT = 18;
    private const float XOFFSET = -13;

    //Pin Texture for pin button
    protected static Texture2D buttonTexture;

    public NodePinCalledController(NodePinCalled nodePin) : base(nodePin)
    {
        if (buttonTexture == null)
        {
            //Pin Icone Texture loading
            buttonTexture = Resources.Load<Texture2D>("Textures/Editor/target");
        }
    }

    //public override bool Link(NodePin pinTarget)
    //{

    //}

    //protected override void Action()
    //{
    //    Debug.Log("TEST Button called");
    //}

    protected override Rect GetButtonRect(Rect nodeRect, float yOffset)
    {
        return new Rect(nodeRect.x + XOFFSET, nodeRect.y + yOffset, WIDTH, HEIGHT);
    }

    protected override Texture2D GetButtonTexture()
    {
        return buttonTexture;
    }
}

public interface INodePinController
{
    void Draw(Rect nodeRect, float yOffset);
    NodePin GetNodePin();
}

public abstract class NodePinController<T> : INodePinController where T : NodePin
{
    protected T nodePin;

    public NodePinController(T nodePin)
    {
        this.nodePin = nodePin;
    }

    protected abstract Texture2D GetButtonTexture();
    protected abstract Rect GetButtonRect(Rect nodeRect, float yOffset);
    //protected abstract void Action();

    //public abstract bool Link(NodePin pinTarget);
    public NodePin GetNodePin()
    {
        return nodePin;
    }

    public void Draw(Rect nodeRect, float yOffset)
    {
        if (GUI.Button(GetButtonRect(nodeRect, yOffset), GetButtonTexture(), GUIStyle.none))
        {
            //Action();
        }
    }
}
