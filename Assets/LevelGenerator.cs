using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;
//main algorithm: to generate the level especially for the maze map
public class LevelGenerator
{
    private static readonly List<Vector2> SurrondVectorsList =
        new List<Vector2> {Vector2.down / 2.0f, Vector2.left / 2.0f, Vector2.up / 2.0f, Vector2.right / 2.0f};

    //using List to store Vector2, the position (x,y) of the cell or road
    private static List<Vector2> _generatedCells = new List<Vector2>();
    private static List<Vector2> _unsearchedRoads = new List<Vector2>();
    private static List<Vector2> _specialRoad = new List<Vector2>();

    public static void GenerateNewLevel(Vector2Int startPos, List<GameObject> wallCaches, List<GameObject> coinCahces, List<GameObject> bonusCaches, List<GameObject> fenceCaches, List<BasicEnemyCharacter> enemyCahces)
    {
        var currentMazeSize = GameManager.GetInstance().CurrentMazeSize;
        GenerateMaze(currentMazeSize.x, currentMazeSize.y, startPos);

        var wallIndex = 0;
        var coinIndex = 0;
        var bonusIndex = 0;
        var enemyIndex = 0;
        var fenceIndex = 0;
        
        //store the position of cells as a List<Vector2>, traverse all possible positions and calculate what the situation of this position should be
        //trick: compare to the ordinary method which storing the situation of each point in a 2D array, this method is more elegant and efficient
        //3: -0.5, 1, 1.5, 2, 2.5, 3, 3.5
        for (float i = -0.5f; i <= currentMazeSize.x + 0.5f; i += 0.5f)
        {
            for (float j = -0.5f; j <= currentMazeSize.y + 0.5f; j += 0.5f)
            {
                var tempVector = new Vector2(i, j);
                
                if (_generatedCells.Contains(tempVector))
                {
                    try
                    {
                        coinCahces[coinIndex].SetActive(true);
                        coinCahces[coinIndex].transform.position = tempVector;
                        coinIndex++;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw new Exception("more coins generated!");
                    }                    
                }
                else if (_specialRoad.Contains(tempVector))
                {
                    //TODO: finish the generation algorithm about bonus coins
                    try
                    {
                        bonusCaches[bonusIndex].SetActive(true);
                        bonusCaches[bonusIndex].transform.position = tempVector;
                        bonusIndex++;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw new Exception("more bonus generated!");
                    }
                }
                else if ( i<0 || i>currentMazeSize.x || j<0 || j>currentMazeSize.y )
                { 
                    //TODO: redesign the fence
                    //"fence" means the outline of the maze map
                    // for example, if the 
                    try
                    {
                        fenceCaches[fenceIndex].SetActive(true);
                        fenceCaches[fenceIndex].transform.position = tempVector;
                        fenceIndex++;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw new Exception("more fence cells generated!");
                    }
                }
                else
                {
                    try
                    {
                        wallCaches[wallIndex].SetActive(true);
                        wallCaches[wallIndex].transform.position = tempVector;
                        wallIndex++;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw new Exception("more walls generated!");
                    }
                }
            }
            
            //set the ScoresToPass
            GameManager.GetInstance().ScoresToPass = coinIndex - 1;
        }
        
        //TODO: combine all the walls to one gameobject

        //generate enemies
        //TODO:generate various enemeies
        List<Vector2> possibleEnemyPos = new List<Vector2>();
        //merge the two lists to calculate the init positions of enemies
        possibleEnemyPos = _generatedCells.Union(_specialRoad).ToList<Vector2>();
        //TODO: improve this random generation algorithm
        if (possibleEnemyPos.Contains(GlobalVariables.InitPlayerPos))
        {
            possibleEnemyPos.Remove(GlobalVariables.InitPlayerPos);
        }
        //TODO: exclude the positions too close to the player
        //set the position of the enemies randomly
        for (int i = 0; i < GameManager.GetInstance().CalNumofLevelEnemy(); i++)
        {
            enemyCahces[i].ResetCharacter();
            int randomnum = Random.Range(0, possibleEnemyPos.Count);
            Vector2 tempPos = possibleEnemyPos[randomnum];
            enemyCahces[i].gameObject.transform.position = new Vector3(tempPos.x, tempPos.y, 0);
            possibleEnemyPos.Remove(tempPos);
        }
    }


