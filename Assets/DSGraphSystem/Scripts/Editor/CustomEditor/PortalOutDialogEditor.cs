
using UnityEngine;
using UnityEditor;

namespace DSGame.GraphSystem
{
    //Custom inspector screen for portalout
    [CustomEditor(typeof(PortalOut))]
    public class PortalOutDialogEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            PortalOut portalOut = (PortalOut)target;
            portalOut.name = EditorGUILayout.TextField("Portal name", portalOut.name);
            SerializedObject serializedObject = new UnityEditor.SerializedObject(portalOut);
        }
    }
}
