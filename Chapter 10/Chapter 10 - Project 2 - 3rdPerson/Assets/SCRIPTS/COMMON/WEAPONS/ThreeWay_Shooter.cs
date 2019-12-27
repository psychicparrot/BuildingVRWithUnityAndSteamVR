// this script simply fires a projectile when the fire button is pressed

using UnityEngine;
using System.Collections;

[AddComponentMenu("Common/Weapons/Three-way Projectile Shooter WeaponScript")]

public class ThreeWay_Shooter : BaseWeaponScript 
{
	private Vector3 offsetSideFireVector;
	
	public override void FireProjectile( Vector3 fireDirection, int ownerID )
	{	
		// make our first projectile
		theProjectile=MakeProjectile( ownerID );
		
		// point the projectile in the direction we want to fire in
		theProjectile.LookAt( theProjectile.position + fireDirection );
		
		// add some force to move our projectile
		theProjectile.GetComponent<Rigidbody>().velocity= fireDirection * projectileSpeed;
		
		// -----------------------------------------------------
		
		offsetSideFireVector= new Vector3( fireDirection.z * 45,0,0);
		
		// make our second projectile
		theProjectile=MakeProjectile( ownerID );
		
		// point the projectile in the direction we want to fire in
		theProjectile.LookAt( theProjectile.position + fireDirection );
		
		// rotate it a little to the side
		theProjectile.Rotate(0,25,0);
		
		// add some force to move our projectile
		theProjectile.GetComponent<Rigidbody>().velocity= offsetSideFireVector + fireDirection * projectileSpeed;
		
		// -----------------------------------------------------

		// make our second projectile
		theProjectile=MakeProjectile( ownerID );

		// point the projectile in the direction we want to fire in
		theProjectile.LookAt( theProjectile.position + fireDirection );
		
		// rotate it a little to the side
		theProjectile.Rotate(0,-25,0);
		
		// add some force to move our projectile
		theProjectile.GetComponent<Rigidbody>().velocity= -offsetSideFireVector + fireDirection * projectileSpeed;
		
		// -----------------------------------------------------
		
		// tell our sound controller to play a pew sound
		//BaseSoundController.Instance.PlaySoundByIndex(0,theProjectile.position);	
	}
}