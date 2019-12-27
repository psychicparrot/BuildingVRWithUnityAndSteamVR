using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayHandleAnimator : MonoBehaviour
{
	public Transform _sprayHandle;
	public float activeAngle = 11.66f;
	public float inactiveAngle = -16.9f;

	private Vector3 targetRot;

	void Start()
	{
		ResetHandle();
	}

	void Update()
	{
		//targetRot.y = 0;
		//targetRot.z = 0;

		//if (_sprayHandle.localEulerAngles!=targetRot)
		//	_sprayHandle.localEulerAngles = Vector3.Lerp(_sprayHandle.localEulerAngles, targetRot, Time.deltaTime * 0.9f);
	}

	public void StartSpray()
    {
		targetRot = Vector3.zero;
		targetRot.x = activeAngle;
		_sprayHandle.localEulerAngles = targetRot;
	}

    
    public void EndSpray()
    {
		CancelInvoke("ResetHandle");
		Invoke("ResetHandle", 0.25f);
	}

	void ResetHandle()
	{
		targetRot = Vector3.zero;
		targetRot.x = inactiveAngle;
		_sprayHandle.localEulerAngles = targetRot;
	}
}
