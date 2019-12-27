using UnityEngine;
using System.Collections;

[AddComponentMenu("Sample Game Glue Code/Laser Blast Survival/Player Controller")]

public class Player_LBS : BaseTopDown
{
	private bool isInvulnerable;
	private bool isRespawning;
	
	public BasePlayerManager myPlayerManager;
	public BaseUserManager myDataManager;
	
	
	public bool godMode =false;
	public GameObject theMeshGO;
	
	public Standard_SlotWeaponController weaponControl;
	public bool canFire;
	
	public bool isFinished;
	
	public override void Init ()
	{
		base.Init();
		
		// do god mode, if needed)
		if(!godMode)
		{
			MakeVulnerable();
		} else {
			MakeInvulnerable();
		}
		
		// start out with no control from the player
		canControl=false;
		
		// get a ref to the weapon controller
		weaponControl= myGO.GetComponent<Standard_SlotWeaponController>();
		
		// if a player manager is not set in the editor, let's try to find one
		if(myPlayerManager==null)
			myPlayerManager= myGO.GetComponent<BasePlayerManager>();
		
		// set up the data for our player
		myDataManager= myPlayerManager.DataManager;
		myDataManager.SetName("Player");
		myDataManager.SetHealth(3);
		
		isFinished= false;
		
		// get a ref to the player manager
		GameController_LBS.Instance.UpdateLivesP1(myDataManager.GetHealth());
	}
	
	public override void GetInput()
	{
		if(isFinished || isRespawning)
		{
			horz=0;
			vert=0;
			return;
		}
		
		// drop out if we're not supposed to be controlling this player
		if(!canControl)
			return;
		
		// grab inputs from the default input provider
		horz= Mathf.Clamp( default_input.GetHorizontal() , -1, 1 );
	    vert= Mathf.Clamp( default_input.GetVertical() , -1, 1 );
		
		// fire if we need to
		if( default_input.GetFire() && canFire )
		{
			// tell weapon controller to deal with firing
			weaponControl.Fire();
		}
	}
	
	public void GameStart()
	{
		// this function is called by the game controller to tell us when we can start moving
		canControl=true;
	}
	
	void LostLife()
	{
		isRespawning=true;
				
		// blow us up!
		GameController_LBS.Instance.PlayerHit( myTransform );
			
		// reduce lives by one
		myDataManager.ReduceHealth(1);
		
		// as our ID is 1, we must be player 1
		GameController_LBS.Instance.UpdateLivesP1( myDataManager.GetHealth() );
		
		if(myDataManager.GetHealth()<1) // <- game over
		{
            // stop movement, as long as rigidbody is not kinematic (otherwise it will have no velocity and we
            // will generate an error message trying to set it)
            if (myPlayerController.GetComponent<Rigidbody>() != null)
            {
                if (!myPlayerController.GetComponent<Rigidbody>().isKinematic)
                    myPlayerController.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }

			// hide ship body
			theMeshGO.SetActive(false);
			
			// disable and hide weapon
			weaponControl.DisableCurrentWeapon();
			
			// do anything we need to do at game finished
			PlayerFinished();
		} else {
			// hide ship body
			theMeshGO.SetActive(false);
			
			// disable and hide weapon
			weaponControl.DisableCurrentWeapon();
					
			// respawn 
			Invoke("Respawn",2f);
		}
	}
	
	void Respawn()
	{
		// reset the 'we are respawning' variable
		isRespawning= false;
		
		// we need to be invulnerable for a little while
		MakeInvulnerable();
		
		Invoke ("MakeVulnerable",3);
		// show ship body again
		theMeshGO.SetActive(true);
		
		// revert to the first weapon
		weaponControl.SetWeaponSlot(0);
		
		// show the current weapon (since it was hidden when the ship explosion was shown)
		weaponControl.EnableCurrentWeapon();
	}
	
	void OnCollisionEnter(Collision collider)
	{
		// MAKE SURE that weapons don't have colliders
		// if you are using primitives, only use a single collider on the same gameobject which has this script on
		
		// when something collides with our ship, we check its layer to see if it is on 11 which is our projectiles
		// (Note: remember when you add projectiles to set the layer correctly!)
		if(collider.gameObject.layer==11 && !isRespawning && !isInvulnerable)
		{
			LostLife();
		}
	}
	
	void MakeInvulnerable()
	{
		isInvulnerable=true;
		//shieldMesh.SetActive(true);
	}
	
	void MakeVulnerable()
	{
		isInvulnerable=false;
		//shieldMesh.SetActive(false);
	}
	
	public void PlayerFinished()
	{
		// tell the player controller that we have finished
		GameController_LBS.Instance.PlayerDied( id );
		
		isFinished=true;
	}
	
}
