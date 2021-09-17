using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class GraphControllerBase
{ 
    protected NodeGraph graph;
    List<NodeControllerComponent> nodes = new List<NodeControllerComponent>();
    NodeControllerComponent curSelectedNode =null;
    GUISkin guiSkin;

    #region MetaData info method
    public abstract string GetAssetPath();
    public abstract string GetDescription();
    public abstract string GetName();
    public abstract string GetNodeGraphControllerType();
    #endregion

    #region Extensible method
    public abstract void FillMenu(GenericMenu menu, Vector2 mousePosition);

    public virtual string getSkinResourcesPath()
    {
        return "GUISkins/NodeEditorSkin";//defaultSkinPath
    }

    public virtual void Update(Event e)
    {
        int i=0;
        foreach (NodeControllerComponent node in nodes)
        {
            node.Update(i, e, guiSkin);
            i++;
        }
    }
    #endregion

    #region main method
    //set the Graph to manage
    public void SetGraph(NodeGraph graph)
    {
        guiSkin = (GUISkin)Resources.Load(getSkinResourcesPath());
        this.graph = graph;

        foreach (NodeComponent node in graph.nodes)
        {
            SetController(node);
        }
    }

    public GUISkin GetGUISkin()
    {
        return guiSkin;
    }

    protected void SetController(NodeComponent node)
    {
        NodeControllerComponent controller = NodeControllerFactory.Build(node, DeleteNode, SelectNode);
        nodes.Add(controller);
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
}
