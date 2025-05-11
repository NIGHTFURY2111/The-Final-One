using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Movement", menuName = "Scriptable Object/Component/Player Movement")]
public class EC_Movement : AC_Component
{
    public override ComponentType componentType => ComponentType.Movement;

    public InputAccessSO playerinput;


    public void Start()
    {
        
    }

    public void Update()
    {
        Debug.Log(playerinput.ListInputs());
    }
}
