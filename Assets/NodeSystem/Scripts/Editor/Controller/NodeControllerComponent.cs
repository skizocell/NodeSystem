using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NodeControllerComponent
{
    public bool isSelected = false;
    public abstract void Drag(Vector2 delta);
    public abstract void Update(int id, Event e, GUISkin skin);
    public abstract NodeComponent GetNode();
}
