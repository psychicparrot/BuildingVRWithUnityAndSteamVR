using UnityEngine;

namespace AIStates
{
	public enum AIState
	{
		moving_looking_for_target,
		chasing_target,
		backing_up_looking_for_target,
		stopped_turning_left,
		stopped_turning_right,
		paused_looking_for_target,
		translate_along_waypoint_path,
		paused_no_target,
		steer_to_waypoint,
		steer_to_target,
	}
	
}
