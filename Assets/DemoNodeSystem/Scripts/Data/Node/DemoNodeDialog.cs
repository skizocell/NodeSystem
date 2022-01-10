using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DSGame.GraphSystem;

[Serializable]
[NodeBox(style = NodeBox.StyleName.blue)]
[NodePin(nodePinsType = new NodePin.PinType[] { NodePin.PinType.caller, NodePin.PinType.receiver }, label = "parameters")]
public class DemoNodeDialog : Node
{
    [NodePin(nodePinsType = new NodePin.PinType[] { NodePin.PinType.getter, NodePin.PinType.setter }, label = "parameters")]
    public string test="dfdsf";

    public DemoDialog data;

    private void OnEnable()
    {
        if (data == null) data = new DemoDialog();
    }

    public string GetTest()
    {
        return test;
    }

    public void SetTest(String test)
    {
        Debug.Log(this.name + "SetTextReceived()" + test);
        this.test = test;
    }

    public void Call()
    {
        Debug.Log("Call Done for node labeled (" + this.name + ")");
    }

}
