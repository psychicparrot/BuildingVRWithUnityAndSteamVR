using UnityEngine;
using System.Collections;

[AddComponentMenu("Common/Spawn Prefabs (no path following)")]

public class Spawner : MonoBehaviour 
{

	// should we start spawning based on distance from the camera?
	// if distanceBased is false, we will need to call this class from elsewhere, to spawn
	public bool distanceBasedSpawnStart;
	
	// if we're using distance based spawning, at what distance should we start?
	public float distanceFromCameraToSpawnAt= 35f;
	
	// if the distanceBasedSpawnStart is false, we can have the path spawner just start spawning
	// automatically
	public bool shouldAutoStartSpawningOnLoad;
	
	public float timeBetweenSpawns= 1;
	public bool startDelay;
	public int totalAmountToSpawn= 10;
	
	public bool shouldRepeatWaves;
	
	public bool shouldRandomizeSpawnTime;
	public float minimumSpawnTimeGap= 0.5f;
	
	public GameObject[] spawnObjectPrefabs;
	
	private int totalSpawnObjects;
	
	private Transform myTransform;
	private GameObject tempObj;

	private int spawnCounter= 0;
	private int currentObjectNum;
	private Transform cameraTransform;
	private bool spawning;
	
	public bool shouldSetSpeed;
	public float speedToSet;
	
	public bool shouldSetSmoothing;
	public float smoothingToSet;
	
	public bool shouldSetRotateSpeed;
	public float rotateToSet;
	
	void Start ()
	{
		// cache ref to our transform
		myTransform= transform;

		// cache ref to the camera
		cameraTransform = GameObject.Find("Camera").transform;
		
		// tell waypoint controller if we want to reverse the path or not
		//waypointControl.SetReverseMode(shouldReversePath);
		
		// count how many objects we have lined up in the spawn object list
		foreach( GameObject go in spawnObjectPrefabs )
		{
			totalSpawnObjects++;
		}
		
		if(shouldAutoStartSpawningOnLoad)
		{
			// note that the first spawn will not happen until timeBetweenSpawns seconds has elapsed
			testSpawn();
		}
	}
	
	public void OnDrawGizmosSelected()
	{
		// we will only draw the distance helper gizmos if we're using distance based wave triggering
		if(distanceBasedSpawnStart)
		{
			Gizmos.color= new Color( 0, 0, 1, 0.5f );
			Gizmos.DrawSphere( transform.position, distanceFromCameraToSpawnAt );
		}
	}
	
	
	public void Update()
	{
		float aDist=Mathf.Abs( myTransform.position.z-cameraTransform.position.z );
		
		if( distanceBasedSpawnStart && !spawning && aDist<distanceFromCameraToSpawnAt )
		{
			testSpawn();
			spawning=true;
		}
	}
	
	void testSpawn()
	{
		StartWave( totalAmountToSpawn, timeBetweenSpawns );
	}
	
	public void StartWave( int HowMany, float timeBetweenSpawns )
	{
		spawnCounter= 0;
		totalAmountToSpawn= HowMany;
		
		// reset 
		currentObjectNum= 0;
		
		CancelInvoke("doSpawn");
		
		// the option is there to spawn at random times, or at fixed intervals...
		if(shouldRandomizeSpawnTime)
		{
			// do a randomly timed invoke call, based on the times set up in the inspector
			Invoke("doSpawn", Random.Range ( minimumSpawnTimeGap, timeBetweenSpawns ));
		} else {
			// do a regularly scheduled invoke call based on times set in the inspector
			InvokeRepeating( "doSpawn", timeBetweenSpawns, timeBetweenSpawns );
		}
	}
	
	void doSpawn()
	{
		SpawnObject();
	}
	
	
	public void SpawnObject()
	{
		if( spawnCounter>=totalAmountToSpawn )
		{
			// if we are in 'repeat' mode, we will just reset the value of spawnCounter to 0 and carry on spawning
			if( shouldRepeatWaves )
			{
				spawnCounter= 0;
			} else {
				// as we are not going to repeat the wave (shouldRepeatWaves=false) we need to drop out here and disable this script
				CancelInvoke("doSpawn");
				this.enabled= false;
				return;
			}
		}
		
		// create an object
		tempObj=SpawnController.Instance.SpawnGO( spawnObjectPrefabs[currentObjectNum], myTransform.position, Quaternion.identity );
		
		// tell object to use this speed (again with no required receiver just incase)
		if(shouldSetSpeed)
			tempObj.SendMessage( "SetSpeed", speedToSet, SendMessageOptions.DontRequireReceiver );
				
		// tell object to use this speed (again with no required receiver just incase)
		if(shouldSetRotateSpeed)
			tempObj.SendMessage( "SetRotateSpeed", rotateToSet ,SendMessageOptions.DontRequireReceiver );
				
		// increase the 'how many objects we have spawned' counter
		spawnCounter++;
	
		// increase the 'which object to spawn' counter
		currentObjectNum++;
		
		// check to see if we've reached the end of the spawn objects array
		if(currentObjectNum> totalSpawnObjects-1 )
			currentObjectNum= 0;
		
		if(shouldRandomizeSpawnTime)
		{
			// cancel invoke for safety
			CancelInvoke("doSpawn");
			
			// schedule the next random spawn
			Invoke("doSpawn", Random.Range ( minimumSpawnTimeGap, timeBetweenSpawns ));
		}
	}
	
}
