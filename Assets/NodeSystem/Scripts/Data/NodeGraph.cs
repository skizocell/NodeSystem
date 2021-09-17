using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeGraph : ScriptableObject
{
    #region public Variables
    public string nodeGraphControllerType;
    public List<NodeComponent> nodes = new List<NodeComponent>();
    #endregion

    #region main 
    #endregion
}
