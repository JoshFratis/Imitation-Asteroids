using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Random;

public class Asteroid : MonoBehaviour
{
    // GameObjects
    [SerializeField] GameObject prefab;
    private GameManager gameManager;

    // Components
    private Transform tf;
    private Rigidbody2D rb2D;
    private SpriteRenderer sr;

    // Variables
    private float velocityLinear;
    private float velocityAngular;
    private Vector2 direction;
    private Vector3 rotation;
    private Vector3 position;

    [SerializeField] float maxVelocity;

    void Awake()
    {
        // Identify Components
        tf = gameObject.GetComponent<Transform>();
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();

        // Identify Objects & Scripts
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        // Generate Random 
        //position = new Vector3(Random.Range(-40,40), Random.Range(-20,20), 0);
        rotation = new Vector3(0f, 0f, Random.Range(0f, 360f));
        direction = new Vector2(Random.Range(0.01f, 1), Random.Range(0.01f, 1f));
        velocityLinear = Random.Range(100f, maxVelocity);
        velocityAngular = Random.Range(1, 100);
        
        /*
        Debug.Log("Rotation: " + rotation);
        Debug.Log("Direction: " + direction);
        Debug.Log("Linear Velocity: " + velocityLinear);
        Debug.Log("Angular Velocity: " + velocityAngular);
        */
    }

    void Start()
    {
        //tf.position = position;
        tf.eulerAngles = rotation;
        rb2D.AddForce(direction * velocityLinear);
        rb2D.AddTorque(velocityAngular);
        sr.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.75f, 0.75f);
    }

   // Split Asteroid
    public void OnHit()
    {
        //Debug.Log("Asteroid Hit.");
       
        // Split asteroid
        float newSize = tf.localScale.x / 2;
        if (newSize >= 1)
        {
            tf.localScale = new Vector3(newSize, newSize, newSize);
            
            GameObject asteroid = Instantiate(prefab) as GameObject;
            gameManager.AddAsteroid(asteroid);
            asteroid = Instantiate(prefab) as GameObject;
            gameManager.AddAsteroid(asteroid);
            Debug.Log("Splitting asteroid to size of " + newSize);
        }

        gameObject.SetActive(false);
    }

    public void AwardPoints()
    {
        float size = tf.localScale.x;

        switch (size)
        {
            case 4.0f:
                gameManager.AddScore(20);
                break;
            case 2.0f:
                gameManager.AddScore(50);
                break;
            case 1.0f:
                gameManager.AddScore(100);
                break;
            default:
                gameManager.AddScore(20);
                Debug.Log("Asteroid of unexpected size "+size+" destroyed.");
                break;
        }
    }
}
