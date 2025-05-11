using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerEntity : Entity
{
    public override void Start()
    {
    }

    public override void Update()
    {
        movementSO.Update();
    }

    public override void EventLinker()
    {
    }

}
