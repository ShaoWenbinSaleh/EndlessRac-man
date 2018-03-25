using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//static class for all global/static variables and some static methods
public static class GlobalVariables
{
	//level generation
	public static readonly int MaxLevel = 20;
	
	public static readonly Vector2Int Level1MazeSize = new Vector2Int(3, 3);
	public static readonly Vector2Int MazeSizeIncreasement = new Vector2Int(1, 1);
	
	public static readonly int Level1EnemyNum = 0;
	public static readonly int EnemyNumIncreasement = 1;

	public static readonly float Level1TimeLimit = 30.0f;
	public static readonly float TimeLimitIncreasement = 5.0f;

	public static int CalMaxEnemy()
	{
		return Level1EnemyNum + (MaxLevel - 1) * EnemyNumIncreasement;
	}

	public static readonly int Level1BonusNum = 200;
	public static readonly int BonusNumIncreasement = 5;

	public static int CalMaxObjectsOneLevel()
	{
		var maxMazeSize = Level1MazeSize + MazeSizeIncreasement * (MaxLevel - 1);
		return (maxMazeSize.x * 2 - 1) * (maxMazeSize.y * 2 - 1);
	}

	public static int CalMaxBonusOneLevel()
	{
		return Level1BonusNum + (MaxLevel - 1) * BonusNumIncreasement;
	}

	public static int CalMaxFenceCell()
	{
		var maxMazeSize = Level1MazeSize + MazeSizeIncreasement * (MaxLevel - 1);
		return (maxMazeSize.x + maxMazeSize.y) * 4;
	}

	public static readonly Vector3 WallScale = new Vector3(0.5f, 0.5f, 0.5f);
	public static readonly Vector3 CoinScale = new Vector3(0.1f, 0.1f, 0.1f);
	public static readonly Vector3 BonusScale = new Vector3(0.3f, 0.3f, 0.3f);
	public static readonly Vector3 FenceScale = new Vector3(0.5f, 0.5f, 0.5f);
	public static readonly Vector3 EnemyScale = new Vector3(0.4f, 0.4f, 0.4f);

	//tags
	public static readonly string TagPlayer = "Player";
	public static readonly string TagEnemy = "Enemy";
	public static readonly string TagWall = "Wall";
	public static readonly string TagCoins = "Coin";
	public static readonly string TagBonus = "Bonus";
	public static readonly string TagFence = "Fence";
	//TODO: consider designing some items?

	//setting
	public static readonly bool IsHorizontalMovingFirst = true;

	public static readonly Vector3 InitPlayerPos = Vector3.zero;
	public static readonly Vector3 PlayerScale = new Vector3(0.5f, 0.5f, 0.5f);
	public static readonly int BonusTotalTime = 4000;
	public static readonly float WaitingTimePassingLevel = 1000;

	public static readonly int StateFuncDeltaTime = 200;
	
	public static readonly string ShaderName = "Unlit/ToonShader";
	public static readonly Color PlayerColor = new Color(0.45f, 1f, 0f);
	public static readonly Color WallColor = new Color(0f, 0.35f, 1f);
	public static readonly Color FenceColor = new Color(0f, 0.35f, 1f);
	public static readonly Color CoinColor = new Color(1f, 0.76f, 0f);
	public static readonly Color BonusColor = new Color(1f, 0.76f, 0f);
	public static readonly Color EnemyColor = new Color(1f, 0f, 0.25f);
	public static readonly Color PlaneColor = new Color(0.51f, 0.51f, 0.51f);

}
