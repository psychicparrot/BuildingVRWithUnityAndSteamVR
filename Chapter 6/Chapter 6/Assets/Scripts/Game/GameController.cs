using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
	#region Public_vars

	public int timeBeforeStart = 10;

	public enum GameState { None, Init, Loaded, GetReady, GameStarting, InGame, Paused, UnPausing, GameOver };
	public GameState currentState;
	public GameState targetState;

	[Space(20)]
	public int maxStingsToEndGame = 10;

	[Space(20)]
	public float bugSpawnMinInterval = 2f;
	public float bugSpawnMaxInterval = 3f;

	[Space(20)]
	public Text _scoreText;
	public Text _stingCountText;
	public Text _finalScoreMessage;
	public Text _countdownText;

	public GameObject _getReadyUI;
	public GameObject _gameOverUI;

	[Space(20)]
	public BugSpawner _bugSpawner;

	[Space(20)]
	public static int sprayedScore;
	public static int stings;

	[Space(20)]
	public AudioSource _ouchSound;

	#endregion

	#region Private_vars

	private float currentSpawnInterval;
	private int countdownTime;

	#endregion

	#region SINGLETON

	public static GameController _instance;

	void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		} else
		{
			if (_instance != this)
				Destroy(gameObject);
		}
	}

	#endregion

	#region Init_And_Setup

	void Start()
	{
		targetState = GameState.Init;
	}

	void Init()
	{
		// hide unused UI
		Hide_CountInMessage();
		Hide_GetReadyMessage();
		Hide_GameOverMessage();

		// set game states
		targetState = GameState.Loaded;

		currentSpawnInterval = bugSpawnMaxInterval;

		Debug.Log("SET CURRENTSPAWNINTERVAL TO " + currentSpawnInterval);

		// reset score and stings
		sprayedScore = 0;
		stings = 0;
	}

	void GetReady()
	{
		// set sting text
		UpdateStingText();

		Invoke("Show_GetReadyMessage", 1f);
		Invoke("StartCountdown", 3f);
	}

	void StartCountdown()
	{
		// do a countdown before the game starts
		// show count in
		Show_CountInMessage();

		countdownTime = timeBeforeStart;
		InvokeRepeating("Countdown", 0, 2f);
	}

	void Countdown()
	{
		// set the text of the countdown text ..
		_countdownText.text = countdownTime.ToString();

		// decrease the countdown number for the next update
		countdownTime--;

		if (countdownTime < 0)
		{
			// start the game!
			targetState = GameState.GameStarting;
			Hide_CountInMessage();
			Hide_GetReadyMessage();

			// stop counting down
			CancelInvoke("Countdown");
		}
	}

	void StartGame()
	{
		// spawn the first bug
		SpawnBug();
		InvokeRepeating("IncreaseSpawnSpeed",5f, 1f);
	}

	#endregion

	#region Main_Loop

	void Update()
	{
		// if the target state is different, currentState needs to be updated and we need to do whatever
		// setup is required for the target state
		if (currentState != targetState)
		{
			UpdateTargetGameState();
		}

		// now we need to do whatever main loop things we need to do for the current state
		UpdateCurrentState();
	}

	#endregion

	#region Game_States

	void UpdateCurrentState()
	{
		switch (targetState)
		{
			// if we are to transition to the 'InGame' state, we may need to do some things as we exit from the current game state..

			case GameState.InGame:

				break;
		}

	}

	void UpdateTargetGameState()
	{
		Debug.Log("CHANGING STATE FROM " + currentState + " TO " + targetState);
		GameState tempState = targetState;

		switch (targetState)
		{
			case GameState.Init:
				Init();
				targetState = GameState.Loaded;
				break;

			case GameState.Loaded:
				targetState = GameState.GetReady;
				break;

			case GameState.GetReady:
				GetReady();
				break;

			case GameState.GameStarting:
				StartGame();
				targetState = GameState.InGame;
				break;

			case GameState.Paused:
				Time.timeScale = 0;
				break;

			case GameState.UnPausing:
				Time.timeScale = 1;
				targetState = GameState.InGame;
				break;

			// if we are to transition to the 'InGame' state, we may need to do some things as we exit from the current game state..
			case GameState.InGame:
				break;

			case GameState.GameOver:
				// show the game over message
				Show_GameOverMessage();

				// stop the recurring invoked call to spawn bugs
				CancelInvoke("SpawnBug");
				break;
		}

		// now that we've done anything we needed to do before switching state, we can go ahead and switch now..
		currentState = tempState;
	}

	#endregion

	#region Game_Functions

	void SpawnBug()
	{
		// tell spawner to spawn a bug
		_bugSpawner.Spawn();

		// stop the recurring invoked call to spawn bugs
		CancelInvoke("SpawnBug");

		float minTime = currentSpawnInterval - bugSpawnMinInterval;

		float nextSpawnTime = Random.Range(minTime, currentSpawnInterval);

		//Debug.Log("currentSpawnInterval="+ currentSpawnInterval+" - "+ bugSpawnMinInterval+"  // MIN TIME = " + minTime+" -- NEXT SPAWN TIME = " + nextSpawnTime);

		// now set up an invoke to call this function and spawn the next bug
		Invoke("SpawnBug", nextSpawnTime); // Random.Range(bugSpawnMinInterval, currentSpawnInterval));
	}

	void EndGame()
	{
		targetState = GameState.GameOver;
	}

	void IncreaseSpawnSpeed()
	{
		// as long as the spawn time is greater than the minimum, here we reduce spawn time by 0.1 every second
		if (currentSpawnInterval > bugSpawnMinInterval)
			currentSpawnInterval -= 0.01f;
	}

	#endregion

	#region Public methods

	public void SprayedBug()
	{
		if (currentState == GameState.GameOver)
			return;

		sprayedScore++;
		_scoreText.text = sprayedScore.ToString();
	}

	public void Stung()
	{
		if (currentState == GameState.GameOver)
			return;
		
		// play ouch sound
		_ouchSound.Play();

		stings++;

		UpdateStingText();

		if(stings==10)
		{
			targetState = GameState.GameOver;
		}
	}

	public void ReturnToMenu()
	{
		SceneManager.LoadScene(0);
	}

	#endregion

	#region UI_Messages

	void UpdateStingText()
	{
		_stingCountText.text = stings.ToString() + "/" + maxStingsToEndGame.ToString();
	}

	void Show_GetReadyMessage()
	{
		// show the get ready message and make sure that the game over message is hidden
		_getReadyUI.SetActive(true);
	}

	void Hide_GetReadyMessage()
	{
		// show the get ready message and make sure that the game over message is hidden
		_getReadyUI.SetActive(false);
	}

	void Show_CountInMessage()
	{
		// show the get ready message and make sure that the game over message is hidden
		_countdownText.gameObject.SetActive(true);
	}

	void Hide_CountInMessage()
	{
		// show the get ready message and make sure that the game over message is hidden
		_countdownText.gameObject.SetActive(false);
	}

	void Show_GameOverMessage()
	{
		// set the text of the game over message to show the final score
		_finalScoreMessage.text = "FINAL SCORE " + sprayedScore.ToString();

		// show the get ready message and make sure that the game over message is hidden
		_gameOverUI.gameObject.SetActive(true);
	}

	void Hide_GameOverMessage()
	{
		// show the get ready message and make sure that the game over message is hidden
		_gameOverUI.gameObject.SetActive(false);
	}

	#endregion
}
