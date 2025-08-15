using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tags : MonoBehaviour
{
    [SerializeField] private Enum_Tag customTag;
    public bool HasTag(Enum_Tag checkTag)
    {
        return customTag.HasFlag(checkTag);
    }
}
