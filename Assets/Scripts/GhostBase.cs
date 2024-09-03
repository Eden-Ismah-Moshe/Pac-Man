using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBase : GhostBehavior
{
    public Transform inside;
    public Transform outside;

    private void OnEnable()
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        // Start the exit transition coroutine if the game object is still active
        if (gameObject.activeInHierarchy) {
            StartCoroutine(ExitTransition());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Reverse the ghost's movement direction when it collides with an obstacle
        // to simulate bouncing within the ghost home
        if (enabled && collision.gameObject.layer == LayerMask.NameToLayer("Obstacle")) {
            ghost.movement.SetDirection(-ghost.movement.Direction);
        }
    }

    private IEnumerator ExitTransition()
    {
        // Temporarily disable movement and make the ghost kinematic during the animation
        ghost.movement.SetDirection(Vector2.up, true);
        ghost.movement.Rb.isKinematic = true;
        ghost.movement.enabled = false;

        Vector3 position = transform.position;

        float duration = 0.5f;
        float elapsed = 0f;

        // Animate the ghost moving to the 'inside' position
        while (elapsed < duration)
        {
            ghost.SetPosition(Vector3.Lerp(position, inside.position, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;

        // Animate the ghost exiting the home and moving to the 'outside' position
        while (elapsed < duration)
        {
            ghost.SetPosition(Vector3.Lerp(inside.position, outside.position, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Set the ghost's direction randomly left or right and re-enable movement
        ghost.movement.SetDirection(new Vector2(Random.value < 0.5f ? -1f : 1f, 0f), true);
        ghost.movement.Rb.isKinematic = false;
        ghost.movement.enabled = true;
    }
    
}
