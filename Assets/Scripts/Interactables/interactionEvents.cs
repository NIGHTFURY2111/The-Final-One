using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class interactionEvents : MonoBehaviour
{
    [SerializeField] 
    private bool activated = false; 
    
    public UnityEvent onEveryHit;
    public UnityEvent onActivate;
    public UnityEvent onReset;
    
    public void OnHit()
    {
        onEveryHit?.Invoke();
        if (!activated)
        {
            onActivate?.Invoke();
            activated = true;
        }
    }

    public void ButtonReset()
    {
        if (activated)
        {
            onReset?.Invoke();
            activated = false;
        }
    }
}
