using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DSGame.GraphSystem
{
    //Selection.activeObject = this; pour sélectioner l'objet adéquat dans l'inspecteur
    //On peut aussi créer des custom inspector si on veut un formulaire spécifique.

    //place code for Edito to an Editor folder to exclude them from build
    //https://docs.unity3d.com/Manual/SpecialFolders.html
    public class NodesEditorWindow : EditorWindow
    {
        #region public Variables
        #endregion

        #region private Variables
        //float zoomScale = 1.0f;
        static NodesEditorWindow curWindow;
        GraphControllerBase curGraphController = null;

        [SerializeField]
        string lastAssetOpen;
        Vector2 offset;
        Vector2 drag;
        #endregion

        #region Unity editor Framework
        //Add menu item
        [MenuItem("Node Editor/Launch Editor")]
        public static void InitNodeEditor()
        {
            NodesEditorWindow.InitEditorWindow();
        }

        //Auto Open NodeGraph asset
        //https://www.csharpcodi.com/vs2/?source=5779/VisualNoiseDesigner/NodeEditor/Editor/NodeEditorWindow.cs
        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool AutoOpenCanvas(int instanceID, int line)
        {
            if (Selection.activeObject != null && Selection.activeObject is Graph)
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
            if (lastAssetOpen != null)
                curGraphController = NodesUtils.LoadGraphController(lastAssetOpen);
        }

        //https://cdn2.hubspot.net/hubfs/2603837/CustomZoomableEditorWindowsinUnity3D-2.pdf?t=1504038261535
        private void OnGUI()
        {
            //zoomScale = EditorGUILayout.Slider("zoom", zoomScale, 1.0f / 25.0f, 2.0f);
            //float xFactor = Screen.width / 1024f * zoomScale;
            //float yFactor = Screen.height / 768f * zoomScale;
            //GUIUtility.ScaleAroundPivot(new Vector2(xFactor, yFactor), Vector2.zero);
            if (curGraphController == null) OpenAsset();
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);

            Event e = Event.current;
            BeginWindows();
            if (curGraphController != null) curGraphController.Update(e);
            EndWindows();
            ProcessEvents(e);
            if (GUI.changed) Repaint();
        }
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
                        if (curGraphController != null) curGraphController.UnSelectNode();
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
            GraphCreationPopup.InitNodePopup((graph) => InitGraphController(graph));
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

            //Handles.DrawLine(new Vector3(-20, 10, 0) + newOffset, new Vector3(20, position.height, 0f) );
            for (int i = -20; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
            }

            for (int j = -20; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }
        #endregion
    }
}
