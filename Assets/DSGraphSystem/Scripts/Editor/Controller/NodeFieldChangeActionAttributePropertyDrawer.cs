using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace DSGame.GraphSystem
{
    [CustomPropertyDrawer(typeof(NodeFieldEditorChangeActionAttribute))]
    public class NodeFieldChangeActionAttributePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int totalLine = 1;
            if (property.isExpanded)
            {
                totalLine = property.CountInProperty();
            }

            return EditorGUIUtility.singleLineHeight * totalLine + EditorGUIUtility.standardVerticalSpacing * (totalLine - 1);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, new GUIContent(label), true);
            if (EditorGUI.EndChangeCheck())
            {
                NodeFieldEditorChangeActionAttribute at = attribute as NodeFieldEditorChangeActionAttribute;
                IEnumerable<MethodInfo> methods = property.serializedObject.targetObject.GetType().GetMethods().Where(m => m.Name == at.OnChangeCall);
                if (methods.Count() != 1)
                    Debug.LogError("No or more than one method named " + at.OnChangeCall + "is found for " + label + " field");
                else
                {
                    MethodInfo method = methods.First();

                    if (method != null && method.GetParameters().Count() == 0)// Only instantiate methods with 0 parameters
                        method.Invoke(property.serializedObject.targetObject, null);
                }
            }
        }
    }
}