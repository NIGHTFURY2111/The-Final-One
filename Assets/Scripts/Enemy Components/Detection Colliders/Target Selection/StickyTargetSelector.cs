using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Target selection strategy that locks onto the first seen target and maintains that lock
/// until the target is lost, then locks onto a new target
/// </summary>
[CreateAssetMenu(fileName = "Sticky Target Selector", menuName = "Scriptable Object/Component/Enemy/Target Selector/Sticky")]
public class StickyTargetSelector : TargetSelector
{
    private Collider lockedTarget;

    /// <summary>
    /// Clears the locked target (call when entering idle state)
    /// </summary>
    public void ClearLockedTarget()
    {
        lockedTarget = null;
    }

    public override Collider SelectTarget(List<Collider> targets, Vector3 entityPosition)
    {
        if (targets == null || targets.Count == 0)
        {
            return null;
        }

        // If we have a locked target and it's still in the list, keep it
        if (lockedTarget != null && targets.Contains(lockedTarget))
        {
            return lockedTarget;
        }

        // Otherwise, lock onto first valid target
        foreach (Collider target in targets)
        {
            if (target != null)
            {
                lockedTarget = target;
                return target;
            }
        }

        return null;
    }
}
