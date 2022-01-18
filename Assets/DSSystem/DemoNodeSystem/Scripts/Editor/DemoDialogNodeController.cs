using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using DSGame.GraphSystem;

//Exemple custom Controller
public class DemoDialogNodeController : NodeControllerBase<DemoNodeDialog>
{
    #region variable
    NodePinCallerController callerPin;
    NodePinCalledController calledPin;
    #endregion

    public DemoDialogNodeController(DemoDialogGraphController graphController, DemoNodeDialog node) : base(graphController,node)
    {
        //method 1 activate rich text
        //style = new GUIStyle();
        //style.richText = true;
        callerPin = new NodePinCallerController("Next", this, false,21);
        calledPin = new NodePinCalledController("Call", this, true,21);

        AddNodePin(callerPin);
        AddNodePin(calledPin);
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

        //EditorGUILayout.BeginHorizontal();
        //GUILayout.Space(12);
        //EditorGUILayout.LabelField("Test Link input :", EditorStyles.boldLabel);
        //EditorGUILayout.EndHorizontal();

        //EditorGUILayout.BeginHorizontal();
        //GUILayout.Space(12);
        //EditorGUILayout.LabelField("Test Link output :", EditorStyles.boldLabel);
        //EditorGUILayout.EndHorizontal();

        EditorGUIUtility.labelWidth = 0;
    }

    protected override void RefreshController()
    {
        throw new NotImplementedException();
    }
    #endregion
}
