using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class NodeControllerBase<T> : NodeControllerComponent where T : NodeComponent
{
    #region variable
    //The windows id for the current node
    protected int nodeWindowsDrawId;

    //The node
    protected T node;

    //Pin Texture for pin button
    protected static Texture2D pinDataButtonTexture;
    protected static Texture2D pinFlowButtonTexture;
    protected static Texture2D pinFlowTargetButtonTexture;

    //Selected header for windows
    protected static GUIStyle headerSelectedStyle;
    protected static Texture2D headerSelectedColor;

    //Action todo when a node is removed or selected
    public Action<NodeControllerComponent> OnRemoveNode;
    public Action<NodeControllerComponent> OnSelect;
    #endregion

    public NodeControllerBase(T node, Action<NodeControllerComponent> OnClickRemoveNode, Action<NodeControllerComponent> OnSelect)
    {
        this.node = node;
        this.OnRemoveNode = OnClickRemoveNode;
        this.OnSelect = OnSelect;

        //Init when style is null
        if (headerSelectedStyle==null)
        {
            //Selected Style White on Blue
            headerSelectedStyle = new GUIStyle();
            headerSelectedStyle.fontStyle = FontStyle.Bold;
            headerSelectedStyle.normal.textColor = Color.white;

            headerSelectedColor = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            headerSelectedColor.SetPixel(0, 0, new Color(0.2f, 0.6f, 1f));
            headerSelectedColor.Apply();
            ///////////////////////////////

            //Pin Icone Texture loading
            pinDataButtonTexture = AssetDatabase.GetBuiltinExtraResource<Texture2D>("UI/Skin/Knob.psd");
            pinFlowButtonTexture = Resources.Load<Texture2D>("Textures/Editor/arrow");
            pinFlowTargetButtonTexture = Resources.Load<Texture2D>("Textures/Editor/target");
        }
    }

    #region Extensible method
    protected abstract void Draw(); //Draw the node 
    protected abstract Texture2D GetHeaderTexture(); //Get the header texture for the node  
    protected abstract GUIStyle GetHeaderStyle(); //Get the header style for this node 

    //Process Event for this node (selection, context menu and Drag the node)
    protected virtual void ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.Used:
                break;
            case EventType.MouseDown:
                //Popup context menu with right clic for this node when the node is selected and the mouse is in the corresponding rect
                if (e.button == 1 && isSelected && node.rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                //left clic select the current node
                if (e.button == 0 && node.rect.Contains(e.mousePosition))
                {
                    OnSelect(this);
                    e.Use();
                }
                break;
            case EventType.MouseDrag:
                //Drag the selected node on mouse position 
                if (e.button == 0 && isSelected)
                {
                    Drag(e.delta);
                    e.Use();
                }
                break;
        }
    }
    
    //Override if specific option needed
    protected virtual void FillContextMenu(GenericMenu menu)
    {
        menu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
    }
    #endregion

    #region main method
    //Drag Node method (called when we move it specificaly or when we move the work view screen 
    public override void Drag(Vector2 delta)
    {
        node.rect.position += delta;
    }

    //Update the node in screen. Triggered by the Graph Controller
    public override void Update(int nodeWindowsDrawId, Event e)
    {
        this.nodeWindowsDrawId = nodeWindowsDrawId;
        ProcessEvents(e);
        Draw();
    }

    //Get the Node
    public override NodeComponent GetNode()
    {
        return node;
    }
    #endregion

    #region Utility method
    //Draw Header of the node window
    protected void DrawHeader()
    {
        GUI.DrawTexture(new Rect(0, 0, node.rect.width + 1, 20), isSelected ? headerSelectedColor : GetHeaderTexture(), ScaleMode.StretchToFill);
        GUI.Label(new Rect(8, 2, node.rect.width - 1, 20), "" + node.name + "", isSelected ? headerSelectedStyle : GetHeaderStyle());
        GUILayout.Space(20);
    }

    //Create the Contextual menu for this node
    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        FillContextMenu(genericMenu);
        genericMenu.ShowAsContext();
    }

    //What todo when remove is called with the contextual menu
    private void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode(this);
        }
    }
    #endregion
}
