using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeGraph : ScriptableObject
{
    #region public Variables
    public string nodeGraphControllerType;
    public List<NodeComponent> nodes = new List<NodeComponent>();
    public List<NodeLink> links = new List<NodeLink>();
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
    #endregion
}
