using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using DSGame.GraphSystem;

public class DemoDialogNodeController : NodeControllerBase<DemoNodeDialog>
{
    #region variable
    NodePinCallerController callerPin;
    NodePinCalledController calledPin;
    NodePinSetterController setterPin;
    NodePinGetterController getterPin;
    #endregion

    public DemoDialogNodeController(DemoDialogGraphController graphController, DemoNodeDialog node) : base(graphController,node)
    {
        //method 1 activate rich text
        //style = new GUIStyle();
        //style.richText = true;
        callerPin = new NodePinCallerController("Next", this, false,21);
        calledPin = new NodePinCalledController("Call", this, true,21);
        setterPin = new NodePinSetterController("GetTest", this, false, 58, typeof(String));
        getterPin = new NodePinGetterController("SetTest", this, false, 37, typeof(String));

        AddNodePin(callerPin);
        AddNodePin(calledPin);
        AddNodePin(setterPin);
        AddNodePin(getterPin);
    }

    #region Utility method
    protected override void DrawWindowsContent()
    {
        EditorGUIUtility.labelWidth = 1;

        DemoDialog dialog = node.data;

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(15);
        EditorGUILayout.LabelField(dialog.GetFirstLine());
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(12);
        EditorGUILayout.LabelField("Test Link input :", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(12);
        EditorGUILayout.LabelField("Test Link output :", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUIUtility.labelWidth = 0;
    }
    #endregion
}
