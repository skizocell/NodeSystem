using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DSGame.GraphSystem;
using System;

[Serializable]
[NodeBox(style = NodeBox.StyleName.blue)]
[NodePin(nodePinsType = new NodePin.PinType[] { NodePin.PinType.receiver }, label = "Choices:")]
public class DemoNodeChoice : Node
{
    [NodePin(nodePinsType = new NodePin.PinType[] { NodePin.PinType.caller }, label="label", id ="id" )]
    public List<Choice> choices;

    [NodePin(nodePinsType = new NodePin.PinType[] { NodePin.PinType.caller, NodePin.PinType.receiver }, label = "parameters")]
    string test = "dfdsf";
}

[Serializable]
public class Choice
{
    public int id;
    public string label; 
}
