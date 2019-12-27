using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Valve.VR;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

public class LaserButtonClicker : MonoBehaviour
{
	public SteamVR_Input_Sources myHand;
	public SteamVR_Action_Boolean interactAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default","InteractUI");

	private SteamVR_LaserPointer laserPointer;
	private GameObject btn;

	private bool pointerOnButton = false;

	void Start()
	{
		laserPointer = GetComponent<SteamVR_LaserPointer>();

		laserPointer.PointerIn += LaserPointer_PointerIn;
		laserPointer.PointerOut += LaserPointer_PointerOut;
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

	private void LaserPointer_PointerOut(object sender, PointerEventArgs e)
	{
		if (btn != null)
		{
			pointerOnButton = false;
			InputModule.instance.HoverEnd(btn);
			btn = null;
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
