using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

public static class NodesUtils
{
    #region main method
    public static NodeGraph CreateGraph(string assetFilePath, string nodeGraphType)
    {
        NodeGraph graph = ScriptableObject.CreateInstance<NodeGraph>();
        graph.nodeGraphControllerType = nodeGraphType;
        AssetDatabase.CreateAsset(graph, assetFilePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return graph;
    }

    public static T CreateNode<T>(NodeGraph graph, Rect rect) where T : NodeComponent
    {
        Type nodeType = typeof(T);
        T node = ScriptableObject.CreateInstance<T>();
        node.name = nodeType.Name;
        node.rect = rect;

        AssetDatabase.AddObjectToAsset(node, graph);
        graph.AddNode(node);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return node;
    }

    public static void DeleteNode(NodeGraph nodeGraph, NodeComponent node)
    {
        if (nodeGraph != null)
        {
            foreach(NodeLink link in nodeGraph.GetLinks().Where(l => l.to == node || l.from == node).ToList())
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
    private static GraphControllerBase ConstructController(NodeGraph graph)
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
    private static NodeGraph LoadGraph()
    {
        string graphPath = EditorUtility.OpenFilePanel("Load Graph", Application.dataPath + "/NodeEditor/Database", "");

        int appPathLen = Application.dataPath.Length;
        string finalPath = graphPath.Substring(appPathLen - 6);
        return LoadGraph(finalPath);
    }

    //Load NodeGraph
    private static NodeGraph LoadGraph(string graphPath)
    {
        NodeGraph curGraph = null;
        curGraph = (NodeGraph)AssetDatabase.LoadAssetAtPath(graphPath, typeof(NodeGraph));
        if (curGraph == null)
        {
            EditorUtility.DisplayDialog("Node Message", "Unable to load selected graph", "Ok");
        }
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
