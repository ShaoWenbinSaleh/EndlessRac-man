using UnityEngine;

public class GameManager
{
	private static volatile GameManager _instance;
	private static object _lock = new object();

	public int CurrentLevel { get; set;}
	public int CurrentScores { get;set;}

	public Vector2Int CurrentMazeSize { get; private set; }
	public bool IsPlayerMovable { get; set; }
	public int ScoresToPass { get; set; }
	public int CurrentBonusTime { get; set; }
	public bool IsPlayerDead { get; set; }
	public float CurrentTimeLimit { get; set; }

	private GameManager()
	{
		
	}

	public static GameManager GetInstance()
	{
		if (_instance == null)
		{
			//for thread safety
			//because it may be also visited in the thread about State Machine
			lock (_lock)
			{
				if (_instance == null)
				{
					_instance = new GameManager();
				}
			}
			return _instance;
		}

		return _instance;
	}

	public void InitGame()
	{
		CurrentMazeSize = GlobalVariables.Level1MazeSize;
		CurrentLevel = 1;
		CurrentScores = 0;
		ScoresToPass = 1000;
		IsPlayerMovable = true;
		CurrentTimeLimit = GlobalVariables.Level1TimeLimit;
	}
	
	public void NextLevel()
	{
		if (CurrentLevel < GlobalVariables.MaxLevel)
		{
			CurrentLevel++;
			CurrentScores = 0;
			CurrentMazeSize += GlobalVariables.MazeSizeIncreasement;
			CurrentTimeLimit = GlobalVariables.Level1TimeLimit + (CurrentLevel - 1) * GlobalVariables.TimeLimitIncreasement;
		}
		else
		{
			//TODO: after the max level
		}
	}

	public void GetCoin()
	{
		CurrentScores++;
	}

	public bool IsLevelPassable()
	{
		return CurrentScores >= ScoresToPass;
	}

	public int CalNumofLevelEnemy()
	{
		return GlobalVariables.Level1EnemyNum + (CurrentLevel - 1) * GlobalVariables.EnemyNumIncreasement;
	}
}
