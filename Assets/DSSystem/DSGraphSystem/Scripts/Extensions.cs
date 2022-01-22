using System.Reflection;
using System;
using UnityEngine;

namespace DSGame.GraphSystem
{
    //Extension for Reflection
    public static class ReflectionExtensions
    {
        //Get a field in the parent class
        public static FieldInfo GetDeepField(this Type type, String name, BindingFlags flags)
        {
            if (type == typeof(System.Object)) return null;

            FieldInfo info = type.GetField(name, flags);
            if (info == null) info = type.BaseType.GetDeepField(name, flags);
            Debug.Log(info);
            return info;
        }
    }

    //Extension for Rect
    public static class RectExtensions
    {
        //Make a rect between 2 point in from upper left
        public static Rect MakeRect(Vector2 point1, Vector2 point2)
        {
            float startX, startY, endX, endY;
            if (point1.x <= point2.x)
            {
                startX = point1.x;
                endX = point2.x;
            }
            else
            {
                endX = point1.x;
                startX = point2.x;
            }
            if (point1.y <= point2.y)
            {
                startY = point1.y;
                endY = point2.y;
            }
            else
            {
                endY = point1.y;
                startY = point2.y;
            }
            ///////////////
            return new Rect(startX, startY, endX - startX, endY - startY);
        }
    }
}
