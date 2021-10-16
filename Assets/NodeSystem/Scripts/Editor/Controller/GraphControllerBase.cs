using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class GraphControllerBase
{ 
    protected NodeGraph graph;
    protected INodeControllerFactory nodeFactory;
    List<NodeControllerComponent> nodes = new List<NodeControllerComponent>();
    private List<NodePinController> nodePins = new List<NodePinController>();
    NodeControllerComponent curSelectedNode = null;

    private NodePinController selectedEmiterPin;
    private NodePinController selectedReceiverPin;

    protected GraphControllerBase()
    {
        InitFactory();
    }

    #region MetaData info method
    public abstract string GetAssetPath(); //Where to save your new graph assets 
    public abstract string GetDescription(); //Description of your graph
    public abstract string GetName(); //Name of graph
    public string GetNodeGraphControllerType()//Type of the graph controller
    {
        return this.GetType().ToString();
    }
    #endregion

    #region Extensible method
    public abstract void FillMenu(GenericMenu menu, Vector2 mousePosition); // mouse position is used to know where to create something on screen if needed
    public abstract void InitFactory();

    public virtual void Update(Event e)
    {
        DrawConnections();
        DrawConnectionLine(e);

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 1)
                {
                    if (selectedEmiterPin != null || selectedReceiverPin != null)
                    {
                        ClearConnectionSelection();
                        e.Use();
                    }
                }
                break;
        }

        int i=0;
        foreach (NodeControllerComponent node in nodes)
        {
            node.Update(i, e);
            i++;
        }
    }
    #endregion

    #region main method
    //set the Graph to manage
    public void SetGraph(NodeGraph graph)
    {
        this.graph = graph;

        foreach (NodeComponent node in graph.nodes)
        {
            SetController(node);
        }
    }



    public void OnClicPinController(NodePinController nodePin)
    {
        if (!nodePin.canHaveManyLink)
        {
            if (graph.IsLinkExistFor(nodePin.linkedNodeConroller.GetNode(), nodePin.type == NodePinController.Type.Emiter))
            {
                return;
            }
        }

        if(nodePin.type == NodePinController.Type.Emiter)
        {
            if (selectedEmiterPin == null)
            {
                selectedEmiterPin = nodePin;
            }
        }

        if (nodePin.type == NodePinController.Type.Receiver)
        {
            if (selectedReceiverPin == null)
            {
                selectedReceiverPin = nodePin;
            }
        }

        if(selectedEmiterPin != null && selectedReceiverPin != null)
        {
            if (selectedEmiterPin.linkedNodeConroller != selectedReceiverPin.linkedNodeConroller)
            {
                NodeLink link = new NodeLink();
                link.from = selectedEmiterPin.linkedNodeConroller.GetNode();
                link.to = selectedReceiverPin.linkedNodeConroller.GetNode();
                link.fromPinId = selectedEmiterPin.nodePinId;
                link.toPinId = selectedReceiverPin.nodePinId;
                graph.links.Add(link);
            }
            else
            {
                EditorUtility.DisplayDialog("Node message", "You can't link a node to himself", "Ok");

            }
            ClearConnectionSelection();
        }
    }

    public void RegisterNodeControllerPin(NodePinController pin)
    {
        nodePins.Add(pin);
    }

    public void Drag(Vector2 delta)
    {
        if(curSelectedNode ==null || !curSelectedNode.isSelected)
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
        UnSelectNode();
        nodeController.isSelected = true;
        Selection.activeObject = nodeController.GetNode();
        curSelectedNode = nodeController;
    }

    public void UnSelectNode()
    {
        if (curSelectedNode != null) curSelectedNode.isSelected = false;
    }
    #endregion

    #region utility method
    private void ClearConnectionSelection()
    {
        selectedEmiterPin = null;
        selectedReceiverPin = null;
    }

    protected void SetController(NodeComponent node)
    {
        NodeControllerComponent controller = nodeFactory.Build(node);
        nodes.Add(controller);
    }

    private NodePinController GetNodePinController(NodeComponent node, string nodePinId)
    {
        IEnumerable<NodeControllerComponent> result = nodes.Where(n => n.GetNode() == node);
        if (result.Count() == 1)
        {
            return result.First().GetControllerFor(nodePinId);
        }
        return null;
    }

    private void DrawConnections()
    {
        if (graph.links != null)
        {
            foreach(NodeLink link in graph.links)
            {
                NodePinController caller = GetNodePinController(link.from, link.fromPinId);
                NodePinController called = GetNodePinController(link.to, link.toPinId);

                if(caller == null || called == null)
                {
                    Debug.LogError("Remove corrupted node link ");
                    graph.links.Remove(link);
                    break;
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

                if (Handles.Button((caller.GetRect().center + called.GetRect().center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
                {
                    graph.links.Remove(link);
                    break;
                }
            
            }
        }
    }

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
    #endregion
}
