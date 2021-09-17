using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodePin
{
    Rect rect;
    public enum PinType
    {
        Call,
        Input, //
        Output
    }
    protected PinType type;
    //excecution pin 
    // call what
    //data pin 
    // accept what and get from...
    // 
    //
}
public class NodePinCalled : NodePin
{
    Action ActionToBeCalled;
    public NodePinCalled(Action ActionToBeCalled)
    {
        this.ActionToBeCalled = ActionToBeCalled;
    }

    public void Process()
    {
        ActionToBeCalled();
    }
}

public abstract class NodePinCaller : NodePin
{
    public abstract NodePin GetTarget();
    public abstract void Process();
}

public class NodePinCall: NodePinCaller
{
    protected NodePinCalled target;

    public NodePinCall()
    {
        type = PinType.Call;
    }

    public override NodePin GetTarget()
    {
        return target;
    }

    public void SetTarget(NodePinCalled target)
    {
        this.target = target;
    }

    public override void Process()
    {
        if (target != null)
        {
            Debug.Log("Process pin and call the next ->");
            target.Process();
        }
    }
}

public class NodePinOutput<T> : NodePinCaller
{
    protected NodePinInput<T> target;
    Func<T> DataAccessor;
    public NodePinOutput(Func<T> DataAccessor)
    {
        type = PinType.Output;
        this.DataAccessor = DataAccessor;
    }

    public override NodePin GetTarget()
    {
        return target;
    }

    public void SetTarget(NodePinInput<T> target)
    {
        this.target = target;
    }

    public override void Process()
    {
        Debug.Log("Process pin and set the next ->");
        target.Set(DataAccessor());
    }
}

public class NodePinInput<T> : NodePin
{
    public enum Status
    {
        Waiting,
        Done
    }
    public Status status = Status.Waiting;

  //  Type type = typeof(T);

    Action<T> set;

    public void Set(T data)
    {
        status = Status.Done;
        set(data);
    }
}
