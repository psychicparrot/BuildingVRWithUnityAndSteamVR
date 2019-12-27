// This script was adapted from Unity Interaction VRUI system's VREyeRaycaster Component

using System;
using UnityEngine;

using Valve.VR.InteractionSystem;
using UnityEngine.UI;

public class HeadInteractor : MonoBehaviour
{
	public Transform headForRaycast;                         // The start point for the raycast / look detection

	[Space]
	public Transform cursor;                              // A GameObject to use as a cursor (with the progress UI attached to it)
	public Image progressImage;                           // An Unity UI image set to Image Type Filled, to use as a stare progress image

	[Space]
	public Vector3 rayPositionOffset;                             // A positional offset for the start of the raycast

	public LayerMask layerMask;           // Layers to exclude from the raycast.
	private float rayLength = 500f;              // How far into the Scene the ray is cast.

	private GameObject currentInteractable;                //The current interactive item
	private GameObject lastInteractable;                   //The last interactive item

	public float stareTimer;                            // How long the user has been staring at this item
	public float activateTime = 3;                       // How long you can stare until it counts as a click / interaction

	[Space]
	public bool isEnabled;                                // Should we even do anything?

	private void Awake()
	{
		if (cursor != null)
		{
			// Hide the cursor
			cursor.gameObject.SetActive(false);
		}

		// Enable look detection
		isEnabled = true;
	}

	private void Update()
	{
		// Quick error check to make sure we have a start point
		if (headForRaycast == null)
		{
			Debug.LogError("You need to set the Transform to your Camera or FollowHead, on the Head Pointer Component!");
			Debug.Break();
			return;
		}

		// Call on the raycasting and detection function if isEnabled is true...
		if (isEnabled)
			EyeRaycast();
	}

	private void EyeRaycast()
	{
		// Work out the position from the headForRaycast Transform with the rayPositionOffset added to it
		Vector3 adjustedPosition = headForRaycast.position + (headForRaycast.right * rayPositionOffset.x) + (headForRaycast.up * rayPositionOffset.y) + (headForRaycast.forward * rayPositionOffset.z);

		// Create a ray that points forwards from the camera.
		Ray ray = new Ray(adjustedPosition, headForRaycast.forward);
		RaycastHit hit;

		// Do the raycast forweards to see if we hit an interactive item
		if (Physics.Raycast(ray, out hit, rayLength, layerMask))
		{
			if (cursor != null)
			{
				// Show the cursor, set its position and rotation
				cursor.gameObject.SetActive(true);
				cursor.position = hit.point;
				cursor.rotation = headForRaycast.rotation; //      The rotation matches the head so that it should always be facing the camera
			}

			// See if the object we hit has a Button Component attached to it
			Button aButton = hit.transform.GetComponent<Button>();
			if (aButton == null)
			{
				// No button was hit, so deactivate the last interactive item.
				DeactivateLastInteractable();
				currentInteractable = null;
				return;
			}

			// Grab the GameObject of the button we hit
			currentInteractable = aButton.gameObject;

			// If we hit an interactive item and it's not the same as the last interactive item, then call hover begin
			if (currentInteractable && currentInteractable != lastInteractable)
			{
				// Use the SteamVR InputModule to start a UI hover
				InputModule.instance.HoverBegin(currentInteractable);
			}
			else if (currentInteractable == lastInteractable)
			{
				// Count stare time and update fillAmount of our progress display
				stareTimer += Time.deltaTime;

				if (progressImage != null)
				{
					progressImage.fillAmount = (stareTimer / activateTime);
				}

				// If we have been staring for longer than activateTime, count it as a click/interaction
				if (stareTimer > activateTime)
				{
					// Use the SteamVR InputModule to tell the button we want to interact with it
					InputModule.instance.Submit(currentInteractable);

					// Reset the stare timer
					stareTimer = 0;

					// Clear out the interactable store
					DeactivateLastInteractable();

					// Disable our look detection for a bit after a click, then make a call to ReEnable() to reenable it in x seconds
					isEnabled = false;
					Invoke("ReEnable", 1f);
				}
			}

			// Deactivate the last interactive item 
			if (currentInteractable != lastInteractable)
				DeactivateLastInteractable();

			lastInteractable = currentInteractable;
		}
		else
		{
			// Nothing was hit, deactive the last interactive item.
			DeactivateLastInteractable();
			currentInteractable = null;
		}
	}

	void ReEnable()
	{
		// This is called after a click, to re-enable the button after a temporary switch off to avoid accidental multi-clicks
		isEnabled = true;
	}

	private void DeactivateLastInteractable()
	{
		// Reset stare timer and set the progress image's fillAmount to zero
		stareTimer = 0;

		if (progressImage != null)
		{
			progressImage.fillAmount = 0;
		}

		// If we don't have a reference to a previously stored interactable object, drop out
		if (lastInteractable == null)
			return;

		// Use SteamVR's InputModule to tell the button that we're done with it (HoverEnd)
		InputModule.instance.HoverEnd(lastInteractable);

		// Null the lastInteractable object store
		lastInteractable = null;

		if (cursor != null)
		{
			// Hide the cursor
			cursor.gameObject.SetActive(false);
		}
	}
}