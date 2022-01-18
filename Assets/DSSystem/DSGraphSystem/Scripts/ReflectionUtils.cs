using System.Reflection;
using UnityEngine;

public static class ReflectionUtils
{
    //With reflection get fielValue... 
    public static string GetObjectFieldValue(string fieldName, System.Object obj)
    {
        string value = string.Empty;
        if (obj == null)
        {
            Debug.Log("obj is null");
        }
        
        else if (fieldName == null)
        {
            //if no fieldname return object.tostring
            value = obj.ToString();
        }
        else
        {
            //else take the object field tagged by label parameter
            FieldInfo info = obj.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (info == null)
            {
                Debug.Log("Field (" + fieldName + ") not found in object of type " + obj.GetType());
            }
            else
            {
                System.Object val = info.GetValue(obj);
                if(val==null) Debug.Log("Field (" + fieldName + ") in object of type " + obj.GetType() + "is null");
                else value = val.ToString();
            }
        }
        return value;
    }
}
