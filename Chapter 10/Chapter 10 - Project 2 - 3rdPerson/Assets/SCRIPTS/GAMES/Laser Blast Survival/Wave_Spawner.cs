using UnityEngine;
using System.Collections;

[AddComponentMenu("Sample Game Glue Code/Laser Blast Survival/Spawn wave controller")]

public class Wave_Spawner : MonoBehaviour 
{
	public bool randomSpawning;
	
	public float timeBeforeFirstSpawn=1f;
	
	public GameObject[] spawnObjectPrefabs;
	
	private int totalSpawnObjects;
	private int currentObjectNum;
	
	public int currentWave;
	
	void Start ()
	{
		// count how many objects we have lined up in the spawn object list
		foreach( GameObject go in spawnObjectPrefabs )
		{
			totalSpawnObjects++;
		}
		
		Debug.Log("Wave_Spawner.cs found "+totalSpawnObjects+" spawn objects to choose from.");
		
		// schedule first attack wave
		Invoke("LaunchWave",timeBeforeFirstSpawn);
	}
	
	public void LaunchWave()
	{
		CancelInvoke("LaunchWave");
				
		if( randomSpawning )
		{
			currentObjectNum= Random.Range ( 0, totalSpawnObjects-1 );
		} else {
			currentObjectNum++;
			
			// loop back to 0 when we reach the end of the current 'run' of things to spawn
			if( currentObjectNum > totalSpawnObjects-1 )
			{
				currentObjectNum=0;
				// you could also implement something to tell game control that all waves have finished if
				// you were making a game that only lasted until that happens
			}
		}
	
		// create an object
		GameObject tempObj= SpawnController.Instance.SpawnGO( spawnObjectPrefabs[currentObjectNum], Vector3.zero, Quaternion.identity );
		
		WaveProperties tempProps= tempObj.GetComponent<WaveProperties>();
		currentWave= tempProps.enemiesInWave;
		
		// play a spawn sound effect, which should be a 2d sound so that its position doesn't matter
		BaseSoundController.Instance.PlaySoundByIndex(4, Vector3.zero);
	}
	
	public void Fragged()
	{
		// one enemy down
		currentWave--;
		
		if( currentWave<=0 )
		{
			// this wave is done, let's start the next one!
			LaunchWave ();
		}
	}
}
