using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DSGame.GraphSystem;
using System;

// Solution pour le problème de refresh sur les Branch https://stackoverflow.com/questions/48223969/onvaluechanged-for-fields-in-a-scriptable-object
[Serializable]
[NodeBox(style = NodeBox.StyleName.blue)]
[NodePin(nodePinsType = new NodePin.PinType[] { NodePin.PinType.receiver }, label = "Choices:")]
public class DemoNodeChoice : Node
{
    //TODO review label and id management
    //MAKE a portal Node
    [NodePin(nodePinsType = new NodePin.PinType[] { NodePin.PinType.caller }, label="label", id ="id" )]
    public List<Branch> choices; //Fork

    //look to make this more generic and call a CustomPropertyDrawer conditionaly on the pin and activate it where branch is used
    //when the modification is detected make sure the controller to redraw the component 
    [NodePin(nodePinsType = new NodePin.PinType[] { NodePin.PinType.caller }, label = "conditional branch", id = "id")]
    [BranchChange(OnChangeCall = "")]
    public Branch conditional;

    [NodePin(nodePinsType = new NodePin.PinType[] { NodePin.PinType.getter, NodePin.PinType.setter }, label = "parameters")]
    public string test = "dfdsf";
}