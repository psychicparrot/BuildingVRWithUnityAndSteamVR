using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;
using UnityEngine.Events;

public class TimedRadialSelector : MonoBehaviour
{
	public SteamVR_Input_Sources myHand;
	public SteamVR_Action_Boolean interactAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "InteractUI");

	[Space]
	public Image _radialImage;

	[Space]
	public float actionConfirmTime = 2f;
	public bool actionDown;

	[Space]
	public UnityEvent OnSelected;

	private float actionDowntime;
	private bool checkAction;

	void Start()
    {
		// if no Image has been set up in the Inspector, let's try to grab it from this GameObject
		if (_radialImage == null)
			_radialImage = GetComponent<Image>();
    }

	void OnEnable()
	{
		// if we've been enabled, set a bool we can check against to enable action
		checkAction = true;
	}

	void OnDisable()
	{
		// if we've been disabled, disable any action be setting this bool to false
		checkAction = false;
	}

	void Update()
	{
		// if button is disabled, drop out
		if (!checkAction)
			return;

		// check to see if the action is 'happening'
		if (interactAction[myHand].state)
		{
			// add time since the last frame to our action timer
			actionDowntime += Time.deltaTime;

			// check how long the action has been down
			if (actionDowntime >= actionConfirmTime)
			{
				actionDown = false;
				// invoke the unity event
				OnSelected.Invoke();
			}
		}
		else
		{
			// if we're not pressing anything we need to reduce the progress back to zero
			// first, check that there is some progress to reset..
			if (actionDowntime > 0)
			{
				// reduce the progress
				actionDowntime -= Time.deltaTime*4f;
			}
			else
			{
				// if the progress is not greater than zero, let's make sure it doesn't drop below zero
				actionDowntime = 0;
			}
		}

		// update the Image to show the progress
		_radialImage.fillAmount = (actionDowntime / actionConfirmTime);
	}
}
