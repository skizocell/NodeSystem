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
        Debug.Log(this.name + "Process() text = " + data.text);
    }

    public string GetTest()
    {
        return data.parameter;
    }

    public void SetTest(String test)
    {
        Debug.Log(this.name + "SetTextReceived()" + test);
        data.parameter=test;
    }

    public void Call()
    {
        Debug.Log("Call Done for node labeled (" + this.name + ")");
    }

}
