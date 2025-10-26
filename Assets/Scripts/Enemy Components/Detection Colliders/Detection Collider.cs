using System;
using UnityEngine;


public class DetectionCollider : MonoBehaviour
{
    //reference to the collider
    public Collider detectionCollider { get; private set; }

    // Type of detection collider and list to its detected objects
    public Enum_DetectionColliderType detectionColliderType;
    [HideInInspector] public DetectedGameObjectList triggerList;

    // Event invoked when the collider's detected objects list is updated
    public Action<Enum_DetectionColliderType> OnColliderUpdate;

    private Enum_Tag DetectionTags = 0;
    
    void Awake()
    {
        Collider tempCollider;
        TryGetComponent<Collider>(out tempCollider);
        detectionCollider = tempCollider;
        triggerList = ScriptableObject.CreateInstance<DetectedGameObjectList>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.HasTagAny(DetectionTags))
        {
            triggerList.list.Add(other);
            OnColliderUpdate?.Invoke(detectionColliderType);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.HasTagAny(DetectionTags))
        {
            triggerList.list.Remove(other);
            OnColliderUpdate?.Invoke(detectionColliderType);
        }
    }
    public void SetDetectionTags(Enum_Tag tags)
    {
        if (DetectionTags != 0) return;
        DetectionTags = tags;
    }
}

public enum Enum_DetectionColliderType
{
    Vision_Cone = 1 << 0,
    Close_Range = 1<<1,
    Chase_Zone = 1<<2,
}