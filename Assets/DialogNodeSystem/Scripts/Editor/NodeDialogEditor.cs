using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NodeDialog))]
public class NodeDialogEditor : Editor
{
    public override void OnInspectorGUI() 
    {
        NodeDialog nodeDialog = (NodeDialog)target;
        nodeDialog.name = EditorGUILayout.TextField("Dialog name", nodeDialog.name);
        SerializedObject serializedObject = new UnityEditor.SerializedObject(nodeDialog);

        Dialog dialog = nodeDialog.GetData();
        EditorGUILayout.LabelField("Dialog: ", EditorStyles.boldLabel);
        dialog.text = EditorGUILayout.TextArea(dialog.text, GUILayout.MinHeight(200));

        SerializedProperty pinCaller = serializedObject.FindProperty("nextCall");
        EditorGUILayout.PropertyField(pinCaller);

        SerializedProperty pinCalled = serializedObject.FindProperty("process");
        EditorGUILayout.PropertyField(pinCalled);
    }
}
