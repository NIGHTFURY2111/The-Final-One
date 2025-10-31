using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Target selection strategy that selects the nearest target
/// </summary>
[CreateAssetMenu(fileName = "Nearest Target Selector", menuName = "Scriptable Object/Component/Enemy/Target Selector/Nearest")]
public class NearestTargetSelector : TargetSelector
{
    public override Collider SelectTarget(List<Collider> targets, Vector3 entityPosition)
    {
        if (targets == null || targets.Count == 0)
            return null;

        if (targets.Count == 1)
            return targets[0];

        // Find nearest target
        Collider nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider target in targets)
        {
            if (target == null) continue;

            float distance = Vector3.Distance(entityPosition, target.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = target;
            }
        }

        return nearest;
    }
}
