using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DSGame.GraphSystem;

namespace DSGame.GraphSystem
{
    public class NodeBox : Attribute
    {
        public enum StyleName { green, dark_blue, blue, turquoise, purple, red, orange }
        public StyleName style { get; set; }
        public int width { get; set; }
    }

    public class NodePin : Attribute
    {
        public enum PinType { caller, receiver, getter, setter }
        public PinType[] nodePinsType { get; set; }
        public string label { get; set; }
        public string id { get; set; }
    }

    public class BranchChange : Attribute
    {
        public string OnChangeCall { get; set; }
    }

    [Serializable]
    public abstract class Node : ScriptableObject
    {
        public Rect rect;
        [NonSerialized] public ProcessStatus processStatus;
        [NonSerialized] public bool isDirty;
    }

    [Serializable]
    public abstract class Node<G> : Node where G : Graph
    {
        public G graph;
    }
}

