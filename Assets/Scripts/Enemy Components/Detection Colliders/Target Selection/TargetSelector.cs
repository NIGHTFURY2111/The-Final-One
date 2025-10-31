using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Abstract base class for target selection strategies
/// </summary>
public abstract class TargetSelector : ScriptableObject
{
    /// <summary>
    /// Selects a target from the provided list based on the strategy
    /// </summary>
    /// <param name="targets">List of available target colliders</param>
    /// <param name="entityPosition">Position of the entity doing the selection</param>
    /// <returns>Selected target collider, or null if no valid target</returns>
    public abstract Collider SelectTarget(List<Collider> targets, Vector3 entityPosition);
}
