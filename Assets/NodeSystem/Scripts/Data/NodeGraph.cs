using System;
using System.Reflection;
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
    public DemoNodeDialog start;
    #endregion

    #region main 
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

    public void Excecute()
    {
        Init();

        NodeComponent node = GetNextReadyNode();
        while (node!=null)
        {
            ProcessNode(node);
            foreach (NodeLink l in GetNextsWaitingNodeLink(node).OrderBy(o => o.linkType))//order to make set before call see LinkType
            {
                ProcessLink(l);
                if (!IsWaitingLinkExistFor(l.to, false))
                {
                    l.to.processStatus = NodeProcessStatus.Ready;
                }
            }
            node = GetNextReadyNode();
        }
    }
    #endregion

    #region Utility method

    //For specific nodeGraph just used with ref link and purely has sequence like dialog system.... 
    //Or with system with a preprocess that can produce a chained link node in term...
    //praticaly the Same than execute
    public List<NodeComponent> GetChainedList()
    {
        List<NodeComponent> nodeList = new List<NodeComponent>();
        Init();
        NodeComponent node = GetNextReadyNode();
        while (node != null)
        {
            nodeList.Add(node);
            node.processStatus = NodeProcessStatus.Done;
            foreach (NodeLink l in GetNextsWaitingNodeLink(node).OrderBy(o => o.linkType))//order to make set before call see LinkType
            {
                ProcessLink(l);
                if (!IsWaitingLinkExistFor(l.to, false))
                {
                    l.to.processStatus = NodeProcessStatus.Ready;
                }
            }
            node = GetNextReadyNode();
        }
        return nodeList;
    }

    private void Init()
    {
        //Get Node waitink for a link to process
        List<NodeComponent> waitingLinkComponents = links.Select(l => l.to).ToList();

        foreach (NodeComponent n in nodes)
        {
            if (waitingLinkComponents.Contains(n)) n.processStatus = NodeProcessStatus.Waiting;
            else n.processStatus = NodeProcessStatus.Ready;

        }
        foreach (NodeLink l in links)
        {
            l.processStatus = NodeProcessStatus.Waiting;
        }
    }

    public NodeComponent GetNextReadyNode()
    {
        return nodes.Where(n => n.processStatus == NodeProcessStatus.Ready).FirstOrDefault();
    }

    private void ProcessNode(NodeComponent n)
    {
        n.processStatus = NodeProcessStatus.Running;
        n.Process();
        n.processStatus = NodeProcessStatus.Done;
    }

    private void ProcessLink(NodeLink l)
    {
        l.processStatus = NodeProcessStatus.Running;
        switch (l.linkType)
        {
            case NodeLink.LinkType.Call:
                MethodInfo method = l.to.GetType().GetMethod(l.toPinId);
                method.Invoke(l.to, null);
                break;
            case NodeLink.LinkType.Set:
                MethodInfo setMethod = l.to.GetType().GetMethod(l.toPinId);
                MethodInfo getMethod = l.from.GetType().GetMethod(l.fromPinId);
                setMethod.Invoke(l.to, new object[] { getMethod.Invoke(l.from, null) });
                break;
        }
        l.processStatus = NodeProcessStatus.Done;
    }

    public bool IsLinkExistFor(NodeComponent component, String pinId, bool isCallerType)
    {
        if (isCallerType)
            return links.Where(l => l.from == component && l.fromPinId.Equals(pinId)).Count() > 0;
        else
        {
            return links.Where(l => l.to == component && l.toPinId.Equals(pinId)).Count() > 0;
        }
    }

    private bool IsWaitingLinkExistFor(NodeComponent component, bool isCallerType)
    {
        if (isCallerType)
            return links.Where(l => l.from == component && l.processStatus == NodeProcessStatus.Waiting).Count() > 0;
        else
        {
            return links.Where(l => l.to == component && l.processStatus == NodeProcessStatus.Waiting).Count() > 0;
        }
    }

    private IEnumerable<NodeLink> GetNextsWaitingNodeLink(NodeComponent node)
    {
        return links.Where(l => l.processStatus == NodeProcessStatus.Waiting
                                && l.from.Equals(node));
    }
    #endregion
}
