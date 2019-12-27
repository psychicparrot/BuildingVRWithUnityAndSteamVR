using UnityEngine;
using System.Collections;

[AddComponentMenu("Base/Character/Third Person")]

public class BaseTopDown : ExtendedCustomMonoBehaviour 
{
	public AnimationClip idleAnimation;
	public AnimationClip walkAnimation;

	public float walkMaxAnimationSpeed = 0.75f;
	public float runMaxAnimationSpeed = 1.0f;

	// When did the user start walking (Used for going into run after a while)
	private float walkTimeStart= 0.0f;
	
	// we've made the following variable public so that we can use an animation on a different gameObject if needed
	public Animation _animation;
	
	enum CharacterState {
		Idle = 0,
		Walking = 1,
		Running = 2,
	}
	
	private CharacterState _characterState;

	// The speed when walking
	public float walkSpeed= 2.0f;
	
	// after runAfterSeconds of walking we run with runSpeed
	public float runSpeed= 4.0f;

	public float speedSmoothing= 10.0f;
	public float rotateSpeed= 500.0f;
	public float runAfterSeconds= 3.0f;
	
	// The current move direction in x-z
	private Vector3 moveDirection= Vector3.zero;
	
	// The current vertical speed
	private float verticalSpeed= 0.0f;
	
	// The current x-z move speed
	public float moveSpeed= 0.0f;
	
	// The last collision flags returned from controller.Move
	private CollisionFlags collisionFlags;
			
	public BasePlayerManager myPlayerController; 

	[System.NonSerialized]
	public Keyboard_Input default_input;
	
	public float horz;
	public float vert;
	
	private CharacterController controller;
	
	// -------------------------------------------------------------------------
			
	void  Awake ()
	{
		// we need to do this before anything happens to the script or object, so it happens in Awake.
		// if you need to add specific set up, consider adding it to the Init() function instead to keep this
		// function limited only to things we need to do before anything else happens.
		
		moveDirection = transform.TransformDirection(Vector3.forward);
		
		// if _animation has not been set up in the inspector, we'll try to find it on the current gameobject
		if(_animation==null)
			_animation = GetComponent<Animation>();
		
		if(!_animation)
			Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");
		
		if(!idleAnimation) {
			_animation = null;
			Debug.Log("No idle animation found. Turning off animations.");
		}
		if(!walkAnimation) {
			_animation = null;
			Debug.Log("No walk animation found. Turning off animations.");
		}

		controller = GetComponent<CharacterController>();
	}
	
	public virtual void Start ()
	{
		Init ();	
	}
	
	public virtual void Init ()
	{
		// cache the usual suspects
		myBody= GetComponent<Rigidbody>();
		myGO= gameObject;
		myTransform= transform;
		
		// add default keyboard input
		default_input= myGO.AddComponent<Keyboard_Input>();
		
		// cache a reference to the player controller
		myPlayerController= myGO.GetComponent<BasePlayerManager>();
		
		if(myPlayerController!=null)
			myPlayerController.Init();
	}
	
	public void SetUserInput( bool setInput )
	{
		canControl= setInput;	
	}
	
	public virtual void GetInput()
	{
		horz= Mathf.Clamp( default_input.GetHorizontal() , -1, 1 );
	    vert= Mathf.Clamp( default_input.GetVertical() , -1, 1 );
	}
	
	public virtual void LateUpdate()
	{
		// we check for input in LateUpdate because Unity recommends this
		if(canControl)
			GetInput();
	}
	
	public bool moveDirectionally;
	
	private Vector3 targetDirection;
	private float curSmooth;
	private float targetSpeed;
	private float curSpeed;
	private Vector3 forward;
	private Vector3 right;

	void  UpdateSmoothedMovementDirection ()
	{			
		if(moveDirectionally)
		{
			UpdateDirectionalMovement();
		} else {
			UpdateRotationMovement();
		}
	}
	
