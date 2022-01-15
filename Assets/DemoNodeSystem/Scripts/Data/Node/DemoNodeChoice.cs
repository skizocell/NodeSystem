using System.Collections.Generic;
using DSGame.GraphSystem;
using System;

// Solution pour le problème de refresh sur les Branch https://stackoverflow.com/questions/48223969/onvaluechanged-for-fields-in-a-scriptable-object
[Serializable]
[NodeBox(style = NodeBox.StyleName.blue)]
[NodePin(nodePinsType = new NodePin.PinType[] { NodePin.PinType.receiver }, label = "Choices:")]
public class DemoNodeChoice : Node
{
    //TODO Zoom (dificult) https://cdn2.hubspot.net/hubfs/2603837/CustomZoomableEditorWindowsinUnity3D-2.pdf?t=1504038261535
    //TODO Improve the code
    [NodePin(nodePinsType = new NodePin.PinType[] { NodePin.PinType.caller })]
    [NodeFieldEditorChangeAction(OnChangeCall = "EditorUpdate")]
    public List<Branch> choices; //Fork

    //look to make this more generic and call a CustomPropertyDrawer conditionaly on the pin and activate it where branch is used
    //when the modification is detected make sure the controller to redraw the component 
    [NodePin(nodePinsType = new NodePin.PinType[] { NodePin.PinType.caller })]
    [NodeFieldEditorChangeAction(OnChangeCall = "EditorUpdate")]
    public Branch conditional;

    [NodePin(nodePinsType = new NodePin.PinType[] { NodePin.PinType.getter, NodePin.PinType.setter }, label = "parameters")]
    public string test = "dfdsf";
}