using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ReflectionExtensions
{
    //public static IList<FieldInfo> GetAllFields(this Type type, BindingFlags flags)
    //{
    //    if (type == typeof(System.Object)) return new List<FieldInfo>();

    //    List<FieldInfo> list = type.BaseType.GetAllFields(flags).ToList();
    //    // in order to avoid duplicates, force BindingFlags.DeclaredOnly
    //    list.AddRange(type.GetFields(flags | BindingFlags.DeclaredOnly));
    //    return list;
    //}

    public static FieldInfo GetDeepField(this Type type, String name, BindingFlags flags)
    {
        Debug.Log("Search " + name + " in type " + type );
        if (type == typeof(System.Object)) return null;

        FieldInfo info = type.GetField(name, flags);
        if (info == null) info = type.BaseType.GetDeepField(name, flags);
        Debug.Log(info);
        return info;
    }
}
