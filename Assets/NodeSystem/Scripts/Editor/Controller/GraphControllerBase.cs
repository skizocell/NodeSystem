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
    private List<INodePinController> nodePins = new List<INodePinController>();
    NodeControllerComponent curSelectedNode = null;

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

        InitNodeLink();
    }

    public void RegisterNodeControllerPin(INodePinController pin)
    {
        Debug.Log("Node pin registred " + pin);
        nodePins.Add(pin);
    }

    public void InitNodeLink()
    {
        Debug.Log("InitNodeLink start");
        //TODO init NodeLink by check emiter target
        foreach (INodePinController item in nodePins.Where(c=>c.GetNodePin() is NodePinEmitter))
        {
            //Test if target is filled with a know receiver and create NodeLink for the graph
        }

        //Next make control of the NodeLink and establish link between node
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
    protected void SetController(NodeComponent node)
    {
        NodeControllerComponent controller = nodeFactory.Build(node);
        nodes.Add(controller);
    }
    #endregion
}
