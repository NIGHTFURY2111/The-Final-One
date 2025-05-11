using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerEntity : Entity
{
    public TMP_Text playerNameText;

    private void Awake()
    {
        movementSO.entity = this;
        movementSO.ComponentAwake();
    }
    public override void Start()
    {
        movementSO.Start();
    }

    public override void Update()
    {
        playerNameText.text = movementSO.stateManager._currentState.name;
        movementSO.Update();
    }

    public override void EventLinker()
    {
    }

}
