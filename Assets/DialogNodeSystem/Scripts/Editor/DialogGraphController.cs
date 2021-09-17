using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogGraphController : GraphControllerBase
{
    #region private Variables
    private const string Name = "Dialog Graph";
    private const string Description = "A Graph to create a dialog between character";
    private const string AssetPath = "Assets/DialogNodeSystem/Resources/";
    private const string NodeGraphControllerType = "DialogGraphController";

    #endregion

    #region MetaData info method
    public override string GetName()
    {
        return Name;
    }

    public override string GetDescription()
    {
        return Description;
    }

    public override string GetAssetPath()
    {
        return AssetPath;
    }

    public override string GetNodeGraphControllerType()
    {
        return NodeGraphControllerType;
    }
    #endregion

    #region main method
    //Add menu option to main menu
    public override void FillMenu(GenericMenu menu, Vector2 mousePosition)
    {
        menu.AddItem(new GUIContent("Add Dialog"), false, (mousePos) => AddDialog((Vector2)mousePos), mousePosition);
    }
    #endregion

    #region Utility Methods
    private void AddDialog(Vector2 mousePos)
    {
        NodeComponent dialog =  NodesUtils.CreateNode(graph, typeof(NodeDialog), new Rect(mousePos.x, mousePos.y, 200f, 65f));
        SetController(dialog);
    }
    #endregion
}