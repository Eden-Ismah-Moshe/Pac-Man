using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Ghost : MonoBehaviour
{ 
    public Movement movement { get; private set; }
    public GhostBase home { get; private set; }
    public GhostScatter scatter { get; private set; }
    public GhostHunt chase { get; private set; }
    public GhostScared frightened { get; private set; }
    
    public GhostBehavior initBehavior;
    public Transform target;
    public int points = 200;

    private void Awake()
    {
        movement = GetComponent<Movement>();
        home = GetComponent<GhostBase>();
        scatter = GetComponent<GhostScatter>();
        chase = GetComponent<GhostHunt>();
        frightened = GetComponent<GhostScared>();
    }

    private void Start()
    {
        ResetState();
    }

    public void ResetState()
    {
        gameObject.SetActive(true);
        movement.ResetState();

        frightened.Disable();
        chase.Disable();
        scatter.Enable();

        if (home != initBehavior) {
            home.Disable();
        }

        if (initBehavior != null) {
            initBehavior.Enable();
        }
    }

    public void SetPosition(Vector3 position)
    {   
        // The z-position determines draw depth so we need to keep it the same
        position.z = transform.position.z;
        transform.position = position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            if (frightened.enabled) {
                GameManager.Instance.GhostEaten(this);
            } else {
                GameManager.Instance.PacmanEaten();
            }
        }
    }

    
}
