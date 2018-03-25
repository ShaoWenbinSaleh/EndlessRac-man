using UnityEngine;

public class PlayerCharacter : BasicCharacter
{
//	private const string BONUSTIME = "BonusTime";
	private const string ISMOVING = "IsMoving";
	private const string ISDEAD = "IsDead";

	public override void ResetCharacter()
	{
		//init player
		IsDead = false;
		gameObject.SetActive(true);
		transform.position = GlobalVariables.InitPlayerPos;
	}

	protected override void RegisterStateMachine()
	{
		State idle = CharacterStateController.AddState("Idle");
		State normalMoving = CharacterStateController.AddState("NormalMoving");
		State bonusMoving = CharacterStateController.AddState("BonusMoving");
		State dead = CharacterStateController.AddState("Dead");
		
//		CharacterStateController.RegisterParams(BONUSTIME, 0f);
		CharacterStateController.RegisterParams(ISDEAD, false);
		CharacterStateController.RegisterParams(ISMOVING, false);

		//lambda
		bonusMoving.StateFunc += () =>
		{
			GameManager.GetInstance().CurrentBonusTime -= GlobalVariables.StateFuncDeltaTime;
//			float currentBonusTime = CharacterStateController.GetParams(BONUSTIME);
//			currentBonusTime -= GlobalVariables.StateFuncDeltaTime;
//			CharacterStateController.SetParams(BONUSTIME, currentBonusTime);
		};
		
		// Idle -> NormalMoving, Dead
		idle.RegisterTranslate(normalMoving, () => CharacterStateController.GetBoolParams(ISMOVING));
		idle.RegisterTranslate(dead, () => CharacterStateController.GetBoolParams(ISDEAD));
		// NormalMoving -> BonusMoving, Idle
		normalMoving.RegisterTranslate(bonusMoving, () =>
		{
			if (GameManager.GetInstance().CurrentBonusTime <= 0) return false;
			SetBonusEffect(true);
			return true;
		});
		normalMoving.RegisterTranslate(idle, () => !CharacterStateController.GetBoolParams(ISMOVING));
		// BonusMoving -> NormalMoving, Idle
		bonusMoving.RegisterTranslate(normalMoving, () =>
		{
			if (GameManager.GetInstance().CurrentBonusTime > 0) return false;
			SetBonusEffect(false);
			return true;
		});
		
		bonusMoving.RegisterTranslate(idle, () => !CharacterStateController.GetBoolParams(ISMOVING));
		// Dead -> Idle
		//TODO: finish dead state
	}

	public override void CharacterMove(Vector2 movement)
	{

		float moveX = movement.x;
		float moveY = movement.y;
		if (Mathf.Abs(moveX) > 0 && Mathf.Abs(moveY) > 0)
		{
			if (GlobalVariables.IsHorizontalMovingFirst)
			{
				movement = new Vector3(moveX, 0, 0);
			}
			else
			{
				movement = new Vector3(0, moveY, 0);
			}
				
		}
		else if (Mathf.Abs(moveX) > 0)
		{
			movement = new Vector3(moveX, 0, 0);
		}
		else if (Mathf.Abs(moveY) > 0)
		{
			movement = new Vector3(0, moveY, 0);
		}
		else
		{
			Rb.velocity = Vector3.zero;
			CharacterStateController.SetParams(ISMOVING, false);
			return;
		}

		movement *= BasicMovingSpeed;
		Rb.AddForce(movement);
		CharacterStateController.SetParams(ISMOVING, true);
	}

	public override void CharacterDead()
	{
		IsDead = true;
		this.gameObject.SetActive(false);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag.Equals(GlobalVariables.TagEnemy))
		{
			if (!CharacterStateController.GetCurrentStateName().Equals("BonusMoving"))
			{
				CharacterStateController.SetParams(ISDEAD, true);
				CharacterStateController.SetParams(ISMOVING, false);
				CharacterDead();
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag.Equals(GlobalVariables.TagBonus))
		{
			other.gameObject.SetActive(false);
			GameManager.GetInstance().CurrentBonusTime = GlobalVariables.BonusTotalTime;
			
			
			
//			CharacterStateController.SetParams(BONUSTIME, GlobalVariables.BonusTotalTime);
		}
		else if (other.gameObject.tag.Equals(GlobalVariables.TagCoins))
		{
			other.gameObject.SetActive(false);
			GameManager.GetInstance().GetCoin();
		}
	}

	//actually this effect cannot be setted...
	private void SetBonusEffect(bool isBonusTime)
	{
		//this is wired because I don't use the unity Animator system
		//because this function of State Machine is not running in the main thread,
		//many properties cannot be visited regarding the player's looking
		if (isBonusTime)
		{
			Debug.Log("bonus start");
			//bonus time starts
//			gameObject.transform.localScale = GlobalVariables.PlayerScale * 1.25f;
//			CharMat.SetColor("_OutlineColor", Color.white);
		}
		else
		{
			Debug.Log("bonus end");
			//bonus time ends
//			gameObject.transform.localScale = GlobalVariables.PlayerScale;
//			CharMat.SetColor("_OutlineColor", Color.black);
		}
	}
}
