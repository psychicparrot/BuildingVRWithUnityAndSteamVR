using UnityEngine;
using System.Collections;

[AddComponentMenu("Sample Game Glue Code/Laser Blast Survival/Enemy Bot")]

public class EnemyBot_LBS : BaseArmedEnemy 
{
	private bool isRespawning;
	
	// here we add respawning and collision to the base armed enemy script
	public void Start ()
	{
		base.Start ();
		
		// lets find our ai controller
		BaseAIController aControl= (BaseAIController) gameObject.GetComponent<BaseAIController>();

		// and tell it to chase our player around the screen (we get the player transform from game controller)
		aControl.SetChaseTarget( GameController_LBS.Instance.GetMainPlayerTransform() );
		
		// now get on and chase it!
		aControl.SetAIState( AIStates.AIState.chasing_target );
	}
	
	public void OnCollisionEnter(Collision collider) 
	{
		// when something collides with us, we check its layer to see if it is on 9 which is our projectiles
		// (Note: remember when you add projectiles to set the layer of the weapon parent correctly!)
		if( collider.gameObject.layer==9 && !isRespawning )
		{
            if (myDataManager == null)
                return;

			myDataManager.ReduceHealth(1);

			if( myDataManager.GetHealth()==0 )
			{
				tempINT= int.Parse( collider.gameObject.name );
				
				// tell game controller to make an explosion at our position and to award the player points for hitting us
				TellGCEnemyDestroyed();
				
				// if this is a boss enemy, tell the game controller when we get destroyed so it can end the level
				if( isBoss )
					TellGCBossDestroyed();
				
				// destroy this
				Destroy(gameObject);
			}
		}
	}
	
	// game controller specifics (overridden for our laser blast survival game controller)
	// ------------------------------------------------------------------------------------------
	
	public void TellGCEnemyDestroyed()
	{
		// tell the game controller we have been destroyed
		GameController_LBS.Instance.EnemyDestroyed( myTransform.position, pointsValue, tempINT );
	}
	
	public void TellGCBossDestroyed()
	{
		// tell the game controller we have been destroyed (and that we are a boss!)
		GameController_LBS.Instance.BossDestroyed();
	}
	
	// ------------------------------------------------------------------------------------------
	
}
