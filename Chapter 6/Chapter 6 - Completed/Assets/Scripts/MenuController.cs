using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MenuController : MonoBehaviour
{
	public SteamVR_LoadLevel _levelLoader;
	
    public void StartGame()
    {
		Debug.Log("Start game...");

		// now tell SteamVR to load the main game scene
		_levelLoader.levelName = "main_scene";
		_levelLoader.fadeOutTime = 1f;
		_levelLoader.Trigger();
    }

	public void ExitGame()
	{
		Debug.Log("Exit game...");

		// start a nice fade out before we close the game completely
		
	}
}
