using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Target : MonoBehaviour
{
    // Components
    private SpriteRenderer sr;

    private void Awake()
    {
        // Identify Components
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    // Collision
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Hit by Laser
        if (other.gameObject.tag == "Laser")
        {
            OnHit();
        }
    }

    // Set color to green
    private void OnHit()
    {
        sr.color = Color.green;
        StartCoroutine(Wait(0.75f));
    }

    // Reset color to white
    private void Reset()
    {
        sr.color = Color.white;
    }

    // Wait for duration before resetting
    IEnumerator Wait(float duration)
    {
        yield return new WaitForSeconds(duration);
        Reset();
    }
}
