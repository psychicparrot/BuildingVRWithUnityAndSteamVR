using UnityEngine;
using System.Collections;

public class VRCam : MonoBehaviour {

    private Vector3 directionVec;
    private Vector3 targetPosition;
    private Vector3 currentPosition;
    private Vector3 targetVelocity;
    private Vector3 currentVelocity;

    public float targetSpeed = 2;
    public float maximumVelocity = 3;
    public float acceleration = 1;

    public Transform followTarget;

    private Transform myTransform;
    private Rigidbody myRB;

    void Start()
    {
        myTransform = GetComponent<Transform>();
        myRB = GetComponent<Rigidbody>();
    }

	void Update ()
    {
        currentPosition = myTransform.position;
        targetPosition = followTarget.position;
        
        // grab direction vector
        directionVec = targetPosition - currentPosition;
        targetVelocity = directionVec * targetSpeed;

        // clamp velocity
        targetVelocity = Vector3.ClampMagnitude(targetVelocity, maximumVelocity);
		currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.deltaTime);

        myRB.velocity = currentVelocity;
    }

    public void SetTarget(Transform aTransform)
    {
        followTarget = aTransform;
    }
}
