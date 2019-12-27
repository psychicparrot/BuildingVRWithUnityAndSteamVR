using UnityEngine;
using System.Collections;

public class EscToQuit : MonoBehaviour
{
	void LateUpdate ()
    {
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
	}
}
