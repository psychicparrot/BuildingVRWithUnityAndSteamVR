using UnityEngine;
using System.Collections;

public class GlobalRaceManager : ScriptableObject
{
	public int totalLaps;
	private int currentID;
	
	private Hashtable raceControllers;
	private Hashtable racePositions;
	private Hashtable raceLaps;
	private Hashtable raceFinished;
	
	private int numberOfRacers;
	
	private int myPos;
	private bool isAhead;
	private RaceController tempRC;
	private RaceController focusPlayerScript;
	private bool raceRunning;
	
	private static GlobalRaceManager instance;
 
	public static GlobalRaceManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance =  ScriptableObject.CreateInstance <GlobalRaceManager>();
			}
			return instance;
		}
	}
 
	public void OnApplicationQuit ()
	{
		instance = null;
	}
	
	public void InitNewRace( int howManyLaps )
	{
		// initialise our hashtables ready for putting objects into
		racePositions= new Hashtable();
		raceLaps= new Hashtable();
		raceFinished= new Hashtable();
		raceControllers= new Hashtable();
		
		totalLaps= howManyLaps;
	}
	
	public int GetUniqueID(RaceController theRaceController)
	{
		// whenever an id is requested, we increment the ID counter. this value never gets reset, so it should always
		// return a unique id (NOTE: these are unique to each race)
		currentID++;
		
		// now set up some default data for this new player
		
		// this player will be on its first lap
		raceLaps.Add(currentID, 1);
		
		// this player will be in last position
		racePositions.Add(currentID, racePositions.Count + 1);
		
		// store a reference to the race controller, to talk to later
		raceControllers.Add ( currentID, theRaceController );
		
		// default finished state
		raceFinished[currentID]=false;
		
		// increment our racer counter so that we don't have to do any counting or lookup whenever we need it
		numberOfRacers++;
		
		// pass this id back out for the race controller to use
		return currentID;
	}
	

	
	public int GetRacePosition(int anID)
	{
		// just returns the entry for this ID in the racePositions hashtable
		return (int)racePositions[anID];
	}
	
	public int GetLapsDone(int anID)
	{
		// just returns the entry for this ID in the raceLaps hashtable
		return (int)raceLaps[anID];
	}
	
	public void CompletedLap(int anID)
	{
		// if should already have an entry in race laps, let's just increment it
		raceLaps[anID]= (int)raceLaps[anID] + 1;
		
		// here, we check to see if this player has finished the race or not (by checking its entry in
		// raceLaps against our totalLaps var) and if it has finished, we set its entry in raceFinished hashtable
		// to true. note that we always have to declare the object's type when we get it from the hashtable, since
		// hashtables store objects of any type and the system doesn't know what they are unless we tell it!
		if((int)raceLaps[anID]==totalLaps)
		{
			raceFinished[anID]= true;
			// tell the race controller for this ID that it is finished racing
			tempRC = (RaceController) raceControllers [anID];
			tempRC.RaceFinished();
		}
	}
	
	public void ResetLapCount(int anID)
	{
		// if there's ever a need to restart the race and reset laps for this player, we reset its entry
		// in the raceLaps hashtable here
		raceLaps[anID]=0;	
	}
			
	public int GetPosition ( int ofWhichID )
    {
		// first, let's make sure that we are ready to go (the hashtables may not have been set up yet, so it's
		// best to be safe and check this first)
		if(raceControllers==null)
		{
			Debug.Log ("GetPosition raceControllers is NULL!");
			return -1;
		}
		
		if(raceControllers.ContainsKey(ofWhichID)==false)
		{
			Debug.Log ("GetPosition says no race controller found for id "+ofWhichID);
			return -1;
		}
		
		// first, we need to find the player that we're trying to calculate the position of
		focusPlayerScript= (RaceController) raceControllers[ofWhichID];
		
		// start with the assumption that the player is in last place and work up
        myPos = numberOfRacers;

        // now we step through each racer and check their positions to determine whether or not
        // our focussed player is in front of them or not
        for ( int b = 1; b <= numberOfRacers; b++ )
        {
            // assume that we are behind this player
            isAhead = false;
            
            // grab a temporary reference to the 'other' player we want to check against
            tempRC = (RaceController) raceControllers [b];
			
			// if car 2 happens to be null (deleted for example) here's a little safety to skip this iteration in the loop
			if(tempRC==null)
				continue;
				
            if ( focusPlayerScript.GetID() != tempRC.GetID() )
            { // <-- make sure we're not trying to compare same objects!

                // is the focussed player a lap ahead?
                if ( focusPlayerScript.GetCurrentLap() > tempRC.GetCurrentLap() )
                    isAhead = true;

                // is the focussed player on the same lap, but at a higher waypoint number?
                if ( focusPlayerScript.GetCurrentLap() == tempRC.GetCurrentLap() && focusPlayerScript.GetCurrentWaypointNum() > tempRC.GetCurrentWaypointNum() && !tempRC.IsLapDone() )
                    isAhead = true;

                // is the focussed player on the same lap, same waypoint, but closer to it?
                if ( focusPlayerScript.GetCurrentLap() == tempRC.GetCurrentLap() && focusPlayerScript.GetCurrentWaypointNum() == tempRC.GetCurrentWaypointNum() && focusPlayerScript.GetCurrentWaypointDist() < tempRC.GetCurrentWaypointDist() && !tempRC.IsLapDone() )
                    isAhead = true;

                // has the player completed a lap and is getting ready to move onto the next one?
                if ( focusPlayerScript.GetCurrentLap() == tempRC.GetCurrentLap() && focusPlayerScript.GetCurrentWaypointNum() == tempRC.GetCurrentWaypointNum() && ( focusPlayerScript.IsLapDone() == true && tempRC.IsLapDone() == false ) )
                    isAhead = true;

                if ( focusPlayerScript.GetCurrentLap() == tempRC.GetCurrentLap() && ( focusPlayerScript.IsLapDone() == true && !tempRC.IsLapDone() ) )
                    isAhead = true;

                if ( isAhead )
                {
                    myPos--;
                }
            }

        }
		
		return myPos;
    }
	
	public void StartRace()
	{
		raceRunning=true;
		
		UpdateRacersRaceState();
	}
	
	public void StopRace()
	{
		raceRunning=false;
		UpdateRacersRaceState();
	}
	
	void UpdateRacersRaceState()
	{
		for ( int b = 1; b <= numberOfRacers; b++ )
        {
			tempRC = (RaceController) raceControllers [b];
			tempRC.UpdateRaceState(raceRunning);
		}
	}
}
