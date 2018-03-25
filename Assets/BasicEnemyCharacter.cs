using System;
using System.Collections;
using UnityEngine;

//There may be more kinds of enemies...here I just designed one
public class BasicEnemyCharacter : BasicCharacter
{
	//nullable feature of C#
	private Vector2 ? _movingTarget = null;

	private bool _isFollowingOrEscaping = false;
	
	public override void ResetCharacter()
	{
		IsDead = false;
		_movingTarget = null;
		gameObject.SetActive(true);
	}

	protected override void RegisterStateMachine()
	{
		//TODO: describe the enemy behavior using Machine State	
	}

	public override void CharacterMove(Vector2 playerPos)
	{
		//TODO: design advanced AI using game algorithm
		//there are many problems in this alogorithm...
		if (!_isFollowingOrEscaping)
		{
			Rb.velocity = Vector3.zero;
			var tempTarget = playerPos - new Vector2(transform.position.x, transform.position.y);
			//In this camera, the direction of x axis is inversed
			var tempMoveX = tempTarget.x;
			if (Math.Abs(tempMoveX) > 0.1f)
			{
				tempMoveX = tempTarget.x / Mathf.Abs(tempTarget.x);
			}

			var tempMoveY = tempTarget.y;
			if (Math.Abs(tempMoveY) > 0.1f)
			{
				tempMoveY = tempTarget.y / Mathf.Abs(tempTarget.y);
			}
				
			var movement = new Vector3(tempMoveX, tempMoveY, 0);
			movement *= (BasicMovingSpeed * 0.75f * 5);

			if (GameManager.GetInstance().CurrentBonusTime > 0)
			{
				//if it is in bonus time, enemy will escape from the player
				movement = -movement;
			}
			Rb.AddForce(movement);
			_isFollowingOrEscaping = true;
		}
		else
		{
			StartCoroutine("DelayMoving");
		}
		
//		
//		
//		if (_movingTarget == null)
//		{
//			var tempTarget = playerPos - new Vector2(transform.position.x, transform.position.y);
//			var tempMoveX = tempTarget.x / Mathf.Abs(tempTarget.x);
//			var tempMoveY = tempTarget.y / Mathf.Abs(tempTarget.y);
//			
//			if (Mathf.Abs(tempMoveX) > 0 && Mathf.Abs(tempMoveY) > 0)
//			{
//				
//			}
//			else if (Mathf.Abs(tempMoveX) > 0)
//			{
//				
//			}
//			else if (Mathf.Abs(tempMoveY) > 0)
//			{
//				
//			}
//			else
//			{
//				throw new Exception("Enemy" + name + " and player are colliding with errors!");
//			}
//		}
//		else
//		{
//			Rb.velocity = Vector3.zero;
//		}
	}

	//Coroutine
	private IEnumerator DelayMoving()
	{
		//wait for 2 seconds
		yield return new WaitForSeconds(4f);
		_isFollowingOrEscaping = false;
	}
	
	public override void CharacterDead()
	{
		IsDead = true;
		gameObject.SetActive(false);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag.Equals(GlobalVariables.TagPlayer))
		{
			CharacterDead();
		}
	}
}
