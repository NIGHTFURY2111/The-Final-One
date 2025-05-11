using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AC_Component: ScriptableObject
{
    [SerializeField] public abstract ComponentType componentType { get; }
}
