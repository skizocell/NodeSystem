using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DSGame.GraphSystem
{
    [Serializable]
    public class NodeLink
    {
        public enum LinkType { Set, Call }
        public LinkType linkType;
        public Node from;
        public Node to;
        public string fromPinId;
        public string toPinId;

        [NonSerialized]
        public ProcessStatus processStatus;
    }
}
