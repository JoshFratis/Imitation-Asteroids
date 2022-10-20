using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaucerSpawner : MonoBehaviour
{
    public int scoreThresholdLower;
    public int scoreThresholdUpper;

    private bool respawning = false;
    private int respawnCounter;

    [SerializeField] float respawnBuffer = 30.0f;
    [SerializeField] int respawnRate;

    public GameObject saucerObject;
    private Saucer saucerScript;

    void Awake()
    {
        saucerScript = saucerObject.GetComponent<Saucer>();
    }

    void OnDisable()
    {
        saucerObject.SetActive(false);
    }

    void Update()
    {
        if (saucerObject.activeSelf == false)
        {
            if (saucerScript.respawnable == true)
            {
                respawnCounter += Random.Range(1, respawnRate / 1000);   
                if (respawnCounter >= respawnRate)
                {
                    Debug.Log("Saucer Spawned.");
                    saucerObject.SetActive(true);
                    respawnCounter = 0;
                }
            }
            else if (saucerScript.respawnable == false)
            {
                if (respawning == false)
                {
                    StartCoroutine(RespawnBufferTimer());
                }
            }
        }
    }

    public IEnumerator RespawnBufferTimer()
    {
        respawning = true;
        Debug.Log("Saucer waiting to respawn...");
        yield return new WaitForSeconds(respawnBuffer);
        Debug.Log("Saucer respawnable...");
        saucerScript.respawnable = true;
        respawning = false;
    }
}
