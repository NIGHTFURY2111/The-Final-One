using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All possible States that the Enemy can be in based on detection of target Entities
/// </summary>
public enum Enum_DetectionState
{//all possible states the enemy can be in based on detection of target entities
    Idle,
    Alerted,
    Chasing,
}


[CreateAssetMenu(fileName = "Enemy Detection Manager", menuName = "Scriptable Object/Component/Enemy/Enemy Detection Manager")]
public class DetectionManager : AC_Component
{
    public override Enum_ComponentType componentType => Enum_ComponentType.Detector;
    Dictionary<Enum_DetectionColliderType, DetectionCollider> DetectorDictionary = new();
    Dictionary<Enum_DetectionColliderType, DetectedGameObjectList> ColliderListsDictionary = new();
    public Action<Enum_DetectionState, Collider> OnStateChanged;
    private Collider targetedCollider;
    //fire an event whenever the value of currentstate is changed
    private Enum_DetectionState _currentState;
    public Enum_DetectionState CurrentState { get => _currentState;
        private set
        {
            if (_currentState != value)
            {
                _currentState = value;
                OnStateChanged?.Invoke(_currentState, targetedCollider);
            }
        }
    }



    public override void ComponentAwake() { }
    public override void ComponentDisable() 
    {
        foreach (DetectionCollider col in DetectorDictionary.Values)
        {
            col.OnColliderUpdate -= HandleDetectorUpdate;
        }

    }
    public override void ComponentStart()
    {
        BuildDictionary();
        AssignTagsAndSubscribe();
        CurrentState = Enum_DetectionState.Idle;
    }

    public override void ComponentUpdate() 
    {
    }
    void BuildDictionary()
    {
        DetectorDictionary.Clear();
        ColliderListsDictionary.Clear();

        List<DetectionCollider> colliders = new();
        entity.gameObject.GetComponentsInChildren<DetectionCollider>(colliders);
        foreach (DetectionCollider col in colliders)
        {
            if (col.detectionCollider == null) continue;

            DetectorDictionary.Add(col.detectionColliderType, col);
            ColliderListsDictionary.Add(col.detectionColliderType, col.triggerList);

        }
    }
    private void AssignTagsAndSubscribe()
    {
        Enum_Tag tags = ((Enemy_Entity)entity).targetTags;
        foreach (DetectionCollider col in DetectorDictionary.Values)
        {
            col.SetDetectionTags(tags);
            col.OnColliderUpdate += HandleDetectorUpdate;
        }
    }

    void HandleDetectorUpdate(Enum_DetectionColliderType type)
    {
        switch (type)
        {
            case Enum_DetectionColliderType.Vision_Cone:
                break;
            case Enum_DetectionColliderType.Close_Range:
                break;
            case Enum_DetectionColliderType.Chase_Zone:
                break;
            default:
                CurrentState = Enum_DetectionState.Idle;
                break;
        }
    }


    bool EnemyIncollider(Enum_DetectionColliderType type) => ColliderListsDictionary[type].list.Count > 0;

}

