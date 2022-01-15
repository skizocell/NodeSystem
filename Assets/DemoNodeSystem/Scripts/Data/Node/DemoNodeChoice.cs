using System.Collections.Generic;
using DSGame.GraphSystem;
using System;

//Exemple node with annotation
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
}