using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;

/*
 * Popup to create a graph for one of the GraphControllerBase implementation
 */
public class GraphCreationPopup : EditorWindow
{
    #region private Variables
    static GraphCreationPopup curPopup;
    string wantedName = "Enter a name...";
    private int selectedTypeInex = 0;
    String[] controllersNames;
    GraphControllerBase[] controllers;
    Action<GraphControllerBase> callBack;
    //IEnumerable<GraphControllerBase> graphControllerTypes;
    #endregion

    #region Main methods
    public static void InitNodePopup(Action<GraphControllerBase> callBack)
    {
        curPopup = EditorWindow.GetWindow<GraphCreationPopup>();
        curPopup.titleContent.text = "Node Popup";
        curPopup.callBack = callBack;
        curPopup.Init();
    }
    #endregion

    #region private methods
    private void Init()
    {
        Dictionary<string, GraphControllerBase>  graphTypes = NodesUtils.GetAvailableGraphControllers();

        if (graphTypes.Count > 0)
        {
            controllersNames = new string[graphTypes.Count];
            controllers = new GraphControllerBase[graphTypes.Count];
            int i = 0;
            foreach (string graphName in graphTypes.Keys.OrderBy(s => s))
            {
                controllersNames[i] = graphName;
                controllers[i] = graphTypes[graphName];
                i++;
            }
            selectedTypeInex = 0;
        }
    }

    private void OnGUI()
    {
        if (controllersNames.Count() > 0)
        {
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);

            GUILayout.BeginVertical();
            EditorGUILayout.LabelField("Create New Graph:", EditorStyles.boldLabel);
            wantedName = EditorGUILayout.TextField("Enter Name:", wantedName);
            EditorGUILayout.LabelField("Graph Type:", EditorStyles.boldLabel);
            selectedTypeInex = EditorGUILayout.Popup(selectedTypeInex, controllersNames);
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Graph", GUILayout.Height(40)))
            {
                if (!string.IsNullOrEmpty(wantedName) && wantedName != "Enter a name...")
                {
                    GraphControllerBase controller = controllers[selectedTypeInex];
                    string newAssetFullPath = controller.GetAssetPath() + wantedName + ".asset";

                    //Search asset with name like wanted Name -> where assetPath == newAssetfull path
                    bool isAlreadyExist = AssetDatabase.FindAssets(wantedName, new[] { controller.GetAssetPath() })
                                            .Where(s => AssetDatabase.GUIDToAssetPath(s).Equals(newAssetFullPath))
                                            .Count() > 0;
                    if (isAlreadyExist)
                    {
                        EditorUtility.DisplayDialog("Info", "The name (" + wantedName + ") already exist in " + controllers[selectedTypeInex].GetAssetPath(), "Ok");
                    }
                    else
                    {
                        callBack(GetGraph(newAssetFullPath, controller));
                        curPopup.Close();
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("Node message", "Please enter a valid Graph Name", "Ok");
                }
            }
            if (GUILayout.Button("Cancel", GUILayout.Height(40)))
            {
                curPopup.Close();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(20);
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
        }
        else
        {
            EditorGUILayout.LabelField("No implementation of class GraphControllerBase was found !!!", EditorStyles.boldLabel);
        }
    }

    public GraphControllerBase GetGraph(string path, GraphControllerBase controller)
    {
        controller.SetGraph(NodesUtils.CreateGraph(path, controller.GetNodeGraphControllerType()));
        return controller;
    }
    #endregion
}
