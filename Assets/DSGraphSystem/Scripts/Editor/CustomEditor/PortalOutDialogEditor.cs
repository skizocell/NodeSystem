
using UnityEngine;
using UnityEditor;

namespace DSGame.GraphSystem
{
    [CustomEditor(typeof(PortalOut))]
    public class DemoNodeDialogEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            PortalOut portalOut = (PortalOut)target;
            portalOut.name = EditorGUILayout.TextField("Portal name", portalOut.name);
            SerializedObject serializedObject = new UnityEditor.SerializedObject(portalOut);
        }
    }
}
