using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DemoNodeDialog : NodeBase<DemoDialog>
{
    private void OnEnable()
    {
        if (data == null) data = new DemoDialog();
    }

    public override void Process()
    {
        //TODO invoke dialog system with dialog data and wait the return then call the next Dialog. 
        //Construct an interface to pass dialog. The settings must be done in the graph node could access graph settings??
        //actually it's attached to controllertype and graph is the same for all...
        Debug.Log(this.name + "Process() text = " + data.text);
        //emiter.Call();
        //if(nextCall!=null) nextCall.Call();
    }

    public string GetTest()
    {
        return data.parameter;
    }

    public void SetTest(String test)
    {
        //When a set is receive and the object is already calculated change the state of the object (to process by example)
        // if other setter is ready...
        Debug.Log(this.name + "SetTextReceived()" + test);
        data.parameter=test;
    }

    public void Call()
    {
        Debug.Log("Call Done for node labeled (" + this.name + ")");
    }

}