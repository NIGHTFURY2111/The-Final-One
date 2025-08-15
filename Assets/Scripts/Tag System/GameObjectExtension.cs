using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtension 
{
    public static bool HasTag(this GameObject gameObject, Enum_Tag tag)
    {
        return gameObject.TryGetComponent<Tags>(out Tags tags) && tags.HasTag(tag);
    }
}
