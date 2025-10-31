using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Target selection strategy that selects the first detected target
/// </summary>
[CreateAssetMenu(fileName = "First Seen Target Selector", menuName = "Scriptable Object/Component/Enemy/Target Selector/First Seen")]
public class FirstSeenTargetSelector : TargetSelector
{
    public override Collider SelectTarget(List<Collider> targets, Vector3 entityPosition)
    {
        if (targets == null || targets.Count == 0)
            return null;

        // Return the first valid target in the list
        foreach (Collider target in targets)
        {
            if (target != null)
                return target;
        }

        return null;
    }
}
