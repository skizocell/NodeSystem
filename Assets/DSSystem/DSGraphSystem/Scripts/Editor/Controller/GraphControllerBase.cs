using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;

//new way to do that:
//using UnityEditor.Experimental.GraphView;
//look here https://www.youtube.com/watch?v=7KHGH0fPL84

//Graph controller base system
namespace DSGame.GraphSystem
{
    [Serializable]
    public abstract class GraphControllerBase
    {
        private Rect boxRect;
        private List<NodePinController> nodePins = new List<NodePinController>();
        private NodeControllerComponent curSelectedNode = null;

        private NodePinController selectedEmiterPin;
        private NodePinController selectedReceiverPin;

        protected Graph graph;
        protected NodeControllerFactory nodeFactory;
        protected List<NodeControllerComponent> nodes = new List<NodeControllerComponent>();

        public bool ctrlOn = false; //control button pushed
        public bool dragBox = false;
        public Vector2 selectBoxStartPos;

        protected GraphControllerBase()
        {
            nodeFactory = new NodeControllerFactory(this);
        }

        #region MetaData info method
        public abstract string GetDescription(); //Description of your graph
        public abstract string GetName(); //Name of graph
        public abstract string GetGraphClassName(); //Class Name of the graph
        public abstract string GetDefaultSaveFolderPath(); //Default Save Folder of the graph (->>>>Resources folder!!!)

        public string GetNodeGraphControllerType()//Type of the graph controller
        {
            return this.GetType().ToString();
        }
        #endregion

        #region Extensible method
        // mouse position is used to know where to create something on screen if needed
        public virtual void FillMenu(GenericMenu menu, Vector2 mousePosition)
        {
            menu.AddItem(new GUIContent("Portal Out"), false, (mousePos) => AddPortalOut((Vector2)mousePos), mousePosition);
            menu.AddItem(new GUIContent("Prortal In"), false, (mousePos) => AddPortalIn((Vector2)mousePos), mousePosition);
        }

