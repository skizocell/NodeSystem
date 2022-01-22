using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace DSGame.GraphSystem
{
    //Class used to manage all technic action on Graph and Node scriptable object (crate update delete)
    public static class NodesUtils
    {
        #region main method
        //create Graph
        public static Graph CreateGraph(string assetFilePath, GraphControllerBase nodeController)
        {
            Graph graph = (Graph)ScriptableObject.CreateInstance(nodeController.GetGraphClassName());
            graph.nodeGraphControllerType = nodeController.GetNodeGraphControllerType();
            AssetDatabase.CreateAsset(graph, assetFilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return graph;
        }

        //create Node
        public static T CreateNode<T>(Graph graph, Rect rect) where T : Node
        {
            T node = ScriptableObject.CreateInstance<T>();
            node.name = typeof(T).Name;
            node.rect = rect;

            AssetDatabase.AddObjectToAsset(node, graph);
            graph.AddNode(node);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return node;
        }

        //Delete Node
        public static void DeleteNode(Graph nodeGraph, Node node)
        {
            if (nodeGraph != null)
            {
                foreach (NodeLink link in nodeGraph.GetLinks().Where(l => l.to == node || l.from == node).ToList())
                {
                    nodeGraph.RemoveLink(link);
                }
                nodeGraph.RemoveNode(node);

                GameObject.DestroyImmediate(node, true);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        public static GraphControllerBase LoadGraphController(string graphPath)
        {
            return ConstructController(LoadGraph(graphPath));
        }

        public static GraphControllerBase LoadGraphController()
        {
            return ConstructController(LoadGraph());
        }

        public static string GetAssetPath(ScriptableObject obj)
        {
            return AssetDatabase.GetAssetPath(obj);
        }

        public static Dictionary<string, GraphControllerBase> GetAvailableGraphControllers()
        {
            Dictionary<string, GraphControllerBase> graphTypes = new Dictionary<string, GraphControllerBase>();
            //Get all implementation of GraphControllerBase interface 
            foreach (Type type in System.Reflection.Assembly.GetExecutingAssembly()
                                    .GetTypes().Where(type => typeof(GraphControllerBase)
                                    .IsAssignableFrom(type) && type.IsClass && !type.IsAbstract))
            {
                GraphControllerBase controller = (GraphControllerBase)Activator.CreateInstance(type);
                //search a method called GetName
                if (controller == null)
                {
                    Debug.LogError("Failed to instantiate a controller of type->" + type);
                }
                else
                {
                    //Invoke the GetName method and fill the dictionary
                    string name = controller.GetName();
                    if (graphTypes.ContainsKey(name))
                    {
                        Debug.LogError("Each class implementing GraphControllerBase must have an unique name returned by getName() static function. The duplicate element is ignored for (" + name + ")");
                    }
                    else
                    {
                        graphTypes[name] = controller;
                    }
                }
            }
            return graphTypes;
        }
        #endregion

        #region Utility Methods
        private static GraphControllerBase ConstructController(Graph graph)
        {
            if (graph != null)
            {
                GraphControllerBase controller = GetGraphController(graph.nodeGraphControllerType);
                if (controller != null) controller.SetGraph(graph);
                return controller;
            }
            return null;
        }

        //Choose a file and then open graph
        private static Graph LoadGraph()
        {
            string graphPath = EditorUtility.OpenFilePanel("Load Graph", Application.dataPath + "/NodeEditor/Database", "");
            if (graphPath == null || graphPath == "") return null;
            int appPathLen = Application.dataPath.Length;
            string finalPath = graphPath.Substring(appPathLen - 6);
            return LoadGraph(finalPath);
        }

        //Load NodeGraph
        private static Graph LoadGraph(string graphPath)
        {
            Graph curGraph = null;
            curGraph = (Graph)AssetDatabase.LoadAssetAtPath(graphPath, typeof(Graph));
            return curGraph;
        }

        //Get Graphcontroller by is type
        private static GraphControllerBase GetGraphController(string graphType)
        {
            foreach (Type type in System.Reflection.Assembly.GetExecutingAssembly()
                                    .GetTypes().Where(type => typeof(GraphControllerBase)
                                    .IsAssignableFrom(type) && type.IsClass && !type.IsAbstract))
            {
                GraphControllerBase controller = (GraphControllerBase)Activator.CreateInstance(type);
                //search a method called GetName
                if (controller == null)
                {
                    Debug.LogError("Failed to instantiate a controller of type->" + type);
                }
                else
                {
                    if (controller.GetNodeGraphControllerType().Equals(graphType))
                    {
                        return controller;
                    }
                }
            }
            return null;
        }
        #endregion
    }
}
