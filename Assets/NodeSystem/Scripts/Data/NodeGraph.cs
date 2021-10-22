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
        //NOT GOOD
        foreach (NodeComponent n in nodes)
        {
            if( n.processStatus==NodeProcessStatus.Waiting && !IsWaitingLinkExistFor(n, false))
            {
                ProcessNode(n);
                foreach(NodeLink l in GetNextsWaitingNodeLink(n).OrderBy(o => o.linkType)) //order to make set before call see LinkType
                {
                    ProcessLink(l);
                    if(!IsWaitingLinkExistFor(l.to, false))
                    {
                        ProcessNode(l.to);
                    }
                }
            } 
        }
    }
    #endregion

    #region Utility method
    private void Init()
    {
        foreach (NodeComponent n in nodes)
        {
            n.processStatus = NodeProcessStatus.Waiting;
        }
        foreach (NodeLink l in links)
        {
            l.processStatus = NodeProcessStatus.Waiting;
        }
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
