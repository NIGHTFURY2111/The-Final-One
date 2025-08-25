using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.HasTag(Enum_Tag.entity))
        {
        Debug.Log("DeathPlane Triggered by " + other.gameObject.name);
            other.gameObject.GetComponent<PlayerEntity>().Ondeath();
        }
    }
}
