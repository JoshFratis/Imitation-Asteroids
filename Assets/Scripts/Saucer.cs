using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saucer : MonoBehaviour
{
    // Scripts
    private GameManager gameManager;

    // Objects
    [SerializeField] GameObject oLaser;
    [SerializeField] GameObject oRagdoll;
    [SerializeField] GameObject playerShip;

    // Components
    private Transform tf;
    private Rigidbody2D rb2D;

    private Transform playerShipTf;

    // Coordinates
    private float xExtent;
    private float yExtent;
    private float upperBound;
    private float lowerBound;
    private float posBuffer = 2.0f;
    private float posX;
    private float posY;

    private float size;

    // Variables
    // Intervals
    [SerializeField] float projectileInterval = 1;
    [SerializeField] float moveInterval = 1;

    // Speed Ranges
    [SerializeField] float xSpeedMin = 200;
    [SerializeField] float xSpeedMax = 400;
    [SerializeField] float ySpeedMin = 0;
    [SerializeField] float ySpeedMax = 400;

    // Position
    private float xDir;
    private float yDir;

    [SerializeField] bool isSmall;
    public bool respawnable = true;

    void Awake()
    {
        // Scripts
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        // Components
        tf = gameObject.GetComponent<Transform>();
        rb2D = gameObject.GetComponent<Rigidbody2D>();

        playerShipTf = playerShip.GetComponent<Transform>();

        // Variables
        size = tf.localScale.x;

        if (isSmall)
        {
            tf.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        }
        else
        {
            tf.localScale = new Vector3(1.25f, 1.25f, 1.25f);
        }
    }

    void OnEnable()
    {
        // Screen Y
        yExtent = Camera.main.GetComponent<Camera>().orthographicSize; 
        lowerBound = -yExtent + posBuffer;
        upperBound = yExtent - posBuffer;

        // Position Y
        posY = Random.Range(lowerBound, upperBound);

        // Screen X
        xExtent = yExtent / Screen.height * Screen.width;

        // Position X
        float side = Mathf.Sign(Random.Range(-1, 1));
        posX = side * (xExtent + posBuffer);

        // Position
        tf.position = new Vector3(posX, posY, 0);

        // Direction
        xDir = -Mathf.Sign(posX);

        // Enable Projectiles
        StartCoroutine(FireProjectile());

        // Enable Movement
        StartCoroutine(Move());
    }

    void Update()
    {
        // Reached the opposite side of the screen
        if (Mathf.Abs(tf.position.x) > Mathf.Abs(posX))
        {
            Debug.Log("Saucer reached edge.");
            respawnable = true;
            gameObject.SetActive(false);
        }
    }

    // Collision
    void OnTriggerEnter2D(Collider2D other)
    {
        // Hit by Saucer
        if (other.gameObject.tag == "Saucer")
        {
            //Debug.Log("Saucer hit Saucer");
            other.gameObject.GetComponent<Saucer>().OnHit();
            OnHit();
        }
        // Hit by Asteroid
        else if (other.gameObject.tag == "Asteroid")
        {
            //Debug.Log("Saucer hit Asteroid");
            other.gameObject.GetComponent<Asteroid>().OnHit();
            OnHit();
        }
    }  
 
    public void OnHit()
    {
        //Debug.Log("Saucer Hit");
        GameObject ragdoll = Instantiate(oRagdoll, tf.position, tf.rotation) as GameObject;
        ragdoll.GetComponent<Transform>().localScale = tf.localScale;
        respawnable = false;
        gameObject.SetActive(false);
    }

    public void AwardPoints()
    {
        switch (size)
        {
            case 1.0f:
                gameManager.AddScore(1000);
                break;
            case 2.0f:
                gameManager.AddScore(200);
                break;
            default:
                gameManager.AddScore(200);
                Debug.Log("Saucer of unexpected size ("+size+") destroyed.");
                break;
        }
    }

    // Projectile
    IEnumerator FireProjectile()
    {
        while (true)
        {
            yield return new WaitForSeconds(projectileInterval + Random.Range(projectileInterval * -0.25f, projectileInterval * 0.25f));

            float dir;
            if (isSmall)
            {   
                // Accuracy
                int maxScore = 60000;
                int initialInacurracy = 30;
                float accuracy = Mathf.Max(0, (100000 - gameManager.score) / (maxScore / initialInacurracy));      // Decrease random range of inaccuracy (initial - 0) as score increases (0 - max)
                float inaccuracy = Random.Range(-accuracy, accuracy);     // Add inaccuracy within random range to shot

                // Aim
                Vector2 lineOfSight = new Vector2(playerShipTf.position.x - tf.position.x, playerShipTf.position.y - tf.position.y);    // Get vector from saucer to ship
                dir = Vector2.SignedAngle(Vector2.up, lineOfSight);     // Get angle of vector from saucer to ship
                dir += inaccuracy;                                      // Add random inaccuray to aim

                // any other differences between small + large asteroid? warrant different scripts / prefabs at all?
                /* 
                    Debug.Log("Saucer position: " + tf.position.x + ", " + tf.position.y);
                    Debug.Log("Player position: " + playerShipTf.position.x + ", " + playerShipTf.position.y);
                    Debug.Log("Line of Sight: " + lineOfSight);
                    Debug.Log("Angle: " + dir);
                    Debug.Log("Projectile Heading: " + projectile.GetComponent<Transform>().eulerAngles.z);
                */ 
            }
            else
            {
                dir = Random.Range(0f, 360f);
            }

            GameObject projectile = Instantiate(oLaser, tf) as GameObject;                  // Create projectile
            projectile.GetComponent<Transform>().localScale = new Vector3(0.25f, 0.25f, 1f);     // Ensure all projectiles are the same size, regardless of parent transform
            projectile.GetComponent<Transform>().eulerAngles = new Vector3(0, 0, dir);     // Set projectile rotation to aim
            projectile.tag = "Non-Player Projectile";                                       // Tag projectile as non-player projectile
        }
    }

    // Movement
    IEnumerator Move()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveInterval + Random.Range(moveInterval * -0.25f, moveInterval * 0.25f));

            float ySpeed = Random.Range(ySpeedMin, ySpeedMax) * Mathf.Sign(Random.Range(-1, 1));
            float xSpeed = Random.Range(xSpeedMin, xSpeedMax) * xDir;

            rb2D.velocity = Vector2.zero;

            rb2D.AddForce(new Vector2(xSpeed, ySpeed));
        } 
    }
}
