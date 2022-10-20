using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private Rigidbody2D rb2D;

    private float velocityLinear;
    private float velocityAngular;
    private Vector2 direction;
    
    void Awake()
    {
        // Identify Components
        rb2D = gameObject.GetComponent<Rigidbody2D>();

        // Generate Random 
        direction = new Vector2(Random.Range(0.01f, 1), Random.Range(0.01f, 1f));
        velocityLinear = Random.Range(50f, 200f);
        velocityAngular = Random.Range(1, 100);
    }

    void Start()
    {
        rb2D.AddForce(direction * velocityLinear);
        rb2D.AddTorque(velocityAngular);
        StartCoroutine(Despawn());
    }

    IEnumerator Despawn()
    {  
        // flashing effect? 

        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}