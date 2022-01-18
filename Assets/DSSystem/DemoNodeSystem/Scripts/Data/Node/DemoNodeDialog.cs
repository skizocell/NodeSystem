using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DSGame.GraphSystem;

//Show Node with a custom controller.
[Serializable]
public class DemoNodeDialog : Node
{
    public DemoDialog data;

    private void OnEnable()
    {
        if (data == null) data = new DemoDialog();
    }

    public void Call()
    {
        Debug.Log("Call Done for node labeled (" + this.name + ")");
    }

}
