using UnityEngine;
using System.Collections;

[AddComponentMenu("Common/Projectile Control")]

public class ProjectileController : MonoBehaviour 
{
	public bool doProjectileHitParticle;
	public GameObject particleEffectPrefab;
	public int ownerType_id;
	
	private Transform myTransform;
	private Vector3 tempVEC;
	
	public bool shouldFollowGround;
	public float groundHeightOffset= 15f;
	public LayerMask groundLayerMask;
	
	public int overrideDamageValue;
	public int overridePoints;
	
	private bool didPlaySound;
	private int whichSoundToPlayOnStart= 0;
	
	void Start ()
	{
		myTransform= transform;
		
		didPlaySound= false;
	}
	
	// having an SetOwnerType id means we can assign a number to represent a sender (so we know who fired)
	public void SetOwnerType(int aNum)
	{
		ownerType_id= aNum;
		transform.name= aNum.ToString();
	}
	
	void Update()
	{
		// we play the pew sound here in the Update loop because we want it to have been positioned in the right
		// place when we do it. If we play the sound in Start() it may be that the projectile is not yet set up
		if(!didPlaySound)
		{
			// tell our sound controller to play a pew sound
			BaseSoundController.Instance.PlaySoundByIndex(whichSoundToPlayOnStart, myTransform.position);
			// we only want to play the sound once, so set didPlaySound here
			didPlaySound=true;
		}
		
		if(shouldFollowGround)
		{
			// cast a ray down from the waypoint to try to find the ground
			tempVEC= myTransform.position;
			
			RaycastHit hit;
			if(Physics.Raycast( tempVEC, -Vector3.up, out hit, groundLayerMask )){
				tempVEC.y= hit.point.y+groundHeightOffset;
				myTransform.position= tempVEC;
			}
		}
	}
	
	void OnCollisionEnter(Collision col)
	{		
		// if we have assigned a particle effect, we will instantiate one when a collision happens.
		if(doProjectileHitParticle)
			Instantiate(particleEffectPrefab, transform.position, Quaternion.identity);
		
		// destroy this game object after a collision
		Destroy(gameObject);
	}
	
}
