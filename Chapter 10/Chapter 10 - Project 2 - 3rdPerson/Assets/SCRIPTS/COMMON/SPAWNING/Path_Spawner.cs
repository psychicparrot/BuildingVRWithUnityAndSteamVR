using UnityEngine;
using System.Collections;

[AddComponentMenu("Common/Spawn Prefabs and Tell To Follow Path")]

public class Path_Spawner : MonoBehaviour
{
	public Waypoints_Controller waypointControl;
	
	// should we start spawning based on distance from the camera?
	// if distanceBased is false, we will need to call this class from elsewhere, to spawn
	public bool distanceBasedSpawnStart;
	// if we're using distance based spawning, at what distance should we start?
	public float distanceFromCameraToSpawnAt = 35f;
	
	// if the distanceBasedSpawnStart is false, we can have the path spawner just start spawning
	// automatically
	public bool shouldAutoStartSpawningOnLoad;
	
	public float timeBetweenSpawns=1;
	public int totalAmountToSpawn=10;
	public bool shouldReversePath;
	
	public GameObject[] spawnObjectPrefabs;
	
	private int totalSpawnObjects;
	
	private Transform myTransform;
	private GameObject tempObj;

	private int spawnCounter=0;
	private int currentObjectNum;
	private Transform cameraTransform;
	private bool spawning;
	
	public bool shouldSetSpeed;
	public float speedToSet;
	
	public bool shouldSetSmoothing;
	public float smoothingToSet;
	
	public bool shouldSetRotateSpeed;
	public float rotateToSet;
	
	private bool didInit;
	
	void Start ()
	{
		Init();
	}
	
	void Init ()
	{
		// cache ref to our transform
		myTransform = transform;
		
		// cache ref to the camera
		Camera mainCam = Camera.main;
		
		if( mainCam==null )
			return;
		
		cameraTransform = mainCam.transform;
		
		// tell waypoint controller if we want to reverse the path or not
		waypointControl.SetReverseMode(shouldReversePath);

		totalSpawnObjects= spawnObjectPrefabs.Length;

		if(shouldAutoStartSpawningOnLoad)
			StartWave(totalAmountToSpawn,timeBetweenSpawns);
	}
	
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0,0,1,0.5f);
		Gizmos.DrawCube(transform.position,new Vector3(200,0,distanceFromCameraToSpawnAt));
	}
	
	
	public void Update()
	{
		if(myTransform==null)
			return;

		if(cameraTransform==null)
			return;

		float aDist=Mathf.Abs(myTransform.position.z-cameraTransform.position.z);
		
		if( distanceBasedSpawnStart && !spawning && aDist<distanceFromCameraToSpawnAt)
		{
			StartWave(totalAmountToSpawn,timeBetweenSpawns);
			spawning=true;
		}
	}
	
	public void StartWave(int HowMany, float timeBetweenSpawns)
	{
		spawnCounter=0;
		totalAmountToSpawn=HowMany;
		
		// reset 
		currentObjectNum=0;
		
		CancelInvoke("doSpawn");
		InvokeRepeating("doSpawn",timeBetweenSpawns,timeBetweenSpawns);
	}
	
	void doSpawn()
	{
		SpawnObject();
	}
	
	
	public void SpawnObject()
	{
		if(spawnCounter>=totalAmountToSpawn)
		{
			// tell your script that the wave is finished here
			CancelInvoke("doSpawn");
			this.enabled=false;
			return;
		}
		
		// create an object
		tempObj=SpawnController.Instance.SpawnGO(spawnObjectPrefabs[currentObjectNum],myTransform.position,Quaternion.identity);
		
		// tell object to reverse its pathfinding, if required
		tempObj.SendMessage("SetReversePath", shouldReversePath, SendMessageOptions.DontRequireReceiver);
		
		// tell spawned object to use this waypoint controller
		tempObj.SendMessage("SetWayController",waypointControl,SendMessageOptions.DontRequireReceiver);
		
		// tell object to use this speed (again with no required receiver just incase)
		if(shouldSetSpeed)
			tempObj.SendMessage("SetSpeed",speedToSet,SendMessageOptions.DontRequireReceiver);
		
		// tell object to use this speed (again with no required receiver just incase)
		if(shouldSetSmoothing)
			tempObj.SendMessage("SetPathSmoothingRate",smoothingToSet,SendMessageOptions.DontRequireReceiver);
		
		// tell object to use this speed (again with no required receiver just incase)
		if(shouldSetRotateSpeed)
			tempObj.SendMessage("SetRotateSpeed",rotateToSet,SendMessageOptions.DontRequireReceiver);
				
		// increase the 'how many objects we have spawned' counter
		spawnCounter++;
	
		// increase the 'which object to spawn' counter
		currentObjectNum++;
		
		// check to see if we've reached the end of the spawn objects array
		if(currentObjectNum> totalSpawnObjects-1 )
			currentObjectNum=0;
	}
}
