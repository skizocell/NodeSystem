using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using DSGame.GraphSystem;

public class DemoDialogGraphController : GraphControllerBase
{
    #region private Variables
    private const string Name = "Demo Dialog Graph";
    private const string Description = "A Graph to create a dialog between character";
    private const string AssetPath = "Assets/DemoNodeSystem/Resources/";
    private const float Width = 200f;
    private const float Height = 65f;
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

    public override string GetNodeClassName()
    {
        return typeof(Graph).FullName;
    }
    #endregion

    #region main method
    //Add menu option to main menu
    public override void FillMenu(GenericMenu menu, Vector2 mousePosition)
    {
        base.FillMenu(menu, mousePosition);
        menu.AddItem(new GUIContent("Add Dialog"), false, (mousePos) => AddDialog((Vector2)mousePos), mousePosition);
        menu.AddItem(new GUIContent("Add Choice"), false, (mousePos) => AddChoice((Vector2)mousePos), mousePosition);
        menu.AddItem(new GUIContent("Execute"), false, Excecute);
    }
    #endregion

    #region Utility Methods
    private void AddDialog(Vector2 mousePos)
    {
        DemoNodeDialog dialog =  NodesUtils.CreateNode<DemoNodeDialog>(graph, new Rect(mousePos.x, mousePos.y, Width, Height));
        SetController(dialog);
    }

    private void AddChoice(Vector2 mousePos)
    {
        DemoNodeChoice choice = NodesUtils.CreateNode<DemoNodeChoice>(graph, new Rect(mousePos.x, mousePos.y, Width, Height));
        SetController(choice);
    }

    private void Excecute()
    {
        
    }

    #endregion
}