using UnityEngine;
using System.Collections;

[AddComponentMenu("Base/Player Manager")]

public class BasePlayerManager : MonoBehaviour
{
	public bool didInit;
	
	// the user manager and AI controllers are publically accessible so that
	// our individual control scripts can access them easily
	public BaseUserManager DataManager;
	
	// note that we initialize on Awake in this class so that it is ready for other classes to
	// access our details when they initialize on Start
	public virtual void Awake ()
	{
		didInit=false;
		
		// rather than clutter up the start() func, we call Init to do any
		// startup specifics
		Init();
	}
	
	public virtual void Init ()
	{
		// cache ref to our user manager
		DataManager= gameObject.GetComponent<BaseUserManager>();
		
		if(DataManager==null)
			DataManager= gameObject.AddComponent<BaseUserManager>();
		
		// do play init things in this function
		didInit= true;
	}
		
	public virtual void GameFinished()
	{
		DataManager.SetIsFinished(true);
	}
	
	public virtual void GameStart()
	{
		DataManager.SetIsFinished(false);
	}
}
