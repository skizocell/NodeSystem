using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

public class DialogNodeController : NodeControllerBase<NodeDialog, DialogGraphController>
{
    #region variable
    protected static Texture2D headerTexture=null;
    protected static GUIStyle headerStyle = null;

    NodePinCallerController callerPin;
    NodePinCalledController calledPin;
    #endregion

    public DialogNodeController(DialogGraphController graphController, NodeDialog node) : base(graphController,node)
    {
        //method 1 activate rich text
        //style = new GUIStyle();
        //style.richText = true;
        
        callerPin = new NodePinCallerController("Next", this, false,21);
        calledPin = new NodePinCalledController("Previous", this, true,21);

        AddNodePin(callerPin);
        AddNodePin(calledPin);

        //PropertyInfo propertyInfo = typeof(DialogNodeController).GetProperty(nameof(Test));
        //PinAttribute pinType = (PinAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(PinAttribute));
        //Debug.Log("Attribute = " + pinType.pintype);
    }

    #region main method
    protected override void Draw()
    {
        //Trick to draw hover a window declare a window without style and draw a box with window style 
        //but problem when selected because the box is not focused 
        //The windows skin is always on top 
        Rect windowRect = GUILayout.Window(nodeWindowsDrawId, node.rect, DrawWindowsContent, "", GUIStyle.none);
        GUI.Box(windowRect, "", GUI.skin.window);
        DrawPin(windowRect);
    }
    #endregion

    #region Utility method
    private void DrawWindowsContent(int windowsId)
    {
        //Draw Header with parent function
        DrawHeader();

        //Content
        EditorGUIUtility.labelWidth = 1;

        Dialog dialog = node.GetData();

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
