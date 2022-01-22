using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DSGame.GraphSystem
{
    //node controler base system
    public abstract class NodeControllerBase<N> : NodeControllerComponent where N : Node
    {
        #region variable
        //The windows id for the current node
        protected int nodeWindowsDrawId;

        //The node
        protected N node;

        //The Graph controller
        protected GraphControllerBase graphController;

        //NodeStyle
        protected NodeControllerStyle nodeStyle = null;

        //Selected header for windows
        protected static GUIStyle headerSelectedStyle;
        protected static Texture2D headerSelectedColor;

        //Action todo when a node is removed or selected
        public Action<NodeControllerComponent> OnRemoveNode;
        public Action<NodeControllerComponent> OnSelect;

        public List<NodePinController> nodePins = new List<NodePinController>();
        #endregion

        public NodeControllerBase(GraphControllerBase graphController, N node)
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
            nodeStyle = DefaultNodeControllerStyles.GetStyle(NodeBox.StyleName.green); //set to default
        }

        #region Extensible method
        protected abstract void DrawWindowsContent();

        protected abstract void RefreshController();

        //Process Event for this node (selection, context menu and Drag the node)
        protected virtual void ProcessEvents(Event e, bool candrag)
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
                    //Drag all selected node on mouse position (responsability is for the graphcontroller) 
                    if (candrag && e.button == 0 && isSelected && !e.control)
                    {
                        graphController.DragSelectedNode(e.delta);
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
        public override void Update(int nodeWindowsDrawId, Event e, bool canDrag)
        {
            this.nodeWindowsDrawId = nodeWindowsDrawId;
            ProcessEvents(e, canDrag);

            //If the node editor update is triggered
            if (node.isEditorUpdateNeeded)
            {
                //Force refresh
                node.isEditorUpdateNeeded = false;
                RefreshController();
                EditorUtility.SetDirty(node);
            }
            Draw();
        }

        //Get the Node
        public override Node GetNode()
        {
            return node;
        }

        //On clic on pin
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
        #endregion

        #region Utility method
        //Draw the node Draw call DrawWindowsContent
        protected void Draw()
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

        //Draw Header of the node window
        protected void DrawHeader()
        {
            GUI.DrawTexture(new Rect(0, 0, node.rect.width + 1, 20), isSelected ? headerSelectedColor : nodeStyle.headerTexture, ScaleMode.StretchToFill);
            GUI.Label(new Rect(8, 2, node.rect.width - 1, 20), "" + node.name + "", isSelected ? headerSelectedStyle : nodeStyle.headerStyle);
            GUILayout.Space(20);
        }

        //Draw nodePins 
        protected void DrawPin(Rect windowRect)
        {
            //try to create a style or use a texture for these button
            Color oldBackGroundColor = GUI.backgroundColor;
            Color oldGuiColor = GUI.color;

            //Draw node pin in a color related to the situation
            NodePinController nodePinSelected = graphController.IfConnectionModeGetFirstSelected();
            GUI.backgroundColor = Color.gray;
            GUI.color = Color.gray;

            foreach (NodePinController pin in nodePins)
            {
                //If we try to make a ling nodepin selected is not null
                if (nodePinSelected != null)
                {
                    //Il this pin is the one first selected draw it in blue
                    if (nodePinSelected == pin)
                    {
                        GUI.backgroundColor = Color.blue;
                        GUI.color = Color.blue;
                    }
                    //If it's another, if he can connect -> green
                    else if (nodePinSelected.CanConectTo(pin))
                    {
                        GUI.backgroundColor = Color.green;
                        GUI.color = Color.green;
                    }
                    //else Red
                    else
                    {
                        GUI.backgroundColor = Color.red;
                        GUI.color = Color.red;
                    }
                }
                //Else we are not in link creation
                else
                {
                    //In connected blue
                    if (pin.isConnected)
                    {
                        GUI.backgroundColor = Color.blue;
                        GUI.color = Color.blue;
                    }
                    //else gray
                    else
                    {
                        GUI.backgroundColor = Color.gray;
                        GUI.color = Color.gray;
                    }
                }
                //Finaly draw the node pin after color selection
                pin.Draw(windowRect);
            }

            GUI.color = oldGuiColor;
            GUI.backgroundColor = oldBackGroundColor;
        }

        //Add node Pin to the node and register it to the graph
        protected void AddNodePin(NodePinController pinController)
        {
            if (nodePins.Where(p => p.GetNodePinId() == pinController.GetNodePinId()).Count() > 0)
            {
                throw new Exception("You can't add another pin with the same id for this controller (" + this.GetNode().name + ")=>" + pinController.nodePinId);
            }
            nodePins.Add(pinController);
            graphController.RegisterNodeControllerPin(pinController);
        }

        //Create the Contextual menu for this node
        private void ProcessContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();
            FillContextMenu(genericMenu);
            genericMenu.ShowAsContext();
        }

        //Call OnRemove action
        private void OnClickRemoveNode()
        {
            if (OnRemoveNode != null)
            {
                OnRemoveNode(this);
            }
        }
        #endregion
    }
}
