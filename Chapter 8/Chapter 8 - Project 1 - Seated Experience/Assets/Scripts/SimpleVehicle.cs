using UnityEngine;
using System.Collections;

public class SimpleVehicle : MonoBehaviour {

    public float moveSpeed= 6;
    public float turnSpeed= 0.5f;

    public Vector3 turnVector = new Vector3(0,1,0);

    private float currentMoveSpeed;
    private float currentTurnSpeed;

    private Transform myTransform;
    private Rigidbody myRB;

    public Transform joyStick;
    private Vector3 joyStickRotation;

    private AudioSource engineNoiseAudioSource;

    void Start ()
    {
        myTransform = GetComponent<Transform>();
        myRB = GetComponent<Rigidbody>();
        engineNoiseAudioSource = GetComponent<AudioSource>();
    }
	
	void Update ()
    {
        currentMoveSpeed = moveSpeed * Input.GetAxis("Vertical");
        currentTurnSpeed = turnSpeed * Input.GetAxis("Horizontal");

        myRB.velocity = myTransform.forward * currentMoveSpeed;
        myRB.angularVelocity = turnVector * currentTurnSpeed;

        joyStickRotation.x = currentMoveSpeed;
        joyStickRotation.y = -currentTurnSpeed * 10;
        joyStickRotation.z = -currentTurnSpeed * 10;
        joyStick.localEulerAngles = joyStickRotation;

        engineNoiseAudioSource.pitch = Mathf.Abs(currentMoveSpeed * 0.25f) + Mathf.Abs (currentTurnSpeed * 0.5f);
    }
}
