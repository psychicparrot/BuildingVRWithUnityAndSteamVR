using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

using Valve.VR;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

public class LaserButtonClicker : MonoBehaviour
{
	public SteamVR_Input_Sources myHand;
	public SteamVR_Action_Boolean interactAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default","InteractUI");

	SteamVR_LaserPointer laserPointer;
	GameObject btn;

	private int deviceIndex = -1;
	public bool pointerOnButton = false;

	void Start()
	{
		laserPointer = GetComponent<SteamVR_LaserPointer>();

		laserPointer.PointerIn += LaserPointer_PointerIn;
		laserPointer.PointerOut += LaserPointer_PointerOut;
	}

	private void SetDeviceIndex(int index)
	{
		deviceIndex = index;
	}
	
	private void LaserPointer_PointerOut(object sender, PointerEventArgs e)
	{
		if (btn != null)
		{
			pointerOnButton = false;
			InputModule.instance.HoverEnd(btn);
			btn = null;
		}
	}

	private void LaserPointer_PointerIn(object sender, PointerEventArgs e)
	{
 		if (e.target.gameObject.GetComponent<Button>() != null && btn == null)
		{
			btn = e.target.gameObject;
			InputModule.instance.HoverBegin(btn);
			pointerOnButton = true;
		}
	}

	void Update()
	{
		if (pointerOnButton)
		{
			if(interactAction[myHand].stateDown)
			{
				InputModule.instance.Submit(btn);
			}
		}
	}
}
