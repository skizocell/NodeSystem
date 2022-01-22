using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace DSGame.GraphSystem
{
    //Node controller Generic used by default. Node annotation control is behavior
    public class NodeControllerGeneric<N> : NodeControllerBase<N> where N : Node
    {
        public List<string> labels;
        public NodeControllerGeneric(GraphControllerBase graphController, N node) : base(graphController, node)
        {
            RefreshController();
        }

        #region Main method
        protected override void RefreshController()
        {
            int yPos = 21;
            int offset = 20;
            labels = new List<string>();
            nodePins = new List<NodePinController>();

            //Check NodeBox attribute on Node class...
            NodeBox nodeBoxParam = (NodeBox)typeof(N).GetCustomAttribute(typeof(NodeBox), true);
            if (nodeBoxParam != null)
            {
                //Apply style parameter
                nodeStyle = DefaultNodeControllerStyles.GetStyle(nodeBoxParam.style);
                node.rect.width = nodeBoxParam.width == 0 ? 200 : nodeBoxParam.width;
            }

            //Check NodePin Attribute on Node Class
            NodePin nodePinParam = (NodePin)typeof(N).GetCustomAttribute(typeof(NodePin), true);
            if (nodePinParam != null)
            {
                //Create a label with the required pins
                labels.Add(nodePinParam.label);
                CreatePins("Node", node, nodePinParam, yPos);
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
                                    //create label and pin for branch
                                    CreatePinFromBranch(yPos, prop.Name, nodePinAttr, branch);
                                    yPos += offset;
                                }
                            }
                        }
                    }
                    //If it's a Branch
                    else if (typeof(Branch).IsAssignableFrom(prop.FieldType))
                    {
                        //create label and pin for branch
                        Branch branch = (Branch)prop.GetValue(node);
                        CreatePinFromBranch(yPos, prop.Name, nodePinAttr, branch);
                        yPos += offset;
                    }
                    else
                    {
                        String label = DataShowAttrProcess(prop, node);//if a a datshow attribute is present take the data.toString() value
                        if(label==null) label = prop.Name; //Else take the property name
                        if (nodePinAttr.label != null && nodePinAttr.label != String.Empty)//if an attribute is set take this
                            label = nodePinAttr.label;
                        
                        //create label and pin
                        labels.Add(label);
                        CreatePins(prop.Name, prop.GetValue(node), nodePinAttr, yPos);
                        yPos += offset;
                    }
                }
                else
                {
                    String label = DataShowAttrProcess(prop, node);
                    if (label != null)
                    {
                        labels.Add(label);
                        yPos += offset;
                    }
                }
            }

            //auto update height of the node
            node.rect.height = yPos;
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
        #endregion

        #region Utility method
        private String DataShowAttrProcess(FieldInfo prop, Node node)
        {
            NodeDataShow nodeDataShowAttr = (NodeDataShow)prop.GetCustomAttribute(typeof(NodeDataShow));
            if (nodeDataShowAttr != null)
            {
                System.Object data = prop.GetValue(node);
                return data==null ? null : data.ToString();
            }
            return null;
        }

        private void CreatePinFromBranch(int yPos, String fieldName, NodePin nodePinAttr, Branch branch)
        {
            labels.Add(branch.label); //add to the list to display
            CreatePins("(" + fieldName + ")$[" + branch.id + "]", branch, nodePinAttr, yPos);
        }

        private void CreatePins(String origin, System.Object obj, NodePin nodePinAttr, int yPosition)
        {
            for(int i = 0; i < nodePinAttr.nodePinsType.Length; i++)
            //foreach (NodePin.PinType pType in nodePinAttr.)
            {
                NodePin.PinType pType = nodePinAttr.nodePinsType[i];
                Nullable<bool> acceptMany = null;
                if (nodePinAttr.acceptMany != null && nodePinAttr.acceptMany.Length >= i) acceptMany = nodePinAttr.acceptMany[i];
                switch (pType)
                {
                    case NodePin.PinType.caller:
                        nodePins.Add(new NodePinCallerController("F$" + origin, this, acceptMany, yPosition));
                        break;
                    case NodePin.PinType.receiver:
                        nodePins.Add(new NodePinCalledController("T$" + origin, this, acceptMany, yPosition));
                        break;
                    case NodePin.PinType.getter:
                        nodePins.Add(new NodePinGetterController("T$" + origin, this, acceptMany, yPosition, obj.GetType()));
                        break;
                    case NodePin.PinType.setter:
                        nodePins.Add(new NodePinSetterController("F$" + origin, this, acceptMany, yPosition, obj.GetType()));
                        break;
                    case NodePin.PinType.portalOut:
                        nodePins.Add(new NodePinPortalOutController("F$" + origin, this, acceptMany, yPosition));
                        break;
                    case NodePin.PinType.portalIn:
                        nodePins.Add(new NodePinPortalInController("T$" + origin, this, acceptMany, yPosition));
                        break;
                    default:
                        break;
                }
                EditorUtility.SetDirty(graphController.GetGraph());
            }
        }
        #endregion
    }
}