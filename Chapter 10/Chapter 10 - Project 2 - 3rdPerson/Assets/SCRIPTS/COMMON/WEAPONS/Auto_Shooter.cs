// this weapon script fires a projectile automatically, with a delay of <fireDelay> seconds in-between

using UnityEngine;
using System.Collections;

[AddComponentMenu("Common/Automatic Shooting Weapon Script")]

public class Auto_Shooter : BaseWeaponScript {
	
	private float fireDelay= 0.2f;
	private Vector3 fireDirection;
	
	public override void Enable()
	{
		// drop out if firing is disabled
		if(canFire==true)
			return;
		
		// enable weapon (do things like show the weapons mesh etc.)
		canFire=true;
		
		// schedule the first fire
		CancelInvoke("FireProjectile");
		InvokeRepeating("FireProjectile", fireDelay, fireDelay);
	}
	
	public void FireProjectile( int ownerID )
	{
		// drop out if firing is disabled
		if(!canFire)
			return;
		
		fireDirection= myTransform.forward;
		
		// make our first projectile
		theProjectile=MakeProjectile( ownerID );
		
		// point the projectile in the direction we want to fire in
		theProjectile.LookAt( myTransform.position + fireDirection );
		
		// add some force to move our projectile
		theProjectile.GetComponent<Rigidbody>().AddRelativeForce(theProjectile.forward * projectileSpeed,ForceMode.VelocityChange);
	}
}