        public virtual void Update(Event e)
        {
            if (dragBox)
            {
                //draw selection box
                boxRect = RectExtensions.MakeRect(selectBoxStartPos, e.mousePosition);
                GUI.Box(boxRect, "");
                GUI.changed = true;
            }
            DrawConnections();
            DrawConnectionLine(e);
            DrawNodes(e);

            ctrlOn = e.control;
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1)
                    {
                        //Right Clic 
                        if (selectedEmiterPin != null || selectedReceiverPin != null)
                        {
                            //cancel link drawing
                            ClearConnectionSelection();
                            e.Use();
                        }
                    }
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        //Left drag
                        if (!dragBox)
                        {
                            //Activate drag mode and savec start position
                            dragBox = true;
                            selectBoxStartPos = e.mousePosition;
                        }
                        e.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (e.button == 0)
                    {
                        //Stop dragbox mode
                        dragBox = false;
                    }
                    break;
            }

        }

        //Drag selected nodes
        public void DragSelectedNode(Vector2 delta)
        {
            foreach (NodeControllerComponent node in nodes.Where(n=>n.isSelected))
            {
                node.Drag(delta);
            }
        }
        #endregion

        #region main method
        //Get Player pref save folder to have much confort
        public string GetGraphSaveFolderPath()
        {
            string key = GetPlayerPrefSaveFolderKey();
            String path = PlayerPrefs.GetString(key);
            if (path == null || path.Equals(""))
            {
                path = GetDefaultSaveFolderPath();
                PlayerPrefs.SetString(key, path);
            }
            return path;
        }

        //Save last choodes folder to player pref
        public void SetGraphSaveFolderPath(string path)
        {
            string key = GetPlayerPrefSaveFolderKey();
            PlayerPrefs.SetString(key, path);
        }

        //set the Graph to manage
        public void SetGraph(Graph graph)
        {
            this.graph = graph;

            foreach (Node node in graph.GetNodes())
            {
                //Call generic method with the good type dynamicaly
                MethodInfo methodInfo = typeof(GraphControllerBase).GetMethod("SetController",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo setControllerGeneric = methodInfo.MakeGenericMethod(node.GetType());
                setControllerGeneric.Invoke(this, new System.Object[] { node });
            }
        }

        public Graph GetGraph()
        {
            return graph;
        }

        public void OnClicPinController(NodePinController nodePin)
        {
            //If lin can not have many link
            if (!nodePin.canHaveManyLink)
            {
                //If a link already exist
                if (graph.IsLinkExistFor(nodePin.linkedNodeConroller.GetNode(), nodePin.nodePinId, nodePin.type == NodePinController.NodePinType.Emiter))
                {
                    //stop doing thing
                    return;
                }
            }

            if (nodePin.type == NodePinController.NodePinType.Emiter)
            {
                if (selectedEmiterPin == null)
                {
                    selectedEmiterPin = nodePin;
                }
            }

            if (nodePin.type == NodePinController.NodePinType.Receiver)
            {
                if (selectedReceiverPin == null)
                {
                    selectedReceiverPin = nodePin;
                }
            }

            //if selected Emiter and selected receiver is set 
            if (selectedEmiterPin != null && selectedReceiverPin != null)
            {
                //if it's not the same node
                if (selectedEmiterPin.linkedNodeConroller != selectedReceiverPin.linkedNodeConroller)
                {
                    //If the connexion can be done
                    if (selectedEmiterPin.CanConectTo(selectedReceiverPin))
                    {
                        //create the link
                        NodeLink link = new NodeLink();
                        link.from = selectedEmiterPin.linkedNodeConroller.GetNode();
                        link.to = selectedReceiverPin.linkedNodeConroller.GetNode();
                        link.fromPinId = selectedEmiterPin.nodePinId;
                        link.toPinId = selectedReceiverPin.nodePinId;
                        link.linkType = selectedEmiterPin.generateLinkType;
                        AddLink(link);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Node message", "This connection can not be done", "Ok");
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("Node message", "You can't link a node to himself", "Ok");

                }
                ClearConnectionSelection();
            }
        }

        //Register new node pin in the graph
        public void RegisterNodeControllerPin(NodePinController pin)
        {
            nodePins.Add(pin);
        }

        //Drag selected node
        public void Drag(Vector2 delta)
        {
            if (curSelectedNode == null || !curSelectedNode.isSelected)
                foreach (NodeControllerComponent node in nodes)
                {
                    node.Drag(delta);
                }
        }

        public void DeleteNode(NodeControllerComponent nodeController)
        {
            NodesUtils.DeleteNode(graph, nodeController.GetNode());
            nodes.Remove(nodeController);
        }

        public void SelectNode(NodeControllerComponent nodeController)
        {
            if(!nodeController.isSelected ) UnSelectNode();
            nodeController.isSelected = true;
            Selection.activeObject = nodeController.GetNode();
            curSelectedNode = nodeController;
        }

        public void UnSelectNode()
        {
            //if ctrl is pushed cancel unselect.
            if (ctrlOn) return;
            if (curSelectedNode != null) curSelectedNode=null;
            foreach (NodeControllerComponent node in nodes)
            {
                node.isSelected = false;
            }
            Selection.activeObject = graph;
        }
        #endregion

        #region utility method
        //Generate a key for player pref (one per company, product, graph class) to save 
        private string GetPlayerPrefSaveFolderKey()
        {
            return PlayerSettings.companyName + "." + PlayerSettings.productName + "." + GetGraphClassName() + ".saveFolder";
        }

        private void AddPortalIn(Vector2 mousePos)
        {
            PortalIn portalIn = NodesUtils.CreateNode<PortalIn>(graph, new Rect(mousePos.x, mousePos.y, 200f, 65f));
            portalIn.graph = graph;
            SetController(portalIn);
        }

        private void AddPortalOut(Vector2 mousePos)
        {
            PortalOut portalOut = NodesUtils.CreateNode<PortalOut>(graph, new Rect(mousePos.x, mousePos.y, 200f, 65f));
            SetController(portalOut);
        }

        private bool IsInNodeConnectionMode()
        {
            return (selectedEmiterPin != null && selectedReceiverPin == null) || (selectedReceiverPin != null && selectedEmiterPin == null);
        }

        public NodePinController IfConnectionModeGetFirstSelected()
        {
            if (IsInNodeConnectionMode())
            {
                return selectedEmiterPin != null ? selectedEmiterPin : selectedReceiverPin;
            }
            return null;
        }

        private void ClearConnectionSelection()
        {
            selectedEmiterPin = null;
            selectedReceiverPin = null;
        }

        //Build a controller to show our node and register it to the graph controller
        protected void SetController<N>(N node) where N : Node
        {
            NodeControllerComponent controller = nodeFactory.BuildNodeControllerComponent(node);
            nodes.Add(controller);
        }

        //Get a node pin controller for a node
        private NodePinController GetNodePinController(Node node, string nodePinId)
        {
            IEnumerable<NodeControllerComponent> result = nodes.Where(n => n.GetNode() == node);
            if (result.Count() == 1)
            {
                return result.First().GetControllerFor(nodePinId);
            }
            return null;
        }

        //Draw nodes
        private void DrawNodes(Event e)
        {
            int i = 0;
            foreach (NodeControllerComponent nodeController in nodes)
            {
                if (dragBox)
                {
                    Node n = nodeController.GetNode();
                    if (boxRect.Overlaps(n.rect))
                    {
                        nodeController.isSelected = true;
                    }
                    else if (!ctrlOn)
                    {
                        nodeController.isSelected = false;
                    }
                }
                nodeController.Update(i, e, !dragBox);
                i++;
            }
        }

        //Draw connections
        private void DrawConnections()
        {
            foreach (NodeLink link in graph.GetLinks())
            {
                NodePinController caller = GetNodePinController(link.from, link.fromPinId);
                NodePinController called = GetNodePinController(link.to, link.toPinId);

                //if one of thew is not found the link is corrupted and must be removed
                if (caller == null || called == null)
                {
                    Debug.LogError("Remove corrupted node link ");
                    RemoveLink(link);
                    break;
                }
                else
                {
                    caller.isConnected = true;
                    called.isConnected = true;
                }

                Handles.DrawBezier(
                        called.GetRect().center,
                        caller.GetRect().center,
                        called.GetRect().center + Vector2.left * 50f,
                        caller.GetRect().center - Vector2.left * 50f,
                        Color.white,
                        null,
                        2f
                    );

                //If you clic the linq he can be removed
                if (Handles.Button((caller.GetRect().center + called.GetRect().center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
                {
                    RemoveLink(link);
                    break;
                }
            }
        }

        //Draw connection line (when we create the connection)
        private void DrawConnectionLine(Event e)
        {
            if (selectedEmiterPin != null && selectedReceiverPin == null)
            {
                Handles.DrawBezier(
                    selectedEmiterPin.GetRect().center,
                    e.mousePosition,
                    selectedEmiterPin.GetRect().center - Vector2.left * 50f,
                    e.mousePosition + Vector2.left * 50f,
                    Color.white,
                    null,
                    2f
                );

                GUI.changed = true;
            }

            if (selectedReceiverPin != null && selectedEmiterPin == null)
            {
                Handles.DrawBezier(
                    selectedReceiverPin.GetRect().center,
                    e.mousePosition,
                    selectedReceiverPin.GetRect().center + Vector2.left * 50f,
                    e.mousePosition - Vector2.left * 50f,
                    Color.white,
                    null,
                    2f
                );

                GUI.changed = true;
            }
        }

        //Add link
        private void AddLink(NodeLink link)
        {
            graph.AddLink(link);
            ForceSave();
        }

        //Remove link
        private void RemoveLink(NodeLink link)
        {
            graph.RemoveLink(link);
            ForceSave();
        }

        //Force the save of the graph
        private void ForceSave()
        {
            EditorUtility.SetDirty(graph);
        }
        #endregion
    }
}
