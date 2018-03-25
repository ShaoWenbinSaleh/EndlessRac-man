using UnityEngine;

//factory pattern
public class CharacterFactory {
	public CharacterFactory()
	{
		
	}

	public static BasicCharacter Create(GameObject gameObject)
	{
		if (gameObject.tag.Equals(GlobalVariables.TagPlayer))
		{
			return gameObject.AddComponent<PlayerCharacter>();
		}
		else if (gameObject.tag.Equals(GlobalVariables.TagEnemy))
		{
			return gameObject.AddComponent<BasicEnemyCharacter>();
		}
		else
		{
			throw new System.NotSupportedException();
		}
	}
	
}
