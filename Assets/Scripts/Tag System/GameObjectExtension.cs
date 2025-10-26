using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtension 
{

    public static bool HasTag(this GameObject gameObject, Enum_Tag tag) => HasTagAll(gameObject, tag);
    public static Enum_Tag CustomTag(this GameObject gameObject) 
    { if (gameObject.TryGetComponent<Tags>(out Tags tags))
            return tags.CustomTag();
        return 0;
    }
    public static bool HasTagAll(this GameObject gameObject, Enum_Tag tag)
    {
        return gameObject.TryGetComponent<Tags>(out Tags tags) && tags.HasTagAll(tag);
    }
    public static bool HasTagAny(this GameObject gameObject, Enum_Tag tag)
    {
        return gameObject.TryGetComponent<Tags>(out Tags tags) && tags.HasTagAny(tag);
    }
}
