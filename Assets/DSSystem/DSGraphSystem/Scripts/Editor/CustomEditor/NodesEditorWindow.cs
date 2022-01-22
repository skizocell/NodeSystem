using UnityEngine;
using UnityEditor;

namespace DSGame.GraphSystem
{
    //The Widndows used to draw our graph
    public class NodesEditorWindow : EditorWindow
    {
        #region private Variables
        static NodesEditorWindow curWindow;
        GraphControllerBase curGraphController = null;
        float zoomScale = 1f;

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

        private void OnGUI()
        {
            if (curGraphController == null) OpenAsset();
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);

            Event e = Event.current;

            Rect _zoomArea = new Rect(0.0f, 0.0f, Screen.width, Screen.height);

            EditorZoomArea.Begin(zoomScale, _zoomArea);
            BeginWindows();
            if (curGraphController != null) curGraphController.Update(e);
            EndWindows();
            EditorZoomArea.End();

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
            lastAssetOpen = AssetDatabase.GetAssetPath(controller.GetGraph().GetInstanceID());
        }

        private void ProcessEvents(Event e)
        {
            drag = Vector2.zero;
            switch (e.type)
            {
                //Manage zoom with scroll Wheel 
                case EventType.ScrollWheel:
                    if (e.delta.y < 0)
                    {
                        if (zoomScale < 1.0f) zoomScale += 0.1f;
                    }
                    else
                    {
                        if (zoomScale > 0.5f) zoomScale -= 0.1f;
                    }
                    Repaint();
                    break;
                case EventType.MouseDown:
                    if (e.button == 1)
                    {
                        //Left clic -> Display main menu
                        GenericMenu menu = new GenericMenu();
                        if (curGraphController != null)
                        {
                            curGraphController.FillMenu(menu, e.mousePosition/zoomScale);
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
                        //Right clic in empty space
                        if (curGraphController != null) curGraphController.UnSelectNode();
                        GUI.changed = true;
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 2)
                    {
                        //OnDrag with scroll wheel button pushed
                        OnDrag(e.delta);
                    }
                    break;
            }
        }

        //Drag the graph
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
            if(curGraphController != null) lastAssetOpen = NodesUtils.GetAssetPath(curGraphController.GetGraph());
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
