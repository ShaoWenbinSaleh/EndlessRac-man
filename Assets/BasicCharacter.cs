using UnityEngine;

public abstract class BasicCharacter : MonoBehaviour
{
	//basic of speed, enemy is the 75% of it
	protected float BasicMovingSpeed = 10;
	//rigid body, to move the charater
	protected Rigidbody Rb;
	//material, to change its looking
	protected Material CharMat;
	
	protected bool IsMoving = false;
	public bool IsDead { set; get; }
	
	protected StateController CharacterStateController;
	// Use this for initialization
	private void Awake()
	{
		Rb = gameObject.AddComponent<Rigidbody>();
		Rb.useGravity = false;
		CharMat = GetComponent<Renderer>().material;
		CharacterStateController = new StateController(gameObject.tag);
		RegisterStateMachine();
		IsDead = true;
	}

	protected abstract void RegisterStateMachine();
	public abstract void CharacterMove(Vector2 refVecter);
	public abstract void CharacterDead();
	public abstract void ResetCharacter();
}