	void UpdateDirectionalMovement()
	{
		// find target direction
		targetDirection= horz * Vector3.right;
		targetDirection+= vert * Vector3.forward;
		
		// We store speed and direction seperately,
		// so that when the character stands still we still have a valid forward direction
		// moveDirection is always normalized, and we only update  it if there is user input.
		if (targetDirection != Vector3.zero)
		{
				moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
				moveDirection = moveDirection.normalized;
		}
		
		// Smooth the speed based on the current target direction
		curSmooth= speedSmoothing * Time.deltaTime;
		
		// Choose target speed
		//* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
		targetSpeed= Mathf.Min(targetDirection.magnitude, 1.0f);
	
		_characterState = CharacterState.Idle;
		
		// decide on animation state and adjust move speed
		if (Time.time - runAfterSeconds > walkTimeStart)
		{
			targetSpeed *= runSpeed;
			_characterState = CharacterState.Running;
		}
		else
		{
			targetSpeed *= walkSpeed;
			_characterState = CharacterState.Walking;
		}
		
		moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);
		
		// Reset walk time start when we slow down
		if (moveSpeed < walkSpeed * 0.3f)
			walkTimeStart = Time.time;
			
		// Calculate actual motion
		Vector3 movement= moveDirection * moveSpeed;
		movement *= Time.deltaTime;
		
		// Move the controller
		collisionFlags = controller.Move(movement);
		
		// Set rotation to the move direction
		myTransform.rotation = Quaternion.LookRotation(moveDirection);
	}
	
	void UpdateRotationMovement ()
	{
		// this character movement is based on the code in the Unity help file for CharacterController.SimpleMove
		// http://docs.unity3d.com/Documentation/ScriptReference/CharacterController.SimpleMove.html
		
        myTransform.Rotate(0, horz * rotateSpeed * Time.deltaTime, 0);
        curSpeed = moveSpeed * vert;
		controller.SimpleMove( myTransform.forward * curSpeed );

		// Target direction (the max we want to move, used for calculating target speed)
		targetDirection= vert * myTransform.forward;
				
		// Smooth the speed based on the current target direction
		float curSmooth= speedSmoothing * Time.deltaTime;
		
		// Choose target speed
		//* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
		targetSpeed= Mathf.Min(targetDirection.magnitude, 1.0f);
	
		_characterState = CharacterState.Idle;
		
		// decide on animation state and adjust move speed
		if (Time.time - runAfterSeconds > walkTimeStart)
		{
			targetSpeed *= runSpeed;
			_characterState = CharacterState.Running;
		}
		else
		{
			targetSpeed *= walkSpeed;
			_characterState = CharacterState.Walking;
		}
		
		moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);
		
		// Reset walk time start when we slow down
		if (moveSpeed < walkSpeed * 0.3f)
			walkTimeStart = Time.time;
		
	}
	
	void Update ()
	{	
		if (!canControl)
		{
			// kill all inputs if not controllable.
			Input.ResetInputAxes();
		}
		
		UpdateSmoothedMovementDirection();
		
		// ANIMATION sector
		if(_animation) {
			if(controller.velocity.sqrMagnitude < 0.1f) {
				_animation.CrossFade(idleAnimation.name);
			}
			else 
			{
				if(_characterState == CharacterState.Running) {
					_animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, runMaxAnimationSpeed);
					_animation.CrossFade(walkAnimation.name);	
				}
				else if(_characterState == CharacterState.Walking) {
					_animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, walkMaxAnimationSpeed);
					_animation.CrossFade(walkAnimation.name);	
				}
			}
		}	

        // apply gravity
        controller.Move(Vector3.down);
    }

	public float GetSpeed ()
	{
		return moveSpeed;
	}
		
	public Vector3 GetDirection ()
	{
		return moveDirection;
	}
	
	public bool IsMoving ()
	{
		 return Mathf.Abs(vert) + Mathf.Abs(horz) > 0.5f;
	}
}