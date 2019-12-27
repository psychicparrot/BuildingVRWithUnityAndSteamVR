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
		_levelLoader.Trigger();
    }

	public void ExitGame()
	{
		Debug.Log("Exit game...");
		SteamVR_Fade.Start(Color.black, 2);
	}
}
