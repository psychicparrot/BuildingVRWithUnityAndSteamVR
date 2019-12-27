using UnityEngine;
using System.Collections;

[AddComponentMenu("Base/Weapon Controller")]

public class BaseWeaponController : MonoBehaviour
{
	public GameObject[] weapons;
	
	public int selectedWeaponSlot;
	public int lastSelectedWeaponSlot;
	
	public Vector3 offsetWeaponSpawnPosition;
	
	public Transform forceParent;

	private ArrayList weaponSlots;
	private ArrayList weaponScripts;
	private BaseWeaponScript TEMPWeapon;
	private Vector3 TEMPvector3;
	private Quaternion TEMProtation;
	private GameObject TEMPgameObject;
	
	private Transform myTransform;
	private int ownerNum;

	public bool useForceVectorDirection;
	public Vector3 forceVector;
	private Vector3 theDir;

	public void Start () 
	{
		// default to the first weapon slot
		selectedWeaponSlot= 0;
		lastSelectedWeaponSlot= -1;
		
		// initialize weapon list ArrayList
		weaponSlots= new ArrayList();
		
		// initialize weapon scripts ArrayList
		weaponScripts= new ArrayList();
		
		// cache a reference to the transform (looking up a transform each step can be expensive, so this is important!)
		myTransform= transform;
		
		if(forceParent==null)
		{
			forceParent= myTransform;
		}
		// rather than look up the transform position and rotation of the player each iteration of the loop below,
		// we cache them first into temporary variables
		TEMPvector3= forceParent.position;
		TEMProtation= forceParent.rotation;
		
		// we instantiate all of the weapons and hide them so that we can activate and use them
		// when needed.
		for( int i=0; i<weapons.Length; i++ )
		{
			// Instantiate the item from the weapons list
			TEMPgameObject= (GameObject) Instantiate( weapons[i], TEMPvector3 + offsetWeaponSpawnPosition, TEMProtation );
			
			// make this gameObject that our weapon controller script is attached to, to be the parent of the weapon
			// so that the weapon will move around with the player
			//
			// NOTE: if you need projectiles to be on a different layer from the main gameObject, set the layer of the
			// forceParent object to the layer you want projectiles to be on
			
			TEMPgameObject.transform.parent= forceParent;
			TEMPgameObject.layer= forceParent.gameObject.layer;
			TEMPgameObject.transform.position= forceParent.position;
			TEMPgameObject.transform.rotation= forceParent.rotation;
			
			// store a reference to the gameObject in an ArrayList
			weaponSlots.Add( TEMPgameObject );
			
			// grab a reference to the weapon script attached to the weapon and store the reference in an ArrayList
			TEMPWeapon= TEMPgameObject.GetComponent<BaseWeaponScript>();
			weaponScripts.Add( TEMPWeapon );

			// disable the weapon
			TEMPgameObject.SetActive( false );
		}
		
		// now we set the default selected weapon to visible
		SetWeaponSlot(0);
	}
	
	public void SetOwner(int aNum)
	{
		// used to identify the object firing, if required
		ownerNum= aNum;	
	}
	
	public virtual void SetWeaponSlot (int slotNum)
	{
		// if the selected weapon is already this one, drop out!
		if(slotNum==lastSelectedWeaponSlot)
			return;
		
		// disable the current weapon
		DisableCurrentWeapon();
		
		// set our current weapon to the one passed in
		selectedWeaponSlot= slotNum;
		
		// make sure sensible values are getting passed in
		if(selectedWeaponSlot<0)
			selectedWeaponSlot= weaponSlots.Count-1;
		
		// make sure that the weapon slot isn't higher than the total number of weapons in our list
		if(selectedWeaponSlot>weaponSlots.Count-1)
			selectedWeaponSlot=weaponSlots.Count-1;
		
		// we store this selected slot to use to prevent duplicate weapon slot setting
		lastSelectedWeaponSlot= selectedWeaponSlot;
		
		// enable the newly selected weapon
		EnableCurrentWeapon();
	}
	
	public virtual void NextWeaponSlot (bool shouldLoop)
	{
		// disable the current weapon
		DisableCurrentWeapon();
		
		// next slot
		selectedWeaponSlot++;
		
		// make sure that the slot isn't higher than the total number of weapons in our list
		if(selectedWeaponSlot==weaponScripts.Count)
		{
			if(shouldLoop)
			{
				selectedWeaponSlot= 0;
			} else {
				selectedWeaponSlot= weaponScripts.Count-1;
			}
		}
		
		// we store this selected slot to use to prevent duplicate weapon slot setting
		lastSelectedWeaponSlot=selectedWeaponSlot;
		
		// enable the newly selected weapon
		EnableCurrentWeapon();
	}
	
	public virtual void PrevWeaponSlot (bool shouldLoop)
	{
		// disable the current weapon
		DisableCurrentWeapon();
		
		// prev slot
		selectedWeaponSlot--;
		
		// make sure that the slot is a sensible number
		if( selectedWeaponSlot<0 )
		{
			if(shouldLoop)
			{
				selectedWeaponSlot= weaponScripts.Count-1;
			} else {
				selectedWeaponSlot= 0;
			}
		}
		
		// we store this selected slot to use to prevent duplicate weapon slot setting
		lastSelectedWeaponSlot=selectedWeaponSlot;
		
		// enable the newly selected weapon
		EnableCurrentWeapon();
	}
	
	
	public virtual void DisableCurrentWeapon ()
	{
		if(weaponScripts.Count==0)
			return;
		
		// grab reference to currently selected weapon script
		TEMPWeapon= ( BaseWeaponScript )weaponScripts[selectedWeaponSlot];
		
		// now tell the script to disable itself
		TEMPWeapon.Disable();
		
		// grab reference to the weapon's gameObject and disable that, too
		TEMPgameObject= ( GameObject )weaponSlots[selectedWeaponSlot];
		TEMPgameObject.SetActive( false );
	}
	
	public virtual void EnableCurrentWeapon ()
	{
		if( weaponScripts.Count==0 )
			return;
		
		// grab reference to currently selected weapon
		TEMPWeapon= ( BaseWeaponScript )weaponScripts[selectedWeaponSlot];
		
		// now tell the script to enable itself
		TEMPWeapon.Enable();
		
		TEMPgameObject= ( GameObject )weaponSlots[selectedWeaponSlot];
		TEMPgameObject.SetActive( true );
	}
		
	public virtual void Fire ()
	{
		if(weaponScripts==null)
			return;
		
		if(weaponScripts.Count==0)
			return;
		
		// find the weapon in the currently selected slot
		TEMPWeapon= ( BaseWeaponScript )weaponScripts[selectedWeaponSlot];
		
		theDir = transform.forward;
		
		if( useForceVectorDirection )
			theDir = forceVector;
		
		// fire the projectile
		TEMPWeapon.Fire( theDir, ownerNum );
	}
	
}
