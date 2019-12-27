using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Transform cameraTarget;
    public Transform lookAtTarget;

    private Transform myTransform;

    public Vector3 targetOffset = new Vector3(0, 0, -1);
	public float minDist= 3f;

    void Start()
    {
        // grab a ref to the transform
        myTransform = transform;

		myTransform.position = cameraTarget.position + targetOffset;

	}

    void Update()
    {
		Vector3 camTargetPos = cameraTarget.position + targetOffset;

		if (Vector3.Distance(myTransform.position, camTargetPos) > minDist)
		{
			// move this gameObject's transform around to follow the target (using Lerp to move smoothly)
			myTransform.position = Vector3.Lerp(myTransform.position, camTargetPos, Time.deltaTime);
		}

		// look at our target object
        myTransform.LookAt(lookAtTarget);
    }
}
