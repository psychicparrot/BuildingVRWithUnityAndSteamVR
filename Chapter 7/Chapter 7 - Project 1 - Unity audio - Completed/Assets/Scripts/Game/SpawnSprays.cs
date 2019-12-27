using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSprays : MonoBehaviour
{
	public Transform _sprayBottlePrefab;
	public Transform _spraySpawnpoint;

	public bool isOccupied;

	public float clearTimeSpawn = 1f;
	private float clearTimer;

	void Start()
	{
		SpawnSpray();
	}

	void Update()
	{
		if(!isOccupied)
		{
			clearTimer += Time.deltaTime;
			if(clearTimer>clearTimeSpawn)
			{
				SpawnSpray();
				clearTimer = 0;
			}
		} else
		{
			clearTimer = 0;
		}
	}

	void SpawnSpray()
	{
		Instantiate(_sprayBottlePrefab, _spraySpawnpoint.position, _spraySpawnpoint.rotation);
	}

	void OnTriggerStay(Collider other)
	{
		isOccupied = true;
	}

	void OnTriggerExit(Collider other)
	{
		isOccupied = false;
	}
}
