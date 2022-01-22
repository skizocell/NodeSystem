using UnityEngine;
using UnityEditor;
using DSGame.GraphSystem;

//Exemple custom Graph controller
public class DemoDialogGraphController : GraphControllerBase
{
    #region private Variables
    private const string Name = "Demo Dialog Graph";
    private const string Description = "A Graph to create a dialog between character";
    private const string DefaultSaveFolderPath = "Assets/DSSystem/DemoNodeSystem/Resources/";
    private const float Width = 200f;
    private const float Height = 45f;
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

    public override string GetGraphClassName()
    {
        return typeof(DemoNodeGraph).FullName;
    }

    public override string GetDefaultSaveFolderPath()
    {
        return DefaultSaveFolderPath;
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