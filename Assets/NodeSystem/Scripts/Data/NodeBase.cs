using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class NodeBase<T> : NodeComponent
{
    [SerializeField]
    protected T data;

    public T GetData()
    {
        return data;
    }

    public void SetData(T data)
    {
        this.data = data;
    }
}
