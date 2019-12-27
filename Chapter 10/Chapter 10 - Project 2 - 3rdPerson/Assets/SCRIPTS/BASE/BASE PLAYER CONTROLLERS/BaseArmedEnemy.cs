using UnityEngine;
using System.Collections;
using AIAttackStates;

[AddComponentMenu("Base/AI Weapon Controller")]

public class BaseArmedEnemy : ExtendedCustomMonoBehaviour
{
	[System.NonSerialized]
	public bool doFire;
	
	public bool onlyFireWhenOnscreen;
	
	public int pointsValue=50;
	public int thisEnemyStrength= 1;
	public bool thisGameObjectShouldFire;
	
	// we use a renderer to test whether or not the ship is on screen
	public Renderer rendererToTestAgainst;
	
	
	public Standard_SlotWeaponController weaponControl;
	public GameObject mesh_parentGO;
	
	private bool canFire;
	
	public float fireDelayTime =1f;
	
	public BasePlayerManager myPlayerManager;
	public BaseUserManager myDataManager;
	
	public bool isBoss= false;
	
	public int tempINT;
	
	// default action is to attack nothing
	public AIAttackState currentState= AIAttackState.random_fire;
	
	public string tagOfTargetsToShootAt;
	
	public void Start ()
	{
		// now call our script-specific init function
		Init ();
	}
	
	public void Init()
	{
		// cache our transform
		myTransform= transform;
		
		// cache our gameObject
		myGO= gameObject;
		
		if(weaponControl==null)
		{
			// try to find weapon controller on this gameobject
			weaponControl= myGO.GetComponent<Standard_SlotWeaponController>();
		}
		
		if(rendererToTestAgainst==null)
		{
			// we need a renderer to find out whether or not we are on-screen, so let's try and find one
			// in our children if we don't already have one set in the editor
			rendererToTestAgainst=myGO.GetComponentInChildren<Renderer>();
		}
				
		// if a player manager is not set in the editor, let's try to find one
		if(myPlayerManager==null)
		{
			myPlayerManager= myGO.AddComponent<BasePlayerManager>();
		}
		
		myDataManager= myPlayerManager.DataManager;
		myDataManager.SetName("Enemy");
		myDataManager.SetHealth(thisEnemyStrength);
				
		canFire=true;
		didInit=true;
	}
	
	private RaycastHit rayHit;
	
	public virtual void Update ()
	{
		// if we are not allowed to control the weapon, we drop out here
		if(!canControl)
			return;
		
		if(thisGameObjectShouldFire)
		{
			// we use doFire to determine whether or not to fire right now
			doFire=false;
			
			// canFire is used to control a delay between firing
			if( canFire )
			{
				if( currentState==AIAttackState.random_fire )
				{
					// if the random number is over x, fire
					if( Random.Range(0,100)>98 )
					{
						doFire=true;
					}
				} else if( currentState==AIAttackState.look_and_destroy )
				{
					if(Physics.Raycast( myTransform.position, myTransform.forward, out rayHit ))
					{
						// is it an opponent to be shot at?
						if( rayHit.transform.CompareTag( tagOfTargetsToShootAt ) )
						{
							//	we have a match on the tag, so let's shoot at it
							doFire=true;
						}
					}
		
				} else {
					// if we're not set to random fire or look and destroy, just fire whenever we can
					doFire=true;	
				}
			}
				
			if( doFire )
			{
				// we only want to fire if we are on-screen, visible on the main camera
				if(onlyFireWhenOnscreen && !rendererToTestAgainst.IsVisibleFrom( Camera.main ))
				{
					doFire=false;
					return;
				}
				
				// tell weapon control to fire, if we have a weapon controller
				if(weaponControl!=null)
				{
					// tell weapon to fire
					weaponControl.Fire();
				}
				// set a flag to disable firing temporarily (providing a delay between firing)
				canFire= false;
				// invoke a function call in <fireDelayTime> to reset canFire back to true, allowing another firing session
				Invoke ( "ResetFire", fireDelayTime );
			}		
		}
	}
	
	public void ResetFire ()
	{
		canFire=true;	
	}
	
}
