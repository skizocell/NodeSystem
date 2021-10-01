using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodePin
{
    public enum PinType
    {
        Caller,
        Called,
        DataEmiter,
        DataReceiver
    }
    [SerializeField]
    protected PinType type;
}

[Serializable]
public abstract class NodePinEmitter : NodePin
{
    public abstract NodePin GetTarget();
    public abstract void Call();
}

[Serializable]
public class NodePinCaller: NodePinEmitter
{
    [SerializeField]
    protected NodePinCalled target;

    public NodePinCaller()
    {
        type = PinType.Caller;
    }

    public override NodePin GetTarget()
    {
        return target;
    }

    public void SetTarget(NodePinCalled target)
    {
        this.target = target;
    }

    public override void Call()
    {
        if (target != null)
        {
            target.Process();
        }
    }
}

[Serializable]
//TODO DATA EMITER can notify data in multiple client
public class NodePinDataEmitter<T> : NodePinEmitter
{
    [SerializeField]
    protected NodePinDataReceiver<T> target;

    [NonSerialized]
    protected Func<T> DataAccessor;

    public NodePinDataEmitter()
    {
        type = PinType.DataEmiter;
    }

    public void SetDataAccessor(Func<T> DataAccessor)
    {
        this.DataAccessor = DataAccessor;
    }

    public override NodePin GetTarget()
    {
        return target;
    }

    public void SetTarget(NodePinDataReceiver<T> target)
    {
        this.target = target;
    }

    public override void Call()
    {
        if (target != null)
        {
            if (DataAccessor == null) throw new Exception("DataAccessor has not been set");
            target.Push(DataAccessor());
        }
    }
}

public class NodePinReceiver : NodePin
{
    public enum Status
    {
        Unset,
        Ready
    }

    public Status status;
}

[Serializable]
public class NodePinCalled : NodePinReceiver
{
    //Action exposed to caller
    [NonSerialized]
    protected Action ActionToBeCalled;

    public NodePinCalled()
    {
        type = PinType.Called;
        status = Status.Unset;
    }

    public void SetAction(Action ActionToBeCalled)
    {
        status = Status.Ready;
        this.ActionToBeCalled = ActionToBeCalled;
    }

    public void Process()
    {
        if (status == Status.Ready)
        {
            ActionToBeCalled();
        }
    }
}

[Serializable]
public class NodePinDataReceiver<T> : NodePinReceiver
{
    [NonSerialized]
    protected Action<T> DataPusher;

    public NodePinDataReceiver()
    {
        type = PinType.DataReceiver;
        status = Status.Unset;
    }

    public void SetDataPusher(Action<T> DataPusher)
    {
        status = Status.Ready;
        this.DataPusher = DataPusher;
    }

    public void Push(T data)
    {
        if (status == Status.Ready)
        {
            DataPusher(data);
        }
    }
}
