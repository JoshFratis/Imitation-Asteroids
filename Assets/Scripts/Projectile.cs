using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Variables
    [SerializeField] float velocity;
    [SerializeField] float duration;

    // Components
    private Transform tf;
    private Rigidbody2D rb2D;

    void Awake()
    {
        // Identify Components
        tf = gameObject.GetComponent<Transform>();
        rb2D = gameObject.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, duration);
        rb2D.AddForce(tf.up * velocity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.tag == "Player Projectile")
        {
            if (other.gameObject.tag == "Saucer")
            {
                //Debug.Log("Player Projectile hit Saucer");
                other.gameObject.GetComponent<Saucer>().OnHit();
                other.gameObject.GetComponent<Saucer>().AwardPoints();
                Destroy(gameObject, 0f);
            }
            else if (other.gameObject.tag == "Asteroid")
            {
                //Debug.Log("Player Projectile hit Asteroid");
                other.gameObject.GetComponent<Asteroid>().OnHit();
                other.gameObject.GetComponent<Asteroid>().AwardPoints();
                //Debug.Log("Projectile Hit.");
                Destroy(gameObject, 0f);
            }
        }
        else if (gameObject.tag == "Non-Player Projectile")
        {
            if (other.gameObject.tag == "Ship")
            {
                //Debug.Log("Non-Player Projectile hit Ship.");
                other.gameObject.GetComponent<Ship>().OnHit();
                Destroy(gameObject, 0f);
            }
            else if (other.gameObject.tag == "Asteroid")
            {
                //Debug.Log("Non-Player Projectile hit Asteroid");
                other.gameObject.GetComponent<Asteroid>().OnHit();
                Destroy(gameObject, 0f);
            }
        }
    }
}