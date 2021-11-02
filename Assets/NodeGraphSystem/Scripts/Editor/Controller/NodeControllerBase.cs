using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class NodeControllerBase<N, G> : NodeControllerComponent where N : NodeComponent where G : GraphControllerBase
{
    #region variable
    //The windows id for the current node
    protected int nodeWindowsDrawId;

    //The node
    protected N node;

    //The Graph controller
    protected G graphController;

    //NodeStyle
    protected NodeControllerStyleNodeStyle nodeStyle = null;

    //Selected header for windows
    protected static GUIStyle headerSelectedStyle;
    protected static Texture2D headerSelectedColor;

    //Action todo when a node is removed or selected
    public Action<NodeControllerComponent> OnRemoveNode;
    public Action<NodeControllerComponent> OnSelect;

    public List<NodePinController> nodePins = new List<NodePinController>();
    #endregion

    public NodeControllerBase(G graphController, N node)
    {
        this.node = node;
        this.graphController = graphController;
        this.OnRemoveNode = graphController.DeleteNode;
        this.OnSelect = graphController.SelectNode;

        //Init when style is null
        if (headerSelectedStyle == null)
        {
            //Selected Style White on Blue
            headerSelectedStyle = new GUIStyle();
            headerSelectedStyle.fontStyle = FontStyle.Bold;
            headerSelectedStyle.normal.textColor = Color.white;

            headerSelectedColor = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            headerSelectedColor.SetPixel(0, 0, new Color(0.2f, 0.6f, 1f));
            headerSelectedColor.Apply();
            ///////////////////////////////
        }
        nodeStyle = DefaultNodeControllerStyles.GetStyle(DefaultNodeControllerStyles.StyleName.green); //set to default
    }

    #region Extensible method
    protected abstract void DrawWindowsContent();

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

    protected void Draw() //Draw the node
    {
        Rect windowRect = GUILayout.Window(nodeWindowsDrawId, node.rect, DrawWindowsContent, "", GUIStyle.none);
        GUI.Box(windowRect, "", GUI.skin.window);
        DrawPin(windowRect);
    }

    private void DrawWindowsContent(int windowsId)
    {
        DrawHeader();
        DrawWindowsContent(); //children content
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

    //on clic on pin
    public override void OnClickNodePin(NodePinController nodepin)
    {
        graphController.OnClicPinController(nodepin);
    }

    public override NodePinController GetControllerFor(string key)
    {
        IEnumerable<NodePinController> nodePinController = nodePins.Where(p => p.GetNodePinId() == key);
        if (nodePinController.Count() == 1)
        {
            return nodePinController.First();
        }
        else
        {
            return null;
        }
    }

    protected void DrawPin(Rect windowRect)
    {
        //try to create a style or use a texture for these button
        Color oldBackGroundColor = GUI.backgroundColor;
        Color oldGuiColor = GUI.color;

        NodePinController nodePinSelected = graphController.IfConnectionModeGetFirstSelected();
        GUI.backgroundColor = Color.gray;
        GUI.color = Color.gray;


        foreach(NodePinController pin in nodePins)
        {
            if(nodePinSelected != null)
            {
                if(nodePinSelected == pin)
                {
                    GUI.backgroundColor = Color.blue;
                    GUI.color = Color.blue;
                }
                else if (nodePinSelected.CanConectTo(pin))
                {
                    GUI.backgroundColor = Color.green;
                    GUI.color = Color.green;
                }
                else
                {
                    GUI.backgroundColor = Color.red;
                    GUI.color = Color.red;
                }
            }
            else
            {
                if (pin.isConnected)
                {
                    GUI.backgroundColor = Color.blue;
                    GUI.color = Color.blue;
                }
                else
                {
                    GUI.backgroundColor = Color.gray;
                    GUI.color = Color.gray;
                }
            }
            pin.Draw(windowRect);
        }

        GUI.color = oldGuiColor;
        GUI.backgroundColor = oldBackGroundColor;
    }
    #endregion

    #region Utility method
    protected void AddNodePin(NodePinController pinController)
    {
        if(nodePins.Where(p => p.GetNodePinId() == pinController.GetNodePinId()).Count()>0)
        {
            throw new Exception("You can't add another pin with the same id for this controller (" + this.GetNode().name + ")=>" + pinController.nodePinId);
        }
        nodePins.Add(pinController);
        graphController.RegisterNodeControllerPin(pinController);
    }

    //Draw Header of the node window
    protected void DrawHeader()
    {
        GUI.DrawTexture(new Rect(0, 0, node.rect.width + 1, 20), isSelected ? headerSelectedColor : nodeStyle.headerTexture, ScaleMode.StretchToFill);
        GUI.Label(new Rect(8, 2, node.rect.width - 1, 20), "" + node.name + "", isSelected ? headerSelectedStyle : nodeStyle.headerStyle);
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