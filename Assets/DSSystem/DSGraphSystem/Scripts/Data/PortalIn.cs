using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DSGame.GraphSystem
{
    [NodeBox(style = NodeBox.StyleName.dark_blue)]
    public class PortalIn : Node
    {
        public Graph graph;
        [NodeDataShow]
        [NodePin(nodePinsType = new NodePin.PinType[] { NodePin.PinType.portalIn })]
        public PortalOut portalOut;
    }
}
