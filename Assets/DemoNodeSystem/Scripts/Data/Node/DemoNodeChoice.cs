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
    //todo review label and id management
    [NodePin(nodePinsType = new NodePin.PinType[] { NodePin.PinType.caller }, label="label", id ="id" )]
    public List<Branch> choices; //Fork

    [NodePin(nodePinsType = new NodePin.PinType[] { NodePin.PinType.getter, NodePin.PinType.setter }, label = "parameters")]
    string test = "dfdsf";
}