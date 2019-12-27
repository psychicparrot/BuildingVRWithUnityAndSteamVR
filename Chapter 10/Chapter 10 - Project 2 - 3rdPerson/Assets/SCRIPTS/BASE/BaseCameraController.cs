using UnityEngine;
using System.Collections;

[AddComponentMenu("Base/Camera Controller")]

public class BaseCameraController : MonoBehaviour {
	
	public Transform cameraTarget;
	
	public virtual void SetTarget(Transform aTarget)
	{
		cameraTarget=aTarget;	
	}
}
