using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Entity : MonoBehaviour
{

    public PlayerInputAccessSO AccessPlayerInput;
    [SerializeField] TMP_Text text;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text.text = AccessPlayerInput.ListInputs();
    }
}
