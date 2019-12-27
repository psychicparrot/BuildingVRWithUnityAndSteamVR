using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[AddComponentMenu("Sample Game Glue Code/Laser Blast Survival/In-Game UI")]

public class UI_LBS : BaseUIDataManager
{
	public GameObject gameOverMessage;
	public GameObject getReadyMessage;
    public Text scoreText;
    public Text highScoreText;
    public Text livesText;

    void Awake()
	{
		Init();
	}
	
	void Init()
	{
		LoadHighScore();
		
		HideMessages ();
		
		Invoke("ShowGetReady",1);
		Invoke("HideMessages",2);
		
	}
	
	public void HideMessages()
	{
		gameOverMessage.SetActive(false);
		getReadyMessage.SetActive(false);
	}

	public void ShowGetReady()
	{
		getReadyMessage.SetActive(true);
	}

	public void ShowGameOver()
	{
		SaveHighScore();
		
		// show the game over message
		gameOverMessage.SetActive(true);
	}
	
    void LateUpdate()
    {
        scoreText.text = player_score.ToString();
        highScoreText.text = player_highscore.ToString();
        livesText.text = player_lives.ToString();
    }

}
