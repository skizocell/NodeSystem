using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class NodeControllerBase<T> : NodeControllerComponent where T : NodeComponent
{
    #region public variable
    protected T node;
    protected String nodeStyle= "NodeDefault";
    protected String nodeSelected = "NodeSelected";
    protected GUIStyle curStyle;
    
    public Action<NodeControllerComponent> OnRemoveNode;
    public Action<NodeControllerComponent> OnSelect;
    #endregion

    public NodeControllerBase(T node, Action<NodeControllerComponent> OnClickRemoveNode, Action<NodeControllerComponent> OnSelect)
    {
        this.node = node;
        this.OnRemoveNode = OnClickRemoveNode;
        this.OnSelect = OnSelect;
    }

    #region Extensible method
    protected abstract void Draw();

    protected virtual void ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.Used:
                break;
            case EventType.MouseDown:
                if (e.button == 1 && isSelected && node.rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                if (e.button == 0 && node.rect.Contains(e.mousePosition))
                {
                    OnSelect(this);
                    e.Use();
                }
                break;
            case EventType.MouseDrag:
                if (e.button == 0 && isSelected)
                {
                    Drag(e.delta);
                    e.Use();
                }
                break;
        }
    }
    protected virtual void FillContextMenu()
    {

    }
    #endregion

    #region main method
    public override void Drag(Vector2 delta)
    {
        node.rect.position += delta;
    }
    #endregion

    #region Utility method
    public override void Update(int id, Event e, GUISkin skin)
    {
        Color savedColor = EditorStyles.label.normal.textColor;
        Color focusedColor = EditorStyles.label.focused.textColor;
        ProcessEvents(e);
        if (!isSelected)
            curStyle = skin.GetStyle(nodeStyle);
        else
            curStyle = skin.GetStyle(nodeSelected);

        GUILayout.Window(id, node.rect, (p) => {
            //GUILayout.BeginArea(new Rect(3, 10, rect.width-3, rect.height-3));
            //EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 80;
            EditorGUILayout.Space(20);
            EditorStyles.label.normal.textColor = curStyle.normal.textColor;
            EditorStyles.label.focused.textColor = curStyle.normal.textColor;
            Draw();
            EditorStyles.label.normal.textColor = savedColor;
            EditorStyles.label.focused.textColor=focusedColor;
            EditorGUIUtility.labelWidth = 0;
            
            //EditorGUILayout.EndHorizontal();

            //GUILayout.EndArea();
        }, node.name, curStyle, null);

        
    }

    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        FillContextMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    private void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode(this);
        }
    }

    public override NodeComponent GetNode()
    {
        return node;
    }
    #endregion
}
