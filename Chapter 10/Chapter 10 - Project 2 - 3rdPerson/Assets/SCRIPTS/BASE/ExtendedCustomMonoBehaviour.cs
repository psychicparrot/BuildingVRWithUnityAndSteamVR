using UnityEngine;
using System.Collections;

public class ExtendedCustomMonoBehaviour : MonoBehaviour 
{
	// This class is used to add some common variables to MonoBehaviour, rather than
	// constantly repeating the same declarations in every class.
	
	public Transform myTransform;
	public GameObject myGO;
	public Rigidbody myBody;
	
	public bool didInit;
	public bool canControl;
	
	public int id;
	
	[System.NonSerialized]
	public Vector3 tempVEC;
	
	[System.NonSerialized]
	public Transform tempTR;
	
	public virtual void SetID( int anID )
	{
		id= anID;
	}
}
