using UnityEngine;
using System.Collections;

[AddComponentMenu("Utility/Fake Friction")]

public class PretendFriction : MonoBehaviour 
{
	private Rigidbody myBody;
	private Transform myTransform;
	private float myMass;
	private float slideSpeed;
	private Vector3 velo;
	private Vector3 flatVelo;
	private Vector3 myRight;
	private Vector3 TEMPvec3;
	
	public float theGrip=100f;
	
	void Start () 
	{
		// cache some references to our rigidbody, mass and transform
		myBody=GetComponent<Rigidbody>();
		myMass=myBody.mass;
		myTransform=transform;
	}
	
	void FixedUpdate () 
	{
		// grab the values we need to calculate grip
		myRight=myTransform.right;
		
		// calculate flat velocity
		velo=myBody.velocity;
		flatVelo.x=velo.x;
		flatVelo.y=0;
		flatVelo.z=velo.z;
		
		// calculate how much we are sliding
		slideSpeed=Vector3.Dot(myRight,flatVelo);
		
		// build a new vector to compensate for the sliding
		TEMPvec3= myRight * (-slideSpeed * myMass * theGrip);
		
		// apply the correctional force to the rigidbody
		myBody.AddForce(TEMPvec3 * Time.deltaTime);
	}
}
