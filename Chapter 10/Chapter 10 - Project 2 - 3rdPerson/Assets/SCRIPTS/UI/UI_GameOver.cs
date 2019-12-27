using UnityEngine;
using System.Collections;

public class UI_GameOver : MonoBehaviour {
	
	[AddComponentMenu("Common/UI/Game Over Message")]
		
	// we use gameOverMessage as a public string so that another script may change this message if needed
	// for example, the tank game's game controller sets this to either 'P1 WINS' or 'P2 WINS'
	public string gameOverMessage="GAME OVER";
	
	void OnGUI () {
		// display a simple message to say that the game is over
		GUI.Label(new Rect ((Screen.width/2)-78,(Screen.height/2)-40,100,50),gameOverMessage);
		
		// provide a simple restart game button
		if(GUI.Button(new Rect ((Screen.width/2)-80,(Screen.height/2)+20,100,25),"PLAY AGAIN"))
		{
			// when the button is pressed, we tell game controller about it
			GameController_LBS.Instance.RestartGameButtonPressed();
		}
	}
}
