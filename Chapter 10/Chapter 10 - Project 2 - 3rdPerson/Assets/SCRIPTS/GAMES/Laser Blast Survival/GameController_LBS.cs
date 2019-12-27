using UnityEngine;
using System.Collections;

[AddComponentMenu("Sample Game Glue Code/Laser Blast Survival/Game Controller")]

public class GameController_LBS : BaseGameController
{
	public string mainMenuSceneName = "menu_LBS";
	public GameObject[] playerPrefabList;
	
	public Wave_Spawner WaveSpawnController;
	
	public Transform playerParent;
    public Transform [] startPoints;
    
	[System.NonSerialized]
    public GameObject playerGO1;
	
	private Vector3[] playerStarts;
	private Quaternion[] playerRotations;
	
    private ArrayList playerList;
	private ArrayList playerTransforms;
	
	private Player_LBS thePlayerScript;
	private Player_LBS focusPlayerScript;
	
	[System.NonSerialized]
	public BaseUserManager mainPlayerDataManager1;
	
	private int numberOfPlayers;
	
	public UI_LBS UIControl;
	
	[System.NonSerialized]
	public static GameController_LBS Instance;
	
	public float gameSpeed=1;
		
	public GameController_LBS()
	{
		Instance=this;
	}
	
	public void Start()
	{
		Init();
		Time.timeScale=gameSpeed;
	}
	
	public void Init()
	{
		Invoke ("StartPlayer",1);
		
		SpawnController.Instance.Restart();
		
		numberOfPlayers= playerPrefabList.Length;
		
		// initialize some temporary arrays we can use to set up the players
        Vector3 [] playerStarts = new Vector3 [numberOfPlayers];
        Quaternion [] playerRotations = new Quaternion [numberOfPlayers];

        // we are going to use the array full of start positions that must be set in the editor, which means we always need to
        // make sure that there are enough start positions for the number of players

        for ( int i = 0; i < numberOfPlayers; i++ )
        {
            // grab position and rotation values from start position transforms set in the inspector
            playerStarts [i] = (Vector3) startPoints [i].position;
            playerRotations [i] = ( Quaternion ) startPoints [i].rotation;
        }
		
        SpawnController.Instance.SetUpPlayers( playerPrefabList, playerStarts, playerRotations, playerParent, numberOfPlayers );
		
		playerTransforms=new ArrayList();
		
		// now let's grab references to each player's controller script
		playerTransforms = SpawnController.Instance.GetAllSpawnedPlayers();
		
		playerList=new ArrayList();
		
		for ( int i = 0; i < numberOfPlayers; i++ )
        {
			Transform tempT= (Transform)playerTransforms[i];
			Player_LBS tempController= tempT.GetComponent<Player_LBS>();
			playerList.Add(tempController);
			tempController.Init ();
		}
		
        // grab a ref to the player's gameobject for later
        playerGO1 = SpawnController.Instance.GetPlayerGO( 0 );

        // grab a reference to the focussed player's car controller script, so that we can
        // do things like access its speed variable
        thePlayerScript = ( Player_LBS ) playerGO1.GetComponent<Player_LBS>();

        // assign this player the id of 0
        thePlayerScript.SetID( 0 );

        // set player control
        thePlayerScript.SetUserInput( true );

        // as this is the user, we want to focus on this for UI etc.
        focusPlayerScript = thePlayerScript;

		// see if we have a camera target object to look at
		Transform aTarget= playerGO1.transform.Find("CamTarget");
        GameObject cameraParent = GameObject.FindWithTag("cameraParent");

        if (aTarget!=null)
		{
            // if we have a camera target to aim for, instead of the main player, we use that instead
            cameraParent.SendMessage("SetTarget", aTarget );
		} else {
            // tell the camera script to target the player
            cameraParent.SendMessage("SetTarget", playerGO1.transform );
		}
	}
	
	void StartPlayer()
	{
		// grab a reference to the main player's data manager so we can update its values later on (scoring, lives etc.)
		mainPlayerDataManager1= playerGO1.GetComponent<BasePlayerManager>().DataManager;
		
		// all ready to play, let's go!
		thePlayerScript.GameStart();
	}
	
	public override void EnemyDestroyed ( Vector3 aPosition, int pointsValue, int hitByID )
	{
		// tell our sound controller to play an explosion sound
		BaseSoundController.Instance.PlaySoundByIndex( 1, aPosition );
		
		// tell main data manager to add score
		mainPlayerDataManager1.AddScore( pointsValue );
			
		// update the score on the UI
		UpdateScoreP1( mainPlayerDataManager1.GetScore() );
		
		// play an explosion effect at the enemy position
		Explode ( aPosition );
		
		// tell spawn controller that we're one enemy closer to the next wave
		WaveSpawnController.Fragged();
	}
	
	public void PlayerHit(Transform whichPlayer)
	{
		// tell our sound controller to play an explosion sound
		BaseSoundController.Instance.PlaySoundByIndex( 2, whichPlayer.position );
		
		// call the explosion function!
		Explode( whichPlayer.position );
	}
		
	public Player_LBS GetMainPlayerScript ()
	{
		return focusPlayerScript;
	}
	
	public Transform GetMainPlayerTransform ()
	{
		return playerGO1.transform;
	}
	
	public GameObject GetMainPlayerGO ()
	{
		return playerGO1;
	}
	
	public void PlayerDied(int whichID)
	{
		// this is a single player game, so just end the game now
		// both players are dead, so end the game
		UIControl.ShowGameOver();
		Invoke ("Exit",5);
	}
	
	void Exit()
	{
		Application.LoadLevel( mainMenuSceneName );
	}
	
	// UI update calls
	// 
	public void UpdateScoreP1( int aScore )
	{
		UIControl.UpdateScoreP1( aScore );
	} 
	
	public void UpdateLivesP1( int aScore )
	{
		UIControl.UpdateLivesP1( aScore );
	}
	
}
