using UnityEngine;
using System.Collections;

public class BattleController : MonoBehaviour
{
    private bool isFinished;
	
	private Vector3 myPosition;
	
	private Transform myTransform;
	public int howmany_frags;
	public int howMany_fraggedOthers;
	
	public bool battleRunning;
	
	private bool doneInit;
	
	// we default myID to -1 so that we will know if the script hasn't finished initializing when
	// another script tries to GetID
	private int myID =-1;
	
	public BattleController ()
	{
		myID= GlobalBattleManager.Instance.GetUniqueID( this );
				
		Debug.Log ("ID assigned is "+myID);
	}
	
	public void Init()
	{
		myTransform= transform;
		doneInit= true;
	}
	
	public int GetID ()
	{
		return myID;	
	}
	
	public void Fragged ()
	{
		howmany_frags++;
	}
	
	public void FraggedOther ()
	{
		howMany_fraggedOthers++;
	}
	
	public void GameFinished ()
	{
		isFinished=true;
		battleRunning=false;
		
		// find out which position we finished in
		int finalBattlePosition= GlobalBattleManager.Instance.GetPosition( myID );
		
		// tell our car controller about the battle ending
		gameObject.SendMessageUpwards("PlayerFinishedBattle", finalBattlePosition, SendMessageOptions.DontRequireReceiver);
	}

	public void BattleStart ()
	{
		isFinished=false;
		battleRunning=true;
	}

	public bool GetIsFinished ()
	{
		return isFinished;
	}
	
	public void UpdateBattleState( bool aState )
	{
		battleRunning= aState;
	}
		
	public void OnTriggerEnter( Collider other )
	{

	}
		
}
