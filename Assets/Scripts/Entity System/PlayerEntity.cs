using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerEntity : Entity
{
    public TMP_Text playerNameText;

    [SerializeField] private EC_Camera cameraSO;
    [SerializeField] private EC_Rigidbody rigidbodySO;

    AC_Component[] components;

    public Action OnStartTick;
    public Action OnUpdateTick;

    private void Awake()
    {
        MassAssign();
        EventLinker();
    }
    public override void Start()
    {
        OnStartTick?.Invoke();
        movementSO.ComponentStart();
    }

    public override void Update()
    {
        playerNameText.text = movementSO.stateManager._currentState.name;
        //movementSO.ComponentUpdate();
        OnUpdateTick?.Invoke();
    }

    public override void EventLinker()
    {
        movementSO.OnCameraMove += cameraSO.UpdateCameraTransform;
        movementSO.OnCameraMove += rigidbodySO.RotateGameObject;

    }

    public void MassAssign()
    {
        components = new AC_Component[]
        {
            cameraSO,
            rigidbodySO,
            movementSO
        };

        foreach (AC_Component component in components)
        {
            component.entity = this;
            component.ComponentAwake();

            OnStartTick += component.ComponentStart;
            OnUpdateTick += component.ComponentUpdate;
        }
    }

}
