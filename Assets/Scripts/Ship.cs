using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ship : MonoBehaviour
{
    // Controls
    public PlayerInput controls;

    private InputAction thrust;
    private InputAction rotate;
    private InputAction fire;
    private InputAction teleport;

    // GameObjects
    private UIManager uiManager;
    [SerializeField] GameObject ragdoll;

    // Components
    private Transform tf;
    private Rigidbody2D rb2D;
    private Collider2D col2D;

    [SerializeField] GameObject shipSpriteObject;
    [SerializeField] GameObject thrustObject;
    private SpriteRenderer thrustSprite;

    [SerializeField] GameObject laserTemplate;

    // Speed Variables
    [SerializeField] float rotateAccel;
    [SerializeField] float rotateMaxVelocity;
    [SerializeField] float thrustAccel; 
    [SerializeField] float thrustMaxVelocity;
    private float teleportBuffer = 2.5f;

    private float sqrThrustMaxVelocity;

    // Other Variables
    [SerializeField] int totalLives;
    private int lives;

    private float respawnBufferDuration;
    private float invulnDuration;
    private float iFrameDuration;
    public bool isInvulnerable = false;

    private void Awake()
    {
        // Initialize Controls
        controls = new PlayerInput();

        // Get Objects & Scripts 
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();

        // Identify Components
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        col2D = gameObject.GetComponent<Collider2D>();
        tf = gameObject.GetComponent<Transform>();
        thrustSprite = thrustObject.GetComponent<SpriteRenderer>();

        // Calculate Secondary Speed Variables
        sqrThrustMaxVelocity = Mathf.Pow(thrustMaxVelocity, 2.0f);

        // Initialize Other Variables
        respawnBufferDuration = 1.0f;
        invulnDuration = 1.0f;
        iFrameDuration = 0.1f;
    }

    private void OnEnable()
    {
        Debug.Log("player enable");
        // Assign Control Variables for Action Callbacks
        thrust = controls.Player.Thrust;
        rotate = controls.Player.Rotate;
        fire = controls.Player.Fire;
        teleport = controls.Player.Teleport;

        // Enable Control Variables
        thrust.Enable();
        rotate.Enable();
        fire.Enable();
        teleport.Enable();

        // Subscribe Control Variables for Action Callbacks to Events 
        fire.performed += Fire;
        teleport.performed += Teleport;

        lives = totalLives;
        StartCoroutine(Respawn());
    }

    private void OnDisable()
    {
        // Disable Control Variables
        thrust.Disable();
        rotate.Disable();
        fire.Disable();
        teleport.Disable();
    }

    void FixedUpdate()
    {
        // Thrust
        if (thrust.ReadValue<float>() > 0) 
        {
            if (rb2D.velocity.sqrMagnitude < sqrThrustMaxVelocity) 
            {
                rb2D.AddForce(tf.up * thrustAccel);
            }
            thrustSprite.color = Color.red;
        }
        else if (thrust.ReadValue<float>() == 0) 
        {
            thrustSprite.color = Color.black;
        }

        // Rotation
        // add torque on press down, negate on release? 
        if (rotate.ReadValue<float>() < 0) 
        {
            // Rotate Left
            rb2D.angularVelocity = rotateAccel;
        }
        else if (rotate.ReadValue<float>() > 0) 
        {
            // Rotate Right
            rb2D.angularVelocity = -rotateAccel;
        }
        else if (rotate.ReadValue<float>() == 0)
        {
            rb2D.angularVelocity = 0;
        }
        
        // Report Velocieties
        //Debug.Log("Velocity: " + rb2D.velocity + ". Angular Velocity: " + rb2D.angularVelocity);

        /* NOTES
            in "Asteroids" rotation differs from acceleration in that it does not conserve momentum
            we can experiment with this
            but in imitating the original, we may need to reset the angular momentum to 0, if rotate returns 0 / Rotate returns cancelled (similar to capping thrust velocity)
        */
        /*
            bc this is a 2Drb, and can only rotate on one axis, torque is a float not a 2Dvector
            still, we need to differentiate between positive and negative torque
        */
    }

    private void Fire(InputAction.CallbackContext context)
    {
        GameObject projectile = Instantiate(laserTemplate, tf) as GameObject;
        projectile.tag = "Player Projectile";
    }

    private void Teleport(InputAction.CallbackContext context)
    {
        float yExtent = Camera.main.GetComponent<Camera>().orthographicSize; 
        float xExtent = yExtent / Screen.height * Screen.width;
        float xPos = Random.Range(-xExtent + teleportBuffer, xExtent - teleportBuffer);
        float yPos = Random.Range(-yExtent + teleportBuffer, yExtent - teleportBuffer);
        tf.position = new Vector3(xPos, yPos, 0);
    }

    public void AddLives(int newLives)
    {
        lives += newLives;
        uiManager.UpdateLives(lives);
    }

    // Collision
    /* 
        except for the isInvulnerable check
            which is redundant anyway, bc same check happens in OnHit()
        Saucer has the same code
        so could be abstracted
    */
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isInvulnerable == false)
        {
            // Hit by Saucer
            if (other.gameObject.tag == "Saucer")
            {
                //Debug.Log("Ship hit Saucer");
                other.gameObject.GetComponent<Saucer>().OnHit();
                OnHit();
            }
            // Hit by Asteroid
            else if (other.gameObject.tag == "Asteroid")
            {
                //Debug.Log("Ship hit Asteroid");
                other.gameObject.GetComponent<Asteroid>().OnHit();
                OnHit();
            }
        }
    }

    public void OnHit()
    {
        if (isInvulnerable == false)
        {
            //Debug.Log("Ship Hit.");
            lives--;
            Instantiate(ragdoll, tf.position, tf.rotation);

            if (lives >= 0)
            {
                StartCoroutine(Respawn());
            }
            else 
            {
                //Debug.Log("Out of Lives. Destroying Ship.");
                gameObject.SetActive(false);
            }
        }
    }

    public IEnumerator Respawn()
    {
        // 'Remove' Ship
        isInvulnerable = true;
        shipSpriteObject.SetActive(false);
        fire.performed -= Fire;
        uiManager.UpdateLives(lives);

        yield return new WaitForSeconds(respawnBufferDuration);

        // Reset Transform
        tf.position = Vector3.zero;
        tf.eulerAngles = Vector3.zero;

        // Reset Velocities
        rb2D.velocity = Vector2.zero;
        rb2D.angularVelocity = 0;

        // Flashing Effect
        for (float i = 0; i < invulnDuration; i += iFrameDuration)
        {
            if (shipSpriteObject.activeSelf == true)
            {
                shipSpriteObject.SetActive(false);
            }
            else if (shipSpriteObject.activeSelf == false)
            {
                shipSpriteObject.SetActive(true);
            }
            yield return new WaitForSeconds(iFrameDuration);
        }
        
        // Reset for Normal Play
        isInvulnerable = false;
        shipSpriteObject.SetActive(true);
        gameObject.SetActive(true);
        fire.performed += Fire;
    }
}
