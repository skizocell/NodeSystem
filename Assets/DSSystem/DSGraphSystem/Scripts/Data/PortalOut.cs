using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DSGame.GraphSystem
{
    //Portal Out Node to return backward in the graph
    [NodeBox(style = NodeBox.StyleName.dark_blue)]
    [NodePin(nodePinsType = new NodePin.PinType[] { NodePin.PinType.portalOut })]
    public class PortalOut : Node
    {
    }
}
