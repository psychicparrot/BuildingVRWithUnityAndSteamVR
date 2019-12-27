using UnityEngine;
using System.Collections;

public class MouseMove : MonoBehaviour {

    private Transform myTransform;
    private Vector3 tempVec;

	void Start () {
		// cache the ref to the Transform for speed
        myTransform = GetComponent<Transform>();
    }
	
	void Update () {
		// get the current eulerAngles from this GameObject
        tempVec = myTransform.eulerAngles;
		// set the y to the mouse position
        tempVec.y = Input.mousePosition.x;
		// now, set our GameObject's eulerAngles to the one with added mouse pos
        myTransform.eulerAngles = tempVec;
    }
}
