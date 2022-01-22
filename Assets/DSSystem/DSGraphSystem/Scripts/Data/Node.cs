using System;
using UnityEngine;

namespace DSGame.GraphSystem
{
    #region Attribute
    //NodeBox attribute to change color...
    public class NodeBox : Attribute
    {
        public enum StyleName { green, dark_blue, blue, turquoise, purple, red, orange }
        public StyleName style { get; set; }
        public int width { get; set; }
    }

    //NodePin attribute to create NodePin on a node
    public class NodePin : Attribute
    {
        public enum PinType { caller, receiver, getter, setter, portalIn, portalOut }
        public PinType[] nodePinsType { get; set; }
        public bool[] acceptMany { get; set; }
        public string label { get; set; }
    }

    //NodeDataShow to show data in the node 
    public class NodeDataShow : Attribute
    {
        public string data { get; set; }
    }

    //To activate Node draw refresh on change in the tagged field
    public class NodeFieldEditorChangeActionAttribute : PropertyAttribute
    {
        public string OnChangeCall { get; set; }
    }
    #endregion

    //The Node Base class
    [Serializable]
    public abstract class Node : ScriptableObject
    {
        public Rect rect;
        [NonSerialized] public ProcessStatus processStatus;
        [NonSerialized] public bool isEditorUpdateNeeded;

        public void EditorUpdate()
        {
            isEditorUpdateNeeded = true;
        }
    }

    //Node base class + ref to the graph (not yet used)
    [Serializable]
    public abstract class Node<G> : Node where G : Graph
    {
        public G graph;
    }
}

