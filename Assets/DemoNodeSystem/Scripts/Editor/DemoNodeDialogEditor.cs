using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DemoNodeDialog))]
public class DemoNodeDialogEditor : Editor
{
    public override void OnInspectorGUI() 
    {
        DemoNodeDialog nodeDialog = (DemoNodeDialog)target;
        nodeDialog.name = EditorGUILayout.TextField("Dialog name", nodeDialog.name);
        SerializedObject serializedObject = new UnityEditor.SerializedObject(nodeDialog);

        DemoDialog dialog = nodeDialog.data;
        EditorGUILayout.LabelField("Dialog: ", EditorStyles.boldLabel);
        dialog.text = EditorGUILayout.TextArea(dialog.text, GUILayout.MinHeight(200));
        dialog.parameter = EditorGUILayout.TextField("Parameter", dialog.parameter);

        //SerializedProperty pinCaller = serializedObject.FindProperty("nextCall");
        //EditorGUILayout.PropertyField(pinCaller);

        //SerializedProperty pinCalled = serializedObject.FindProperty("process");
        //EditorGUILayout.PropertyField(pinCalled);
    }
}
