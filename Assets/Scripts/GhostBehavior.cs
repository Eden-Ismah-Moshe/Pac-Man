using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ghost))]
public abstract class GhostBehavior : MonoBehaviour
{
    public Ghost ghost { get; private set; }
    public float duration;

    private void Awake()
    {
        ghost = GetComponent<Ghost>();
    }

    public void Enable()
    {
        Enable(duration);
    }

    public virtual void Enable(float duration)
    {
        if (ghost == null || this == null)
        {
            return;
        }

        enabled = true;

        CancelInvoke(); 
    
        // Safely invoke the Disable method after the duration
        if (gameObject.activeInHierarchy) 
        {
            Invoke(nameof(Disable), duration);
        }
    }

    public virtual void Disable()
    {
        enabled = false;

        CancelInvoke();
    }
}
