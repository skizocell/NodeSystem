using UnityEngine;

namespace DSGame.GraphSystem
{
    //Base Node Controller Component
    public abstract class NodeControllerComponent
    {
        public bool isSelected = false;
        public abstract void Drag(Vector2 delta);
        public abstract void Update(int id, Event e, bool canDrag);
        public abstract Node GetNode();
        public abstract NodePinController GetControllerFor(string key);
        public abstract void OnClickNodePin(NodePinController nodepin);
    }
}
