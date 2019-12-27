using UnityEngine;
using System.Collections;
using Valve.VR;


public class VR_Config : MonoBehaviour
{
    public enum roomTypes { standing, sitting };
    public roomTypes theRoomType;

    void Awake()
    {
		SetUpRoom();
    }

	void SetUpRoom()
    {
        if (theRoomType == roomTypes.standing)
        {
			SteamVR_Settings.instance.trackingSpace= ETrackingUniverseOrigin.TrackingUniverseStanding;
		}
        else
        {
			SteamVR_Settings.instance.trackingSpace = ETrackingUniverseOrigin.TrackingUniverseSeated;
        }

        Recenter();
    }

    void LateUpdate()
    {
        // here, we check for a keypress to reset the view
        // whenever the mode is sitting .. don't forget to set this up in Input Manager
        if (theRoomType == roomTypes.sitting)
        {
            if (Input.GetKeyUp(KeyCode.R))
            {
                Recenter();
            }
        }
    }

    void Recenter()
    {
        
        // reset the position
        var system = OpenVR.System;
        if (system != null)
        {
            system.ResetSeatedZeroPose();
        }
    }
}
