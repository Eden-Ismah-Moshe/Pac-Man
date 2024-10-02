using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScared : GhostBehavior
{
    public SpriteRenderer body;
    public SpriteRenderer eyes;
    public SpriteRenderer blue;
    public SpriteRenderer white;

    private bool eaten;

    public override void Enable(float duration)
    {
        base.Enable(duration);

        if (this != null)
        {
            // Hide the normal ghost appearance and show the blue 'scared' sprite
            if (body != null) body.enabled = false;
            if (eyes != null) eyes.enabled = false;
            if (blue != null) blue.enabled = true;
            if (white != null) white.enabled = false;
        }
        if (this != null) // Check again before invoking
        {
            // Hide the normal ghost appearance and show the blue 'scared' sprite
            Invoke(nameof(Flash), duration / 2f);
        }
    }

    public override void Disable()
    {
        base.Disable();

        // Restore the normal ghost appearance when the scared state ends
        if (body != null) body.enabled = true;
        if (eyes != null) eyes.enabled = true;
        if (blue != null) blue.enabled = false;
        if (white != null) white.enabled = false;
    }

    private void Eaten()
    {
        eaten = true;
        // Send the ghost back to its home when eaten by Pacman
        ghost.SetPosition(ghost.home.inside.position);
        ghost.home.Enable(duration);

        // Show only the eyes after being eaten
        if (body != null) body.enabled = false;
        if (eyes != null) eyes.enabled = true;
        if (blue != null) blue.enabled = false;
        if (white != null) white.enabled = false;
    }

    private void Flash()
    {
       
        if (!eaten && this != null)
        {
            // Switch from blue to white to create a flashing effect
            if (blue != null) blue.enabled = false;
            if (white != null)
            {
                white.enabled = true;
                white.GetComponent<AnimatedSprite>().Restart();
            }
        }
    }

    private void OnEnable()
    {
        // Restart the blue animation and reduce the ghost's speed
        //blue.GetComponent<AnimatedSprite>().Restart();
        
        if (blue != null) blue.GetComponent<AnimatedSprite>().Restart();

        ghost.movement.speedMultiplier = 0.5f;
        eaten = false;
    }

    private void OnDisable()
    {
        // Restore the ghost's speed to normal when the behavior is disabled
        ghost.movement.speedMultiplier = 1f;
        eaten = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();

        if (node != null && enabled)
        {
            Vector2 direction = Vector2.zero;
            float maxDistance = float.MinValue;

            // Find the direction that moves the ghost farthest from Pacman
            foreach (Vector2 availableDirection in node.availableDirections)
            {
                Vector3 newPosition = transform.position + new Vector3(availableDirection.x, availableDirection.y);
                float distance = (ghost.target.position - newPosition).sqrMagnitude;

                // Update the direction if this one is farther from Pacman
                if (distance > maxDistance)
                {
                    direction = availableDirection;
                    maxDistance = distance;
                }
            }

            ghost.movement.SetDirection(direction);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If the ghost collides with Pacman while scared, mark it as eaten
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman"))
        {
            if (enabled) {
                Eaten();
            }
        }
    }
    
    private void OnDestroy()
    {
        // Cancel any Invoke calls related to this object
        CancelInvoke();
    }

     
}
