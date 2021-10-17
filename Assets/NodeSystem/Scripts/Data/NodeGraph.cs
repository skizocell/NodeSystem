using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class NodeGraph : ScriptableObject
{
    #region public Variables
    public string nodeGraphControllerType;
    [SerializeField]
    private List<NodeComponent> nodes = new List<NodeComponent>();
    [SerializeField]
    private List<NodeLink> links = new List<NodeLink>();
    public NodeDialog start;
    #endregion

    #region main 
    public bool IsLinkExistFor(NodeComponent component, bool isCallerType)
    {
        if(isCallerType)
            return links.Where(l => l.from == component).Count() > 0;
        else
        {
            return links.Where(l => l.to == component).Count() > 0;
        }
    }

    public void AddNode(NodeComponent node)
    {
        nodes.Add(node);
    }

    public void RemoveNode(NodeComponent node)
    {
        nodes.Remove(node);
    }

    public void AddLink(NodeLink link)
    {
        links.Add(link);
    }

    public void RemoveLink(NodeLink link)
    {
        links.Remove(link);
    }

    public List<NodeComponent> GetNodes()
    {
        return nodes;
    }

    public List<NodeLink> GetLinks()
    {
        return links;
    }

    //Todo method to drive node
    //iterator on nodes to have next link etc...
    //Make a nod DialogGraph to specific process

    //private void Test()
    //{
    //    Debug.Log("Test Start");
    //    graph.start.Process();
    //    NodeLink next = graph.GetLinks().Where(n => n.from == graph.start).First();

    //    MethodInfo method = next.to.GetType().GetMethod(next.toPinId);
    //    method.Invoke(next.to, null);
    //}
    #endregion
}
