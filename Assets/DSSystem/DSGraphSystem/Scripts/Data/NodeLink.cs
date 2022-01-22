using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DSGame.GraphSystem
{
    //Link between node
    [Serializable]
    public class NodeLink
    {
        public enum LinkType { Set, Call, Portal }
        public LinkType linkType;
        public Node from;
        public Node to;
        public string fromPinId;
        public string toPinId;

        [NonSerialized]
        public ProcessStatus processStatus;
    }
}
