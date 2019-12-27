using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class SprayController : MonoBehaviour
{

	[Space]
	public Transform _sprayPoint;
	public Transform _sprayColliderPrefab;
	public Transform _sprayParticlePrefab;

	[Space]
	public int ammo = 20;
	public Text _ammoDisplay;
	public LayerMask groundLayerMask;

	void Start()
	{
		_ammoDisplay.text = ammo.ToString("D2");
	}

	public void StartSpray()
	{
		if (ammo <= 0)
			return;

		// decrease ammo and update our display
		ammo--;
		_ammoDisplay.text = ammo.ToString("D2");

		MakeSprayCollider();

		// show spray effect
		Instantiate(_sprayParticlePrefab, _sprayPoint.position, _sprayPoint.rotation);
	}

	void MakeSprayCollider()
	{
		float dist = 2f;
		RaycastHit hit;

		// Does the ray intersect any objects excluding the player layer
		if (Physics.Raycast(_sprayPoint.position, _sprayPoint.TransformDirection(Vector3.forward), out hit, dist, groundLayerMask))
		{
			dist= hit.distance;
		}

		// make invisible collision mesh to hit bugs with
		Instantiate(_sprayColliderPrefab, (_sprayPoint.position + _sprayPoint.TransformDirection(Vector3.forward) * dist), Quaternion.identity);
	}
}
