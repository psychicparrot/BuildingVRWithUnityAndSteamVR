using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Valve.VR;
using Valve.VR.InteractionSystem;

public class ControllerInput : MonoBehaviour
{
	public SteamVR_Action_Boolean interactAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "Trigger_Press");
	public SteamVR_Action_Vibration hapticAction = SteamVR_Input.GetVibrationAction("default", "haptic", false);

	[Space]
	public bool do_haptic_buzz = true;
	public float haptic_duration = 0.1f;
	public float haptic_freq = 250f;
	public float haptic_amplitude = 0.1f;

	[Space]
	public UnityEvent OnActionEvent;
	public UnityEvent OnActionEndEvent;

	private SteamVR_Input_Sources myHand;
	private Interactable _interactableComponent;

	private bool isON;
	
	void Start()
    {
		// grab a ref to the Interactable Component
		_interactableComponent = GetComponent<Interactable>();
	}

    void Update()
    {
		// make sure we're being held before trying to find out what's holding us
		if (_interactableComponent.attachedToHand == null)
			return;

		// get the hand currently holding this spray bottle
		myHand = _interactableComponent.attachedToHand.handType;

		// double check that the hand value returned is valid
		if (myHand == null)
			return;

		// if the state has just started, then we spray..
		if (interactAction[myHand].state)
		{
			if (!isON)
			{
				ActionStart();
				isON = true;
			}
		} else
		{
			if(isON)
			{
				isON = false;
				ActionEnd();
			}
		}
	}

	void ActionStart()
	{
		// take care of the haptics
		if(do_haptic_buzz)
			hapticAction.Execute(0, haptic_duration, haptic_freq, haptic_amplitude, myHand);

		// fire off the unity event
		OnActionEvent.Invoke();
	}

	void ActionEnd()
	{
		OnActionEndEvent.Invoke();
	}
}
