// this script simply fires a projectile when the fire button is pressed

using UnityEngine;
using System.Collections;

[AddComponentMenu("Base/A Weapon Script")]

public class BaseWeaponScript : MonoBehaviour
{
	[System.NonSerialized]
	public bool canFire;
	
	public int ammo= 100;
	public int maxAmmo= 100;
	
	public bool isInfiniteAmmo;
	public GameObject projectileGO;
	public Collider parentCollider;
	
	private Vector3 fireVector;
	
	[System.NonSerialized]
	public Transform myTransform;
	
	private int myLayer;
	
	public Vector3 spawnPosOffset;
	public float forwardOffset=	1.5f;
	public float reloadTime=	0.2f;
	public float projectileSpeed= 10f;
	public bool inheritVelocity;
	
	[System.NonSerialized]
	public Transform theProjectile;	
	
	private GameObject theProjectileGO;
	private bool isLoaded;
	private ProjectileController theProjectileController;
	
	public virtual void Start()
	{
		Init();
	}
	
	public virtual void Init()
	{
		// cache the transform
		myTransform= transform;
		
		// cache the layer (we'll set all projectiles to avoid this layer in collisions so that things don't shoot themselves!)
		myLayer= gameObject.layer;
	
		// load the weapon
		Reloaded();
	}
	
	public virtual void Enable()
	{
		// enable weapon (do things like show the weapons mesh etc.)
		canFire=true;
	}
	
	public virtual void Disable()
	{
		// hide weapon (do things like hide the weapons mesh etc.)
		canFire=false;
	}
	
	public virtual void Reloaded()
	{
		// the 'isLoaded' var tells us if this weapon is loaded and ready to fire
		isLoaded= true;
	}

	public virtual void SetCollider( Collider aCollider )
	{
		parentCollider= aCollider;
	}
	
	public virtual void Fire( Vector3 aDirection, int ownerID )
	{
		// be sure to check canFire so that the weapon can be enabled or disabled as required!
		if( !canFire )
			return;
		
		// if the weapon is not loaded, drop out
		if( !isLoaded )
			return;
		
		// if we're out of ammo and we do not have infinite ammo, drop out..
		if( ammo<=0 && !isInfiniteAmmo )
			return;

		// decrease ammo
		ammo--;
		
		// generate the actual projectile
		FireProjectile( aDirection, ownerID );
		
		// we need to reload before we can fire again
		isLoaded= false;
		
		// schedule a completion of reloading in <reloadTime> seconds:
		CancelInvoke( "Reloaded" );
		Invoke( "Reloaded", reloadTime );
	}
		
	public virtual void FireProjectile( Vector3 fireDirection, int ownerID )
	{
		// make our first projectile
		theProjectile= MakeProjectile( ownerID );
	
		// direct the projectile toward the direction of fire
		theProjectile.LookAt( theProjectile.position + fireDirection );
	
		// add some force to move our projectile
		theProjectile.GetComponent<Rigidbody>().velocity= fireDirection * projectileSpeed;
	}
	
	public virtual Transform MakeProjectile( int ownerID )
	{		
		// create a projectile
		theProjectile= SpawnController.Instance.Spawn( projectileGO, myTransform.position+spawnPosOffset + ( myTransform.forward * forwardOffset ), myTransform.rotation );
		theProjectileGO= theProjectile.gameObject;
		theProjectileGO.layer= myLayer;
		
		// grab a ref to the projectile's controller so we can pass on some information about it
		theProjectileController= theProjectileGO.GetComponent<ProjectileController>();

		// set owner ID so we know who sent it
		theProjectileController.SetOwnerType(ownerID);
			
		Physics.IgnoreLayerCollision( myTransform.gameObject.layer, myLayer );
		
		// NOTE: Make sure that the parentCollider is a collision mesh which represents the firing object
		// or a collision mesh likely to be hit by a projectile as it is being fired from the vehicle.
		// One limitation with this system is that it only reliably supports a single collision mesh
		
		if( parentCollider!=null )
		{
			// disable collision between 'us' and our projectile so as not to hit ourselves with it!
			Physics.IgnoreCollision( theProjectile.GetComponent<Collider>(), parentCollider );
		}
		
		// return this projectile incase we want to do something else to it
		return theProjectile;
	}
	
}