    private static bool IsCellInRange(Vector2 cell, int width, int height)
    {
        if (cell.x < 0 || cell.x >= width || cell.y < 0 || cell.y >= height)
        {
            return false;
        }

        return true;
    }

    private static List<Vector2> AddNeighborUnsearchedRoads(List<Vector2> walls, Vector2Int cellPos, int width, int height)
    {
        foreach (var t in SurrondVectorsList)
        {
            var newWall = cellPos + t;
            if (IsCellInRange(newWall, width, height) && !walls.Contains(newWall))
            {
                walls.Add(newWall);
            }
        }

        return walls;
    }

    private static bool IsCellDeadEnd(Vector2 cell, int width, int height)
    {
        int numofNeighboringCells = 0;
        foreach (var t in SurrondVectorsList)
        {
            var neighboringCell = cell + t;
            if (IsCellInRange(cell, width, height) &&
                (_generatedCells.Contains(neighboringCell) || _specialRoad.Contains(neighboringCell)))
            {
                numofNeighboringCells++;
            }
        }

        return numofNeighboringCells <= 1;
    }

    private static void GenerateMaze(int width, int height, Vector2Int startPos)
    {
        //there may be one road between two cells, the "width" and "height" describe the num of cells
        // for example, the situation width=3 and height=3 will be:
        // *-*-*
        // | | |
        // *-*-*
        // | | |
        // *-*-*
        // where "*" means cell, "-" and "|" mean road
        // if the road is passable, it will become a cell otherwise it will become a "wall"
        
        _generatedCells.Clear();
        _unsearchedRoads.Clear();
        _specialRoad.Clear();

        //width and height must be odd and >= 3
        _generatedCells.Add(startPos);

        //Prim algorithm:1. store the 4 neighbooring roads of one selected cell as a list
        _unsearchedRoads = AddNeighborUnsearchedRoads(_unsearchedRoads, startPos, width, height);

        //2. select one road from the mentioned list roadomly
        while (_unsearchedRoads.Count > 0)
        {
            int randomNum = Random.Range(0, _unsearchedRoads.Count - 1);
            Vector2 currentWall = _unsearchedRoads[randomNum];

            var floorx = Mathf.FloorToInt(currentWall.x);
            var ceilx = Mathf.CeilToInt(currentWall.x);
            var floory = Mathf.FloorToInt(currentWall.y);
            var ceily = Mathf.CeilToInt(currentWall.y);
            Vector2Int connectedCell0;
            Vector2Int connectedCell1;
            if (floorx == ceilx)
            {
                connectedCell0 = new Vector2Int(floorx, floory);
                connectedCell1 = new Vector2Int(floorx, ceily);
            }
            else
            {
                connectedCell0 = new Vector2Int(floorx, floory);
                connectedCell1 = new Vector2Int(ceilx, floory);
            }
            //3. if one of the neighbooring cells of this road is "connected", mark this road as "passable" meaning make it become one cell
            if (_generatedCells.Contains(connectedCell0) && _generatedCells.Contains(connectedCell1))
            {
                _unsearchedRoads.Remove(currentWall);
                //4. in case of dead-end roads
                if (IsCellDeadEnd(connectedCell0, width, height) || IsCellDeadEnd(connectedCell1, width, height))
                {
                    _specialRoad.Add(currentWall);
                }
            }
            else if (_generatedCells.Contains(connectedCell0))
            {
                _generatedCells.Add(connectedCell1);
                _unsearchedRoads = AddNeighborUnsearchedRoads(_unsearchedRoads, connectedCell1, width, height);
                _unsearchedRoads.Remove(currentWall);
                _generatedCells.Add(currentWall);
            }
            else if (_generatedCells.Contains(connectedCell1))
            {
                _generatedCells.Add(connectedCell0);
                _unsearchedRoads = AddNeighborUnsearchedRoads(_unsearchedRoads, connectedCell0, width, height);
                _unsearchedRoads.Remove(currentWall);
                _generatedCells.Add(currentWall);
            }
            else
            {
                
            }
        }
    }
}