using UnityEngine;
using System.Collections;

[AddComponentMenu("Base/Input Controller")]

public class BaseInputController : MonoBehaviour {
	
	// directional buttons
	public bool Up;
	public bool Down;
	public bool Left;
	public bool Right;
	
	// fire / action buttons
	public bool Fire1;
	
	// weapon slots
	public bool Slot1;
	public bool Slot2;
	public bool Slot3;
	public bool Slot4;
	public bool Slot5;
	public bool Slot6;
	public bool Slot7;
	public bool Slot8;
	public bool Slot9;
	
	public float vert;
	public float horz;
	public bool shouldRespawn;
	
	public Vector3 TEMPVec3;
	private Vector3 zeroVector = new Vector3(0,0,0);
	
	public virtual void CheckInput ()
	{	
		// override with your own code to deal with input
		horz=Input.GetAxis ("Horizontal");
		vert=Input.GetAxis ("Vertical");
	}
	
	public virtual float GetHorizontal()
	{
		// returns our cached horizontal input axis value
		return horz;
	}
	
	public virtual float GetVertical()
	{
		// returns our cached vertical input axis value
		return vert;
	}
	
	public virtual bool GetFire()
	{
		return Fire1;
	}
	
	public bool GetRespawn()
	{
		return shouldRespawn;	
	}
	
	public virtual Vector3 GetMovementDirectionVector()
	{
		// temp vector for movement dir gets set to the value of an otherwise unused vector that always have the value of 0,0,0
		TEMPVec3=zeroVector;
		
		TEMPVec3.x=horz;
		TEMPVec3.y=vert;

		// return the movement vector
		return TEMPVec3;
	}

}
