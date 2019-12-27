using UnityEngine;
using System.Collections;

public class RaceController : MonoBehaviour
{
    private bool isFinished;
	private bool isLapDone;
	private float currentWaypointDist;
	private int currentWaypointNum;
	
	private float waypointDistance= 110f;
	
	private Vector3 myPosition;
	private Vector3 diff;
	private int totalWaypoints;
	
	private Waypoints_Controller waypointManager;
	private Transform currentWaypointTransform;
	private Transform myTransform;
	private Vector3 nodePosition;
	private float targetAngle;
	private int lapsComplete;
	public bool goingWrongWay;
	public bool oldWrongWay;
	public bool raceRunning;
	public float timeWrongWayStarted;
	
	private bool doneInit;
	
	// we default myID to -1 so that we will know if the script hasn't finished initializing when
	// another script tries to GetID
	private int myID =-1;
	
	public RaceController ()
	{
		myID= GlobalRaceManager.Instance.GetUniqueID( this );
				
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
	
	public bool IsLapDone ()
	{
		return isLapDone;
	}
	
	public void RaceFinished ()
	{
		isFinished=true;
		raceRunning=true;
		
		// find out which position we finished in
		int finalRacePosition= GlobalRaceManager.Instance.GetPosition( myID );
		
		// not the cleanest solution, but it means we don't have to hardwire in the game controller script type
		// or the player control script at least. Here, we take our object and do a SendMessage call to it, to say the lap finished
		// NOTE: if you change the name of the function PlayerFinishedRace, make sure it isn't the same as any functions in any other
		// script attached to the player, or you could cause problems
		gameObject.SendMessageUpwards("PlayerFinishedRace", finalRacePosition, SendMessageOptions.DontRequireReceiver);
	}

	public void RaceStart ()
	{
		isFinished=false;
		raceRunning=false;
	}
	
	public bool IsFinished ()
    {
        return isFinished;
    }

    public int GetCurrentLap ()
    {
        return GlobalRaceManager.Instance.GetLapsDone(myID) +1;
    }
	
	public void ResetLapCounter()
	{
		GlobalRaceManager.Instance.ResetLapCount(myID);	
	}
	
    public int GetCurrentWaypointNum ()
    {
        return currentWaypointNum;
    }

    public float GetCurrentWaypointDist ()
    {
        return currentWaypointDist;
    }
	
	public bool GetIsFinished ()
	{
		return isFinished;
	}
	
	public bool GetIsLapDone ()
	{
		return isLapDone;
	}
	
	public void UpdateRaceState( bool aState )
	{
		raceRunning= aState;
	}
	
	public void SetWayController ( Waypoints_Controller aControl )
	{
		waypointManager= aControl;
	}
	
	public Transform GetWaypointTransform ()
	{
		// if we have no waypoint transform already 'in the system' then we need to grab one
		if(currentWaypointTransform==null)
		{
			currentWaypointNum=0;
			currentWaypointTransform= waypointManager.GetWaypoint(currentWaypointNum);
		}
		
		return currentWaypointTransform;
	}

	public Transform GetRespawnWaypointTransform ()
	{
		// if we are past the first waypoint, lets go back a waypoint and return that one rather than the
		// current one. That way, the car will appear roughly where it despawned rather than ahead of it.
		if(currentWaypointNum>0)
			currentWaypointNum--;
		
		currentWaypointTransform= waypointManager.GetWaypoint(currentWaypointNum);
		
		return currentWaypointTransform;
	}
	
	public void UpdateWaypoints()
	{
		if(!doneInit)
			Init();
		
		// quick check to make sure that we have a reference to the waypoint manager
		if( waypointManager==null )
			return;
		
		// because of the order that scripts run and are initialised, it is possible for this function
		// to be called before we have actually finished running the waypoints initialization, which
		// means we need to drop out to avoid doing anything silly or before it breaks the game.
		if(totalWaypoints==0)
		{
			// grab total waypoints
			totalWaypoints = waypointManager.GetTotal();
			return;
		}
	
		// here, we deal with making sure that we always have a waypoint set up and
		// if not take the steps to find out what our current waypoint should be
		if(currentWaypointTransform==null)
		{
			currentWaypointNum=0;
			currentWaypointTransform=waypointManager.GetWaypoint(currentWaypointNum);
		}
	
		// now we need to check to see if we are close enough to the current waypoint
		// to advance on to the next one
	
		myPosition = myTransform.position;
		myPosition.y=0;
	
		// get waypoint position and 'flatten' it
		nodePosition = currentWaypointTransform.position;
		nodePosition.y=0;
	
		// check distance from this car to the waypoint
	
		currentWaypointDist = Vector3.Distance(nodePosition, myPosition);
	
		if (currentWaypointDist < waypointDistance) {
			// we are close to the current node, so let's move on to the next one!
			currentWaypointNum++;	
	
			// now check to see if we have been all the way around the track and need to start again
	
			if(currentWaypointNum>=totalWaypoints){
				// completed a lap! set the lapDone flag to true, which will be checked when we go over
				// the first waypoint (so that you can't almost complete a race then go back around the
				// other way to confuse it)
				isLapDone=true;
	
				// reset our current waypoint to the first one again
				currentWaypointNum=0;
			}
			
			// grab our transform reference from the waypoint controller
			currentWaypointTransform= waypointManager.GetWaypoint(currentWaypointNum);
	
		}
	
		// position our debug box at the current waypoint so we can see it (uncomment if you're debugging!)
		// debugBox.transform.position=currentWaypointTransform.position;
	}
	
	
	public void CheckWrongWay()
	{
		if(currentWaypointTransform==null)
			return;
			
		Vector3 relativeTarget = myTransform.InverseTransformPoint (currentWaypointTransform.position);
		// Calculate the target angle for the wheels,  
		// so they point towards the target 
		targetAngle = Mathf.Atan2 (relativeTarget.x, relativeTarget.z);
		// Atan returns the angle in radians, convert to degrees 
		targetAngle *= Mathf.Rad2Deg;
		
		if(targetAngle<-90 || targetAngle>90){
			goingWrongWay=true;
		} else {
			goingWrongWay=false;
			timeWrongWayStarted=-1;
		}
		
		if(oldWrongWay!=goingWrongWay)
		{
			// store the current time
			timeWrongWayStarted= Time.time;
		}
		
		oldWrongWay=goingWrongWay;
	}
	
	public void OnTriggerEnter( Collider other )
	{
		// if the trigger we just hit is the start line trigger, we can increase our
		// lap counter when lapDone is true
		// check the name of the trigger and make sure lapDone is true
		if(	other.name=="TRIGGER_STARTLINE" && isLapDone==true)
		{
			// increase lap counter
			lapsComplete++;
			
			// reset our lapDone flag ready for when we finish the next lap
			isLapDone=false;
			
			// tell race controller we just finished a lap and which lap we are now on
			GlobalRaceManager.Instance.CompletedLap(myID);
			
			// not the cleanest solution, but it means we don't have to hardwire in the game controller script type
			// or the player control script at least. Here, we take our object and do a SendMessage call to it, to say the lap finished
			gameObject.SendMessageUpwards("LapFinished",SendMessageOptions.DontRequireReceiver);				
		}
	}
		
}
