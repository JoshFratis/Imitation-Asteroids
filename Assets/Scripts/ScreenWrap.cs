using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWrap : MonoBehaviour
{
    private float xExtent;
    private float yExtent;
    private float leftBound;
    private float rightBound;
    private float upperBound;
    private float lowerBound;

    private Transform tf;
    private float buffer = 2.5f;

    void Start()
    {
        // turn this into global variables, maybe separate script w public variables given to spawners

        // Assumes camera starts at origin
        yExtent = Camera.main.GetComponent<Camera>().orthographicSize; 
        xExtent = yExtent / Screen.height * Screen.width;
        leftBound = -xExtent - buffer;
        rightBound = xExtent + buffer;
        lowerBound = -yExtent - buffer;
        upperBound = yExtent + buffer;

        /*
        Debug.Log("xExtent: " + xExtent);
        Debug.Log("yExtent: " + yExtent);
        Debug.Log("leftBound: " + leftBound);
        Debug.Log("rightBound: " + rightBound);
        Debug.Log("upperBound: " + upperBound);
        Debug.Log("lowerBound: " + lowerBound);
        */
    }

    void Awake()
    {
        tf = gameObject.GetComponent<Transform>();
    }

    void Update()
    {
        if (tf.position.x < leftBound) 
        {
            tf.position = new Vector3(rightBound, tf.position.y, tf.position.z);
            //Debug.Log(gameObject.name + " Exceeded Left Bound.");
        }
        if (tf.position.x > rightBound) 
        {
            tf.position = new Vector3(leftBound, tf.position.y, tf.position.z);
            //Debug.Log(gameObject.name + " Exceeded Right Bound.");
        }
        if (tf.position.y < lowerBound) 
        {
            tf.position = new Vector3(tf.position.x, upperBound, tf.position.z);
            //Debug.Log(gameObject.name + " Exceeded Lower Bound.");
        }
        if (tf.position.y > upperBound) 
        {
            tf.position = new Vector3(tf.position.x, lowerBound, tf.position.z);
            //Debug.Log(gameObject.name + " Exceeded Upper Bound.");
        }
        //Debug.Log(gameObject.name + " position: " + tf.position);
    }
}