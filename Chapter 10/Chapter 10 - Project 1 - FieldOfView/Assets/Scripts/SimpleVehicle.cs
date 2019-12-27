using UnityEngine;
using System.Collections;

public class SimpleVehicle : MonoBehaviour
{

	public float moveSpeed = 6;
	public float turnSpeed = 0.5f;

	public Vector3 turnVector = new Vector3(0, 1, 0);

	private float currentMoveSpeed;
	private float currentTurnSpeed;

	private Transform myTransform;
	private Rigidbody myRB;

	public Transform joyStick;
	private Vector3 joyStickRotation;

	private AudioSource engineNoiseAudioSource;

	void Start()
	{
		myTransform = GetComponent<Transform>();
		myRB = GetComponent<Rigidbody>();
		engineNoiseAudioSource = GetComponent<AudioSource>();
	}

	void FixedUpdate()
	{
		if (Input.GetAxis("Vertical") > 0.4f)
		{
			currentMoveSpeed = moveSpeed;
		}
		else if (Input.GetAxis("Vertical") < -0.4f)
		{
			currentMoveSpeed = -moveSpeed;
		}
		else
		{
			currentMoveSpeed = 0;
		}

		if (Input.GetAxis("Horizontal") > 0.4f)
		{
			currentTurnSpeed = turnSpeed;
		}
		else if (Input.GetAxis("Horizontal") < -0.4f)
		{
			currentTurnSpeed = -turnSpeed;
		}
		else
		{
			currentTurnSpeed = 0;
		}

		myRB.velocity = myTransform.forward * currentMoveSpeed;
		myRB.angularVelocity = turnVector * currentTurnSpeed;

		joyStickRotation.x = Mathf.Lerp(joyStickRotation.x, currentMoveSpeed, 0.2f);
		joyStickRotation.y = Mathf.Lerp(joyStickRotation.y, -currentTurnSpeed * 10, 0.2f);
		joyStickRotation.z = Mathf.Lerp(joyStickRotation.z, -currentTurnSpeed * 10, 0.2f);

		joyStick.localEulerAngles = joyStickRotation;

		engineNoiseAudioSource.pitch = Mathf.Abs(currentMoveSpeed * 0.25f) + Mathf.Abs(currentTurnSpeed * 0.5f);
	}
}
