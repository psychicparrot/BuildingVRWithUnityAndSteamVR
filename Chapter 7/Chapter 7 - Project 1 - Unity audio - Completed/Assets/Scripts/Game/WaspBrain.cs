using UnityEngine;
using System.Collections;

public class WaspBrain : MonoBehaviour
{
	public enum BrainStates { init, rising, attacking, retreating };

	public BrainStates currentState;
	public BrainStates targetState;

	[Space]
	public bool gameFinished;

	// explode particle effect
	[Space]
	public Transform _explodeEffect;

	[Space]
	public AudioSource _sprayedSound;

	// we will pick a random speed between min and max values here
	[Space]
	public float minMoveSpeed = 0.5f;
	public float maxMoveSpeed = 1f;

	[Space]
	public Transform _attackTarget;

	// the moveSpeed var is used for the movement
	private float moveSpeed = 0.5f;

	private bool isSprayed;
	private bool inTheStingArea;
	private bool isFlyingAway;

	private Transform _myTransform;

	void Start()
	{
		Init();
	}

	void Init()
	{
		// choose a random move speed
		moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);

		// grab a reference to this transform so that we can move it etc.
		_myTransform = GetComponent<Transform>();

		// set initial bug brain state
		targetState = BrainStates.rising;
	}

	void UpdateCurrentState()
	{
		switch (currentState)
		{
			case BrainStates.init:
				Init();
				break;

			// -----------------------------

			case BrainStates.rising:
				// set movespeed high
				moveSpeed = 5f;

				// move up
				_myTransform.Translate(0, moveSpeed * Time.deltaTime, 0);

				// check height
				if (_myTransform.position.y >= 8)
				{
					// if we're high enough, move into attack mode
					targetState = BrainStates.attacking;
				}
				break;

			// -----------------------------

			case BrainStates.attacking:
				// look at the target (the player)
				_myTransform.LookAt(_attackTarget, _myTransform.up);

				// move forward along the Z towards the player
				_myTransform.Translate(0, 0, moveSpeed * Time.deltaTime);
				break;

			// -----------------------------

			case BrainStates.retreating:

				_myTransform.Translate((Vector3.up * 10) * Time.deltaTime);
				break;

				// -----------------------------
		}

		if(currentState!=BrainStates.retreating)
		{
			if (GameController._instance.currentState == GameController.GameState.GameOver)
				targetState = BrainStates.retreating;
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

			case BrainStates.retreating:
				// destroy this wasp in 4 seconds
				Invoke("DestroyMe", 4);
				break;
		}

		// now update the current state to the one we just processed..
		currentState = newTarget;
	}

	void Update()
	{
		if (currentState != targetState)
		{
			UpdateTargetState();
		}

		UpdateCurrentState();
	}

	void OnCollisionEnter(Collision collision)
	{
		if ((currentState == BrainStates.retreating) || isSprayed || gameFinished)
			return;

		if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			// increase the house invader count on SceneController
			GameController._instance.Stung();

			// fly home!
			targetState = BrainStates.retreating;
		}
		
		if(collision.gameObject.layer == LayerMask.NameToLayer("Spray"))
		{
			// set a flag to make sure we don't repeat these calls on multiple collisions
			isSprayed = true;

			// increase score
			GameController._instance.SprayedBug();

			// play sprayed sound
			_sprayedSound.Play();

			// fly home!
			targetState = BrainStates.retreating;
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
