using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeBase<T> : NodeComponent
{
    protected T data;

    public NodeBase()
    {
        data = (T)Activator.CreateInstance(typeof(T));
    }

    public T GetData()
    {
        return data;
    }

    public void SetData(T data)
    {
        this.data = data;
    }

    public override void Update()
    {
        Debug.Log("Node " + name + "  updated");
    }
}
