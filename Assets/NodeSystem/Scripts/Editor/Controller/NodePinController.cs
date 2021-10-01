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

    protected override void Action()
    {
        Debug.Log("TEST Button caller");
    }

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

    protected override void Action()
    {
        Debug.Log("TEST Button called");
    }

    protected override Rect GetButtonRect(Rect nodeRect, float yOffset)
    {
        return new Rect(nodeRect.x + XOFFSET, nodeRect.y + yOffset, WIDTH, HEIGHT);
    }

    protected override Texture2D GetButtonTexture()
    {
        return buttonTexture;
    }
}

public abstract class NodePinController<T> where T : NodePin
{
    private T nodePin;

    public NodePinController(T nodePin)
    {
        this.nodePin = nodePin;
    }

    protected abstract Texture2D GetButtonTexture();
    protected abstract Rect GetButtonRect(Rect nodeRect, float yOffset);
    protected abstract void Action();

    public void Draw(Rect nodeRect, float yOffset)
    {
        if (GUI.Button(GetButtonRect(nodeRect, yOffset), GetButtonTexture(), GUIStyle.none))
        {
            Action();
        }
    }
}
