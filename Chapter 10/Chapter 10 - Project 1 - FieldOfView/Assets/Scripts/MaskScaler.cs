using UnityEngine;
using System.Collections;

public class MaskScaler : MonoBehaviour
{
	public Transform objectToTrack;
	[Space(10)]

	public float maskOpenScale = 2.82f;
	public float maskClosedScale = 0.5f;

	public float maxSpeed = 0.04f;
	public float transitionTime = 0.5f;

	[Space(10)]
	private float moveSpeed;
	private Vector3 lastPosition;
	private float theScale;

	private Transform myTransform;
	private Vector3 myScale;

	private float rotationMagnitude;

	void Start()
	{
		// grab some info we are going to need for each update
		myTransform = transform;
		myScale = transform.localScale;

		// set the default value for the scale variable
		theScale = maskOpenScale;

		// start out with the mask open
		myTransform.localScale = (Vector3.one * maskOpenScale);
	}

	void Update()
	{
		// figure out how fast the object we are tracking is moving..
		moveSpeed = Mathf.Min ( (objectToTrack.position - lastPosition).magnitude, maxSpeed );
		lastPosition = objectToTrack.position;

		rotationMagnitude = myTransform.rotation.eulerAngles.magnitude;

		// calculate what percentage between 0 and max speed we're currently moving at
		float maxUnit = (100.0f / maxSpeed);
		float t = ((moveSpeed * maxUnit) * 0.01f);
		
		// now use that percentage to figure out where the target scale should be
		theScale = Mathf.Lerp(maskOpenScale, maskClosedScale, t);
		Vector3 targetScale = (Vector3.one * theScale);

		// finally, we lerp the localScale property of our transform towards the target scale
		myTransform.localScale = Vector3.Lerp(myTransform.localScale, targetScale, Time.deltaTime * transitionTime);
	}
}
