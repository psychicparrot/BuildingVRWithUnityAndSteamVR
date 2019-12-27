using UnityEngine;
using System.Collections;

public class RandomRotation : MonoBehaviour {

    public Vector3 rotationAxis;
    public float frameTime = 0.2f;

    private int currentFrame;
    private Transform myTransform;

    void Start()
    {
        myTransform = GetComponent<Transform>();

        InvokeRepeating("NextFrame", Random.Range(0, frameTime), frameTime);
    }

    void NextFrame()
    {
        myTransform.Rotate(rotationAxis * (Random.Range(1,3) * 90));
    }
}
