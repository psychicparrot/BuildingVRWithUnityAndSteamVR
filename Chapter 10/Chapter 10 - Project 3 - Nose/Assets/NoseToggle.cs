using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoseToggle : MonoBehaviour
{
	public GameObject noseModel;
	public bool startOff;

	void Start()
	{
		if(startOff)
			noseModel.SetActive(false);
	}

	void Update()
    {
		if(Input.GetKeyUp(KeyCode.N))
		{
			noseModel.SetActive(!noseModel.activeSelf);
		}
    }
}
