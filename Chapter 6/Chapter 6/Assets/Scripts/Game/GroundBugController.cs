using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBugController : MonoBehaviour
{
	public enum BrainStates { init, crawling, attacking, dead };

	public BrainStates currentState;
	public BrainStates targetState;

	[Space]
	public bool gameFinished;

	// the moveSpeed var is used for the movement
	public float moveSpeed = 0.5f;
	private float attackSpeed = 2.5f;

	[Space]
	public AudioSource _sprayedSound;
	public Transform _attackTarget;

	private bool isSprayed;
	private Transform _myTransform;
	private Vector3 upVec;
	private Rigidbody _RB;

	void Start()
	{ 
		Init();
	}

	void Init()
	{
		// grab a reference to this transform so that we can move it etc.
		_myTransform = GetComponent<Transform>();

		_RB = GetComponent<Rigidbody>();

		// set initial bug brain state
		targetState = BrainStates.crawling;

		upVec = transform.up;
	}

	void Update()
	{
		if (currentState != targetState)
		{
			UpdateTargetState();
		}

		UpdateCurrentState();
	}

	void UpdateCurrentState()
	{
		switch (currentState)
		{
			case BrainStates.init:
				break;

			// -----------------------------

			case BrainStates.attacking:
				// look at the target (the player)
				_myTransform.LookAt(_attackTarget, _myTransform.up);

				// move forward along the Z towards the player
				_myTransform.Translate(0, 0, attackSpeed * Time.deltaTime);
				break;

			case BrainStates.crawling:
				// move forward along the Z towards the player
				_myTransform.Translate(0, 0, moveSpeed * Time.deltaTime);
				break;

			case BrainStates.dead:
				_RB.velocity = new Vector3(0, 10, -10);
				break;
		}
	}

	void UpdateTargetState()
	{
		// store the current target state for use at the end of this function
		BrainStates newTarget = targetState;
		// process the current target state
		switch (targetState)
		{
			case BrainStates.init:
				Init();
				break;

			case BrainStates.attacking:
				// release the bug!
				_RB.useGravity = false;
				_RB.constraints = RigidbodyConstraints.None;
				_RB.velocity = Vector3.zero;
				_RB.angularVelocity = Vector3.zero;

				// look at the target (the player)
				_myTransform.LookAt(_attackTarget, Vector3.up);
				break;

			case BrainStates.dead:
				_RB.constraints = RigidbodyConstraints.None;
				_RB.useGravity = false;
				_RB.velocity = Vector3.up*10f;
				break;
		}

		// now update the current state to the one we just processed..
		currentState = newTarget;
	}

	void OnCollisionEnter(Collision collision)
	{
		if ((currentState == BrainStates.dead) || isSprayed || gameFinished)
			return;

		if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			// increase the house invader count on SceneController
			GameController._instance.Stung();

			// dead!
			targetState = BrainStates.dead;
		}

		if (collision.gameObject.layer == LayerMask.NameToLayer("Spray"))
		{
			// set a flag to make sure we don't repeat these calls on multiple collisions
			isSprayed = true;

			// increase score
			GameController._instance.SprayedBug();

			// play sprayed sound
			_sprayedSound.Play();

			// dead!
			targetState = BrainStates.dead;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (gameFinished)
			return;

		if(other.gameObject.layer==LayerMask.NameToLayer("JumpZone"))
		{
			targetState = BrainStates.attacking;
		}
	}

	void DestroyMe()
	{
		// destroy this gameObject
		Destroy(this.gameObject);
	}

	public void SetTarget(Transform aTarget)
	{
		_attackTarget = aTarget;
	}
}
