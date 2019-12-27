using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {

	private float volume;
	public string gamePrefsName= "DefaultGame"; // DO NOT FORGET TO SET THIS IN THE EDITOR!!

	public AudioClip music;
	
	public bool loopMusic;
	
	private AudioSource source;
	private GameObject sourceGO;

	private int fadeState;
	private int targetFadeState;

	private float volumeON;
	private float targetVolume;

	public float fadeTime=15f;
	public bool shouldFadeInAtStart= true;

	void Start ()
	{
		// we will grab the volume from PlayerPrefs when this script first starts
		volumeON= PlayerPrefs.GetFloat(gamePrefsName+"_MusicVol");
	
		// create a game object and add an AudioSource to it, to play music on
		sourceGO= new GameObject("Music_AudioSource");
		source= sourceGO.AddComponent<AudioSource>();
		source.name= "MusicAudioSource";
		source.playOnAwake= true;
		source.clip= music;
		source.volume= volume;

		// the script will automatically fade in if this is set
		if(shouldFadeInAtStart)
		{
			fadeState=0;
			volume=0;
		} else {
			fadeState=1;
			volume=volumeON;
		}

		// set up default values
		targetFadeState=1;
		targetVolume=volumeON;
		source.volume=volume;
	}
	
	void Update ()
	{
		// if the audiosource is not playing and it's supposed to loop, play it again (Sam?)
		if( !source.isPlaying && loopMusic )
			source.Play();

		// deal with volume fade in/out
		if(fadeState!=targetFadeState)
		{
			if(targetFadeState==1)
			{
				if(volume==volumeON)
					fadeState=1;
			} else {
				if(volume==0)
					fadeState=0;
			}

			volume=Mathf.Lerp(volume, targetVolume, Time.deltaTime * fadeTime);
			source.volume=volume;
		} 
	}

	public void FadeIn ( float fadeAmount )
	{
		volume=0;
		fadeState=0;
		targetFadeState=1;
		targetVolume=volumeON;
		fadeTime=fadeAmount;
	}

	public void FadeOut ( float fadeAmount )
	{
		volume=volumeON;
		fadeState=1;
		targetFadeState=0;
		targetVolume=0;
		fadeTime=fadeAmount;
	}
}
