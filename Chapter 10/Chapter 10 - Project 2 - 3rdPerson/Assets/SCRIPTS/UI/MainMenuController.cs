using UnityEngine;
using System.Collections;

[AddComponentMenu("UI/Generic Main Menu")]

public class MainMenuController : MonoBehaviour
{
	public int whichMenu= 0;
	
	public GUISkin menuSkin;
	
	private float buttonPosX;
	private float buttonPosY;
	
	public string gameDisplayName= "- DEFAULT GAME NAME -";
	public string gamePrefsName= "DefaultGame";
	
	public string singleGameStartScene;
	public string coopGameStartScene;
	
	public float default_width= 720;
	public float default_height= 480;
	
	public float audioSFXSliderValue;
	public float audioMusicSliderValue;
	
	public float graphicsSliderValue;
	private int detailLevels= 6;
	
	public GameObject sceneManagerPrefab;
	
	public string[] gameLevels;
	
	private GameObject sceneManagerGO;
	private SceneManager sceneManager;
	
	public bool useSceneManagerToStartgame;
	public bool isLoading;
	
	void Start()
	{
		// set up default options, if they have been saved out to prefs already
		if(PlayerPrefs.HasKey(gamePrefsName+"_SFXVol"))
		{
			audioSFXSliderValue= PlayerPrefs.GetFloat(gamePrefsName+"_SFXVol");
		} else {
			// if we are missing an SFXVol key, we won't got audio defaults set up so let's do that now
			audioSFXSliderValue= 1;
			audioMusicSliderValue= 1;
			string[] names = QualitySettings.names;
			detailLevels= names.Length;
			graphicsSliderValue= detailLevels;
			// save defaults
			SaveOptionsPrefs();
		}
		if(PlayerPrefs.HasKey(gamePrefsName+"_MusicVol"))
		{
			audioMusicSliderValue= PlayerPrefs.GetFloat(gamePrefsName+"_MusicVol");
		}
		if(PlayerPrefs.HasKey(gamePrefsName+"_GraphicsDetail"))
		{
			graphicsSliderValue= PlayerPrefs.GetFloat(gamePrefsName+"_GraphicsDetail");
		}

		Debug.Log ("quality="+graphicsSliderValue);

		// set the quality setting
		QualitySettings.SetQualityLevel( (int)graphicsSliderValue, true);
		
		// check for an instance of the scene manager, to deal with loading
		sceneManagerGO = GameObject.Find ( "SceneManager" );
		
		// if no instance exists already, we instantiate a prefab containing an empty gameObject with the SceneManager script attached
		if( sceneManagerGO == null )
		{
			// instantiate a scene manager object into the scene
			sceneManagerGO = (GameObject) Instantiate( sceneManagerPrefab );
			sceneManagerGO.name = sceneManagerPrefab.name;
		}
		
		// grab a reference to the scene manager script, so we can access it later
		sceneManager = sceneManagerGO.GetComponent<SceneManager>();
		
		// now tell scene manager about the levels we have in this game
		sceneManager.levelNames = gameLevels;
	}
	
    void LateUpdate()
    {
        if (Input.GetButtonUp("Fire1"))
        {
            PlayerPrefs.SetInt("totalPlayers", 1);
            if (!useSceneManagerToStartgame)
            {
                LoadLevel(singleGameStartScene);
            }
            else
            {
                isLoading = true;
                Debug.Log("Telling scene Manager to load next level..");
                sceneManager.GoNextLevel();
            }
        }
    }
	
	void LoadLevel( string whichLevel )
	{
		// tell the sceneManager object to deal with loading the level
		sceneManager.LoadLevel ( whichLevel );
	}
	
	void GoMainMenu()
	{
		whichMenu=0;	
	}
	
	void SaveOptionsPrefs()
	{
		PlayerPrefs.SetFloat(gamePrefsName+"_SFXVol", audioSFXSliderValue);
		PlayerPrefs.SetFloat(gamePrefsName+"_MusicVol", audioMusicSliderValue);
		PlayerPrefs.SetFloat(gamePrefsName+"_GraphicsDetail", graphicsSliderValue);
		
		// set the quality setting
		QualitySettings.SetQualityLevel( (int)graphicsSliderValue, true);
	}
	
	void ExitGame()
	{
		// tell level loader to shut down the game for us
		Application.Quit();
	}
}
