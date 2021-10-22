using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

public class DemoDialogNodeController : NodeControllerBase<DemoNodeDialog, DemoDialogGraphController>
{
    #region variable
    protected static Texture2D headerTexture=null;
    protected static GUIStyle headerStyle = null;

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
        setterPin = new NodePinSetterController("GetTest", this, false, 58);
        getterPin = new NodePinGetterController("SetTest", this, false, 37);

        AddNodePin(callerPin);
        AddNodePin(calledPin);
        AddNodePin(setterPin);
        AddNodePin(getterPin);
    }

    #region Utility method
    protected override void DrawWindowsContent()
    {
        EditorGUIUtility.labelWidth = 1;

        DemoDialog dialog = node.GetData();

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

    protected override Texture2D GetHeaderTexture()
    {
        if(headerTexture == null)
        {
            //Normal Style 
            headerTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            headerTexture.SetPixel(0, 0, new Color(0.25f, 0.4f, 0.25f));
            headerTexture.Apply();
        }
        return headerTexture;
    }

    protected override GUIStyle GetHeaderStyle()
    {
        if (headerStyle == null)
        {
            headerStyle = new GUIStyle();
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.normal.textColor = Color.white;
        }
        return headerStyle;
    }
    #endregion
}
