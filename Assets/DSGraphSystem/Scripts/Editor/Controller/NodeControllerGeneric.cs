using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace DSGame.GraphSystem
{
    public class NodeControllerGeneric<N> : NodeControllerBase<N> where N : Node
    {
        public List<string> labels;
        public NodeControllerGeneric(GraphControllerBase graphController, N node) : base(graphController, node)
        {
            RefreshController();
        }

        private void RefreshController()
        {
            int yPos = 21;
            int offset = 20;
            labels = new List<string>();
            nodePins = new List<NodePinController>();

            //https://www.pluralsight.com/guides/how-to-create-custom-attributes-csharp

            //Check NodeBox attribute color on Node class...
            NodeBox nodeBoxParam = (NodeBox)typeof(N).GetCustomAttribute(typeof(NodeBox), true);
            if (nodeBoxParam != null)
            {
                nodeStyle = DefaultNodeControllerStyles.GetStyle(nodeBoxParam.style);
                node.rect.width = nodeBoxParam.width == 0 ? 200 : nodeBoxParam.width;
            }

            //Check NodePin Attribute on Node Class
            NodePin nodePinParam = (NodePin)typeof(N).GetCustomAttribute(typeof(NodePin), true);
            if (nodePinParam != null)
            {
                labels.Add(nodePinParam.label);
                CreatePins("Node", node, nodePinParam.nodePinsType, yPos);
                yPos += offset;
            }

            //Check NodePin Attribute on Fields
            foreach (FieldInfo prop in typeof(N).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                NodePin nodePinAttr = (NodePin)prop.GetCustomAttribute(typeof(NodePin));
                if (nodePinAttr != null)
                {
                    //If its a list
                    if (typeof(IList).IsAssignableFrom(prop.FieldType))
                    {
                        Type[] typeParameters = prop.FieldType.GetGenericArguments();
                        //If it's a list of Branch
                        if (typeof(Branch).IsAssignableFrom(typeParameters[0]))
                        {
                            //Get the fork and create pin
                            IList<Branch> fork = (IList<Branch>)prop.GetValue(node);
                            if ( fork != null)
                            {
                                foreach (Branch branch in (IEnumerable)prop.GetValue(node))
                                {
                                    CreatePinFromBranch(yPos, prop.Name, nodePinAttr, branch);
                                    yPos += offset;
                                }
                            }
                        }
                    }
                    else if (typeof(Branch).IsAssignableFrom(prop.FieldType))
                    {
                        Branch branch = (Branch)prop.GetValue(node);
                        CreatePinFromBranch(yPos, prop.Name, nodePinAttr, branch);
                        yPos += offset;
                    }
                    else
                    {
                        string label = prop.Name;
                        if (nodePinAttr.label != null && nodePinAttr.label != String.Empty)
                            label = nodePinAttr.label;
                        
                        labels.Add(label);
                        CreatePins(prop.Name, prop.GetValue(node), nodePinAttr.nodePinsType, yPos);
                        yPos += offset;
                    }
                }
            }

            //auto update height of the node
            node.rect.height = yPos;
        }

        private void CreatePinFromBranch(int yPos, String fieldName, NodePin nodePinAttr, Branch branch)
        {
            labels.Add(branch.label); //add to the list to display
            CreatePins("(" + fieldName + ")$[" + branch.id + "]", branch, nodePinAttr.nodePinsType, yPos);
        }

        private void CreatePins(String origin, System.Object obj, NodePin.PinType[] pTypes, int yPosition)
        {
            foreach (NodePin.PinType pType in pTypes)
            {
                switch (pType)
                {
                    case NodePin.PinType.caller:
                        nodePins.Add(new NodePinCallerController("F$" + origin, this, false, yPosition));
                        break;
                    case NodePin.PinType.receiver:
                        nodePins.Add(new NodePinCalledController("T$" + origin, this, true, yPosition));
                        break;
                    case NodePin.PinType.getter:
                        nodePins.Add(new NodePinGetterController("T$" + origin, this, true, yPosition, obj.GetType()));
                        break;
                    case NodePin.PinType.setter:
                        nodePins.Add(new NodePinSetterController("F$" + origin, this, true, yPosition, obj.GetType()));
                        break;
                    default:
                        break;
                }
                EditorUtility.SetDirty(graphController.GetGraph());
            }
        }

        protected override void DrawWindowsContent()
        {
            EditorGUIUtility.labelWidth = 1;
            foreach (string label in labels)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(15);
                EditorGUILayout.LabelField(label);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUIUtility.labelWidth = 0;
        }
    }
}