using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public float speed = 8f;
    public float speedMultiplier = 1f;
    public Vector2 initialDirection;
    public LayerMask obstacleLayer;
    
    public Rigidbody2D Rb { get; private set; }
    public Vector2 Direction { get; private set; }
    public Vector2 NextDirection { get; private set; }
    public Vector3 StartingPosition { get; private set; }
    
    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        StartingPosition = transform.position;
    }

    private void Start()
    {
        ResetState();
    }

    private void Update()
    {
        // to make the movements more responsive we will set the direction while it's queued        
        if(NextDirection != Vector2.zero)
        {
            SetDirection(NextDirection);
        }
    }

    public void ResetState()
    {
        speedMultiplier = 1f;
        Direction = initialDirection;
        NextDirection = Vector2.zero;
        transform.position = StartingPosition;
        Rb.isKinematic = false; // Enable physics (for let the ghosts go through walls)
        enabled = true;
    }

    private void FixedUpdate()
    {
        Vector2 position = Rb.position;
        Vector2 movement = Direction * (speed * speedMultiplier * Time.fixedDeltaTime);
        Vector2 newPosition = position + movement;
        Rb.MovePosition(newPosition);
    }
    
    public void SetDirection(Vector2 newDirection, bool forced = false)
    {
        // if the direction is Occupied (by tile) we set this as next direction 
        // so it'll automatically be set when it does become available
        if (forced || !Occupied(newDirection))
        {
            Direction = newDirection;
            NextDirection = Vector2.zero;   
        }
        else
        {
            NextDirection = newDirection;
        }
    }           
    
    public bool Occupied(Vector2 direction)
    {
        // If no collider is hit then there is no obstacle in that direction
        RaycastHit2D hit = Physics2D.BoxCast(Rb.position, Vector2.one * 0.75f, 0f, direction, 1.5f, obstacleLayer);
        return hit.collider != null;
    }
    
}
