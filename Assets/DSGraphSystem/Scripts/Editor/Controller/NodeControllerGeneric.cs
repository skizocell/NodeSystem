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
                    if (typeof(IList).IsAssignableFrom(prop.FieldType))
                    {
                        if (prop.GetValue(node) != null)
                        {
                            foreach (var obj in (IEnumerable)prop.GetValue(node))
                            {
                                string id = ReflectionUtils.GetObjectFieldValue(nodePinAttr.id, obj);
                                string label = ReflectionUtils.GetObjectFieldValue(nodePinAttr.label, obj);

                                labels.Add(label); //add to the list to display
                                CreatePins("(" + prop.Name + ")$[" + id + "]", obj, nodePinAttr.nodePinsType, yPos);
                                yPos += offset;
                            }
                        }
                    }
                    else
                    {
                        string label = prop.Name;
                        if (nodePinAttr.label != null && nodePinAttr.label != String.Empty)
                            label = nodePinAttr.label;
                        
                        labels.Add(label);
                        CreatePins(label, prop.GetValue(node), nodePinAttr.nodePinsType, yPos);
                        yPos += offset;
                    }
                }
            }

            //auto update height of the node
            node.rect.height = yPos;
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