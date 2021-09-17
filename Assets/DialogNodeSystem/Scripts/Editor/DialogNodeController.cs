using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class DialogNodeController : NodeControllerBase<NodeDialog>
{
    public DialogNodeController(NodeDialog node, Action<NodeControllerComponent> OnClickRemoveNode, Action<NodeControllerComponent> OnSelect) : base(node, OnClickRemoveNode, OnSelect)
    {
    }

    protected override void Draw()
    {
        Dialog dialog = node.GetData();
        dialog.text = EditorGUILayout.TextField("Dialog: ", dialog.text);
    }
}
