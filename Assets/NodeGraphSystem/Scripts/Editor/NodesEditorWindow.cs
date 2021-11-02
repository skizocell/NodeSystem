using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Selection.activeObject = this; pour s�lectioner l'objet ad�quat dans l'inspecteur
//On peut aussi cr�er des custom inspector si on veut un formulaire sp�cifique.

//place code for Edito to an Editor folder to exclude them from build
//https://docs.unity3d.com/Manual/SpecialFolders.html
public class NodesEditorWindow : EditorWindow
{
    #region public Variables
    #endregion

    #region private Variables
    static NodesEditorWindow curWindow;
    GraphControllerBase curGraphController = null;

    [SerializeField]
    string lastAssetOpen; //https://blog.unity.com/technology/unity-serialization
    Vector2 offset;
    Vector2 drag;
    #endregion

    #region Unity editor Framework
    //Add menu item
    [MenuItem("Node Editor/Launch Editor")]
    public static void InitNodeEditor()
    {
        Debug.Log("InitNodeEditor");
        NodesEditorWindow.InitEditorWindow();
    }

    //Auto Open NodeGraph asset
    //https://www.csharpcodi.com/vs2/?source=5779/VisualNoiseDesigner/NodeEditor/Editor/NodeEditorWindow.cs
    [UnityEditor.Callbacks.OnOpenAsset(1)]
    public static bool AutoOpenCanvas(int instanceID, int line)
    {
        if (Selection.activeObject != null && Selection.activeObject.GetType() == typeof(NodeGraph))
        {
            NodesEditorWindow.InitEditorWindow();
            string NodeCanvasPath = AssetDatabase.GetAssetPath(instanceID);
            curWindow.lastAssetOpen = NodeCanvasPath;
            curWindow.OpenAsset();
            return true;
        }
        return false;
    }

    private void OpenAsset()
    {
        if(lastAssetOpen!=null)
        curGraphController = NodesUtils.LoadGraphController(lastAssetOpen);
    }

    private void OnGUI()
    {
        if (curGraphController == null) OpenAsset();
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        Event e = Event.current;
        BeginWindows();
        if (curGraphController!=null) curGraphController.Update(e);
        EndWindows();
        ProcessEvents(e);
        if (GUI.changed) Repaint();
    }
    #endregion

    #region Main methods

    #endregion


    #region Utility Methods
    public static void InitEditorWindow()
    {
        curWindow = EditorWindow.GetWindow<NodesEditorWindow>();
        curWindow.titleContent.text = "Node Editor";
    }

    private void InitGraphController(GraphControllerBase controller)
    {
        curGraphController = controller;
        lastAssetOpen = controller.GetAssetPath();
    }

    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 1)
                {
                    //Display main menu
                    GenericMenu menu = new GenericMenu();
                    if (curGraphController != null)
                    {
                        curGraphController.FillMenu(menu, e.mousePosition);
                    }
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Create Graph"), false, () => OnClicCreateGraph());
                    menu.AddItem(new GUIContent("Load Graph"), false, () => OnLoadGraph());
                    menu.AddItem(new GUIContent("Unload Graph"), false, () => OnUnloadGraph());
                    menu.ShowAsContext();
                    e.Use();
                }

                if (e.button == 0)
                {
                    if(curGraphController!=null) curGraphController.UnSelectNode();
                    GUI.changed = true;
                }
                break;

            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);
                }
                break;
        }
    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (curGraphController != null)
        {
            curGraphController.Drag(delta);
        }
        GUI.changed = true;
    }

    private void OnClicCreateGraph()
    {
        GraphCreationPopup.InitNodePopup((graph)=> InitGraphController(graph));
    }

    private void OnLoadGraph()
    {
        curGraphController = NodesUtils.LoadGraphController();
        lastAssetOpen = NodesUtils.GetAssetPath(curGraphController.GetGraph());
    }

    private void OnUnloadGraph()
    {
        curGraphController = null;
        lastAssetOpen = null;
    }

    //draw grid on the editor window
    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }
    #endregion
}