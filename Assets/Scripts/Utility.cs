using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Utility{
    public static T PickRandom<T>(T[] objects)
    {
        if (objects.Length < 1)
        {
            return default(T);
        }

        var index = Random.Range(0, objects.Length);
        return objects[index];
    }

    public static T PopRandom<T>(ref List<T> objects)
    { 
        var index = Random.Range(0, objects.Count);
        var returnElement = objects[index];
        objects.Remove(returnElement);
        return returnElement;
    }

}
