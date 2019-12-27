using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugSpawner : MonoBehaviour
{
	[Space]
	public GameObject[] _airborneBugPrefabs;

	[Space]
	public float spawnCircleInnerRadius = 25f;
	public float spawnCircleOuterRadius = 30f;

	[Space(20)]
	public GameObject[] _groundBugPrefabs;

	[Space]
	public Transform _groundBugSpawnpoint;

	private Transform _attackTarget;

	void Start()
	{
		// find headset to use as an attack target
		GameObject tempGO = GameObject.Find("HeadCollider");
		if (tempGO == null)
			Debug.LogError("Could not find headset for wasp to attack!");

		_attackTarget = tempGO.transform;
	}

	public void Spawn()
	{
		int bugType = Random.Range(0, 100);

		if (bugType > 25)
		{
			// airborne
			Vector2 randomCirclePoint = Random.insideUnitCircle.normalized * Random.Range(spawnCircleInnerRadius, spawnCircleOuterRadius);

			// The the random point on the circle is on the XZ plane and the random height is the Y axis.
			Vector3 thePosition = transform.position + new Vector3(randomCirclePoint.x, 0, -Mathf.Abs(randomCirclePoint.y));

			int whichBug = Random.Range(0,_airborneBugPrefabs.Length);

			// now we instantiate the bug at the value held in thePosition 
			GameObject tempGO = (GameObject)Instantiate(_airborneBugPrefabs[whichBug], thePosition, Quaternion.identity);

			// tell the new bug where to attack!
			tempGO.SendMessage("SetTarget", _attackTarget);
		}
		else
		{
			int whichBug = Random.Range(0, _airborneBugPrefabs.Length);
			GameObject tempGO = (GameObject)Instantiate(_groundBugPrefabs[whichBug], _groundBugSpawnpoint.position, _groundBugSpawnpoint.rotation);
			// tell the new bug where to attack!
			tempGO.SendMessage("SetTarget", _attackTarget);
		}
	}
}
