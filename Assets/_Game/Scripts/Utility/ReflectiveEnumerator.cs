using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class ReflectiveEnumerator
{
    public static List<T> GetClassesOfType<T>(params object[] constructorArgs) where T : class
    {
        List<T> objects = new List<T>();
        foreach (Type type in Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
        {
            objects.Add((T)Activator.CreateInstance(type, constructorArgs));
        }

        return objects;
    }
}
