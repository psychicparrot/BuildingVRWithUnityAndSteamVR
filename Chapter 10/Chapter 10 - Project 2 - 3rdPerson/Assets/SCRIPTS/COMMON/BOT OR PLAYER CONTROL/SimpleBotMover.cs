using UnityEngine;
using System.Collections;

// use this script in conjunction with the BaseAIController to make a bot move around

public class SimpleBotMover : MonoBehaviour
{
	public BaseAIController AIController;
	public Rigidbody myBody;
	public float turnSpeed= 0.5f;
	public float moveSpeed= 0.5f;
	
	public Vector3 centerOfGravity;
	
	private Transform myTransform;
	
	void Start ()
	{
		// cache a ref to our transform
		myTransform= transform;
		
		// if it hasn't been set in the editor, let's try and find it on this transform
		if(AIController==null)
			AIController= myTransform.GetComponent<BaseAIController>();
		
		// set center of gravity
		if(myBody!=null)
		{
			myBody.centerOfMass= centerOfGravity;
		}
	}
	
	void Update () 
	{
		// turn the transform, if required
		myTransform.Rotate( new Vector3( 0, Time.deltaTime * AIController.horz * 0.1f , 0 ) );
		
		// if we have a rigidbody, move it if required
		if(myBody!=null)
		{
			myBody.AddForce( ( myTransform.forward * moveSpeed * Time.deltaTime ) * AIController.vert, ForceMode.VelocityChange);
		}
		
		
	}
}
