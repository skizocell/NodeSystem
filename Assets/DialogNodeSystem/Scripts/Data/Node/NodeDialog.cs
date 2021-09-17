using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeDialog : NodeBase<Dialog>
{
    public NodePinCall nextCall;
    public NodePinCalled process;
    public NodeDialog() : base()
    {
        //TODO Avoid to pin the same NodeDialog Pin
        process = new NodePinCalled(Process);
        pins.Add(process);
    }

    public void setNextCall(NodePinCall nextCall)
    {
        this.nextCall = nextCall;
        pins.Add(nextCall);
    }

    public void Process()
    {
        //TODO invoke dialog system with dialog data and wait the return then call the next Dialog. 
        //Construct an interface to pass dialog. The settings must be done in the graph node could access graph settings??
        //actually it's attached to controllertype and graph is the same for all...
        Debug.Log(data.text);
        if(nextCall!=null) nextCall.Process();
    }
}
