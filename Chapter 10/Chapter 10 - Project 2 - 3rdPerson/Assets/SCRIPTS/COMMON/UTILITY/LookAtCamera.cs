using UnityEngine;
using System.Collections;

[AddComponentMenu("Utility/Look At Camera")]

public class LookAtCamera : MonoBehaviour {
	public GameObject target;
	
	void LateUpdate() {
		transform.LookAt(target.transform);
	}
}
