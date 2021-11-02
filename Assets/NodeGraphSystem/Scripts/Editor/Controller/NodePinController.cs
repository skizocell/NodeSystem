using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class NodePinController
{
    public enum NodePinType { Emiter, Receiver }

    private float yOffset;

    protected Rect rect;
    protected abstract Texture2D GetButtonTexture();
    protected abstract Rect GetButtonRect(Rect nodeRect, float yOffset);
    protected abstract bool IsCompatibleWith(NodePinController pin);

    public NodePinType type;
    public NodeLink.LinkType generateLinkType;
    public NodeControllerComponent linkedNodeConroller;
    public string nodePinId;//id can also be the method name depending of used method
    public bool canHaveManyLink;
    public bool isConnected;

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
        isConnected = false;
    }

    public Rect GetRect()
    {
        return rect;
    }

    public bool CanConectTo(NodePinController pin)
    {
        return this.type != pin.type 
            && (pin.canHaveManyLink ? true : !pin.isConnected)
            && IsCompatibleWith(pin);
    }
}

public class NodePinCallerController : NodePinController
{
    protected static Texture2D buttonTexture; //Pin Texture for pin button
    private const float WIDTH=18;
    private const float HEIGHT=18;
    private const float XOFFSET=-4;

    public NodePinCallerController(string methodName, NodeControllerComponent node, bool canHaveManyLink, float yOffset) : base(methodName, node, canHaveManyLink, yOffset)
    {
        type = NodePinType.Emiter;
        generateLinkType = NodeLink.LinkType.Call;
    }

    protected override Rect GetButtonRect(Rect nodeRect, float yOffset)
    {
        rect = new Rect(nodeRect.x + nodeRect.width + XOFFSET, nodeRect.y + yOffset, WIDTH, HEIGHT);
        return rect;
    }

    protected override Texture2D GetButtonTexture()
    {
        if (buttonTexture == null) buttonTexture = Resources.Load<Texture2D>("Textures/Editor/arrow");
        return buttonTexture;
    }

    protected override bool IsCompatibleWith(NodePinController pin)
    {
        return pin.generateLinkType == NodeLink.LinkType.Call;
    }
}

public class NodePinCalledController : NodePinController
{
    protected static Texture2D buttonTexture; //Pin Texture for pin button
    private const float WIDTH = 18;
    private const float HEIGHT = 18;
    private const float XOFFSET = -13;

    public NodePinCalledController(string methodName, NodeControllerComponent node, bool canHaveManyLink, float yOffset) : base(methodName, node, canHaveManyLink, yOffset)
    {
        type = NodePinType.Receiver;
        generateLinkType = NodeLink.LinkType.Call;
    }

    protected override Rect GetButtonRect(Rect nodeRect, float yOffset)
    {
        rect = new Rect(nodeRect.x + XOFFSET, nodeRect.y + yOffset, WIDTH, HEIGHT);
        return rect;
    }

    protected override Texture2D GetButtonTexture()
    {
        if (buttonTexture == null) buttonTexture = Resources.Load<Texture2D>("Textures/Editor/target");
        return buttonTexture;
    }

    protected override bool IsCompatibleWith(NodePinController pin)
    {
        return pin.generateLinkType == NodeLink.LinkType.Call;
    }
}

public abstract class NodePinDataTransfertController : NodePinController
{
    public Type transfertDataType;
    public NodePinDataTransfertController(string id, NodeControllerComponent linkedNode, bool canHaveManyLink, float yOffset, Type transfertDataType) : base(id, linkedNode, canHaveManyLink, yOffset)
    {
        this.transfertDataType = transfertDataType;
    }

    protected override bool IsCompatibleWith(NodePinController pin)
    {
        if (pin is NodePinDataTransfertController)
        {
            NodePinDataTransfertController other = (NodePinDataTransfertController)pin;
            return other.generateLinkType == NodeLink.LinkType.Set 
                && other.transfertDataType == this.transfertDataType;
        }
        else return false;
    }
}

public class NodePinSetterController : NodePinDataTransfertController
{
    protected static Texture2D buttonTexture; //Pin Texture for pin button
    private const float WIDTH = 24;
    private const float HEIGHT = 24;
    private const float XOFFSET = -9;

    public NodePinSetterController(string methodName, NodeControllerComponent node, bool canHaveManyLink, float yOffset, Type transfertDataType) : base(methodName, node, canHaveManyLink, yOffset, transfertDataType)
    {
        type = NodePinType.Emiter;
        generateLinkType = NodeLink.LinkType.Set;
    }

    protected override Rect GetButtonRect(Rect nodeRect, float yOffset)
    {
        rect = new Rect(nodeRect.x + nodeRect.width + XOFFSET, nodeRect.y + yOffset, WIDTH, HEIGHT);
        return rect;
    }

    protected override Texture2D GetButtonTexture()
    {
        if (buttonTexture == null) buttonTexture = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Texture2D>("UI/Skin/Knob.psd");
        return buttonTexture;
    }
}

public class NodePinGetterController : NodePinDataTransfertController
{
    protected static Texture2D buttonTexture; //Pin Texture for pin button
    private const float WIDTH = 24;
    private const float HEIGHT = 24;
    private const float XOFFSET = -14;

    public NodePinGetterController(string methodName, NodeControllerComponent node, bool canHaveManyLink, float yOffset, Type transfertDataType) : base(methodName, node, canHaveManyLink, yOffset, transfertDataType)
    {
        generateLinkType = NodeLink.LinkType.Set;
        type = NodePinType.Receiver;
    }

    protected override Rect GetButtonRect(Rect nodeRect, float yOffset)
    {
        rect = new Rect(nodeRect.x + XOFFSET, nodeRect.y + yOffset, WIDTH, HEIGHT);
        return rect;
    }

    protected override Texture2D GetButtonTexture()
    {
        if (buttonTexture == null) buttonTexture = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Texture2D>("UI/Skin/Knob.psd");
        return buttonTexture;
    }
}
