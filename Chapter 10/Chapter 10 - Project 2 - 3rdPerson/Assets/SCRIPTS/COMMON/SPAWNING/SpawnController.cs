using UnityEngine;
using System.Collections;

[AddComponentMenu("Utility/Spawn Controller")]

public class SpawnController : ScriptableObject 
{
	private ArrayList playerTransforms;
	private ArrayList playerGameObjects;
	
	private Transform tempTrans;
	private GameObject tempGO;
	
	private GameObject[] playerPrefabList;
	private Vector3[] startPositions;
	private Quaternion[] startRotations;
	
	// singleton structure based on AngryAnt's fantastic wiki entry over at http://wiki.unity3d.com/index.php/Singleton
	
	private static SpawnController instance;
 
	public SpawnController () 
	{
		// this function will be called whenever an instance of the SpawnController class is made
		// first, we check that an instance does not already exist (this is a singleton, afterall!)
		if (instance != null)
		{
			// drop out if instance exists, to avoid generating duplicates
			Debug.LogWarning("Tried to generate more than one instance of singleton SpawnController.");
			return;
		}
 
		// as no instance already exists, we can safely set instance to this one
		instance = this;
	}
 
	public static SpawnController Instance
	{
		// to every other script, this getter setter is way they get access to the singleton instance of this script
		get
		{
			// the other script is trying to access an instance of this script, so we need to see if an instance already exists
			if (instance == null)
			{
				// no instance exists yet, so we go ahead and create one
				 ScriptableObject.CreateInstance<SpawnController>(); // new SpawnController ();
			}
 			// now we pass the reference to this instance back to the other script so it can communicate with it
			return instance;
		}
	}
	
	public void Restart ()
	{
		playerTransforms=new ArrayList();
		playerGameObjects=new ArrayList();
	}
	
	public void SetUpPlayers (GameObject[] playerPrefabs, Vector3[] playerStartPositions, Quaternion[] playerStartRotations, Transform theParentObj, int totalPlayers)
	{
		// we pass in everything needed to spawn players and take care of spawning players in this class so that we don't 
		// have to replicate this code in every game controller
		playerPrefabList= playerPrefabs;
		startPositions= playerStartPositions;
		startRotations= playerStartRotations;
		
		// call the function to take care of spawning all the players and putting them in the right places
		CreatePlayers( theParentObj, totalPlayers );
	}
	
	public void CreatePlayers ( Transform theParent, int totalPlayers )
	{		
		playerTransforms=new ArrayList();
		playerGameObjects=new ArrayList();
		
		for(int i=0; i<totalPlayers;i++)
		{
			// spawn a player
			tempTrans= Spawn ( playerPrefabList[i], startPositions[i], startRotations[i] );
			
			// if we have passed in an object to parent the players to, set the parent
			if(theParent!=null)
			{
				tempTrans.parent= theParent;
				// as we are parented, let's set the local position
				tempTrans.localPosition= startPositions[i];
			}
			
			// add this transform into our list of player transforms
			playerTransforms.Add(tempTrans);
			
			// add its gameobject into our list of player gameobjects (we cache them seperately)
			playerGameObjects.Add (tempTrans.gameObject);
		}
	}
	
	public GameObject GetPlayerGO (int indexNum)
	{
		return (GameObject)playerGameObjects[indexNum];	
	}
	
	public Transform GetPlayerTransform (int indexNum)
	{
		return (Transform)playerTransforms[indexNum];	
	}
	
	public Transform Spawn(GameObject anObject, Vector3 aPosition, Quaternion aRotation)
	{
		// instantiate the object
		tempGO=(GameObject)Instantiate(anObject, aPosition, aRotation);
		tempTrans= tempGO.transform;

		// return the object to whatever was calling
		return tempTrans;
	}
	
	// here we just provide a convenient function to return the spawned objects gameobject rather than its transform
	public GameObject SpawnGO(GameObject anObject, Vector3 aPosition, Quaternion aRotation)
	{
		// instantiate the object
		tempGO=(GameObject)Instantiate(anObject, aPosition, aRotation);
		tempTrans= tempGO.transform;
		
		// return the object to whatever was calling
		return tempGO;
	}

	public ArrayList GetAllSpawnedPlayers()
	{
		return playerTransforms;
	}
}
