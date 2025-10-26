using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tags : MonoBehaviour
{
    [SerializeField] private Enum_Tag customTag;


    public bool HasTag(Enum_Tag checkTag) => HasTagAll(checkTag);
    public Enum_Tag CustomTag() => customTag;
    public bool HasTagAll(Enum_Tag checkTag)
    {
        return customTag.HasFlag(checkTag);
    }
    public bool HasTagAny(Enum_Tag checkTag)
    {
        return (customTag & checkTag) != 0;
    }

}
