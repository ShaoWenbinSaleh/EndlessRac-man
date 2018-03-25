using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    private List<GameObject> _wallsCache;
    private List<GameObject> _coinsCache;
    private List<GameObject> _bonusCache;
    //the fence of the map
    private List<GameObject> _fenceCache;
    
    //there WILL be more kinds of enemy extending from the BasicEnemyCharacter...
    private List<BasicEnemyCharacter> _enemyCache;
    
    private PlayerCharacter _player;

    private bool _isLevelGenerating = false;
    // Use this for initialization
    private void Awake()
    {
        //init camera
        transform.position = new Vector3(0, 0, -5);
//        transform.RotateAround(Vector3.zero, Vector3.left, 10);
//        Camera camera = GetComponent<Camera>();
        
        //player
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
        go.tag = GlobalVariables.TagPlayer;
        SetGameObjectColor(go, GlobalVariables.PlayerColor);
                
        _player = (PlayerCharacter) CharacterFactory.Create(go);
    }

    private void Start()
    {
        Renderer rend = _player.GetComponent<Renderer>();
        rend.material = new Material(Shader.Find(GlobalVariables.ShaderName));
        rend.material.SetColor("_Color", GlobalVariables.PlayerColor);
        
        _wallsCache = new List<GameObject>();
        _coinsCache = new List<GameObject>();
        _bonusCache = new List<GameObject>();
        _fenceCache = new List<GameObject>();
        _enemyCache = new List<BasicEnemyCharacter>();
        
        GameManager.GetInstance().InitGame();
        
        //TODO:finish the background plane
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.RotateAround(Vector3.zero, Vector3.left, 90f);
        plane.transform.localPosition = new Vector3(0, 0, 0.26f);
        plane.transform.localScale = new Vector3(1000, 1000, 1000);
        //plane color???
                
        CreateGameObjectCaches();
        GenerateNewLevel();
    }

    // main game process controller
    private void FixedUpdate()
    {
        if (GameManager.GetInstance().CurrentTimeLimit <= 0)
        {
            _player.CharacterDead();
            return;
        }

        GameManager.GetInstance().CurrentTimeLimit -= Time.deltaTime;
        
        if (_player.IsDead)
        {
            //TODO: the situation if the player dead
            return;
        }
        
        
        Vector2 inputMovement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (GameManager.GetInstance().IsLevelPassable())
        {
            //stop the player
            //inputMovement = new Vector2(0, 0);
            if (!_isLevelGenerating)
            {
                _isLevelGenerating = true;
                StartCoroutine("WaitAndPassThisLevel");
            }
        }
        
        //BUG: sometimes _player.transform.position may be > 0 ??? why?
        //So I have to add this line every frame to keep the player's position be zero
        _player.transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y, 0);
        
        //observer pattern
        _player.CharacterMove(inputMovement);
        //camera following
        transform.position = new Vector3(_player.transform.position.x, _player.transform.position
            .y, transform.position.z);

        foreach (var t in _enemyCache)
        {
            if (!t.IsDead)
            {
                t.CharacterMove(new Vector2(_player.transform.position.x, _player.transform.position.y));
            }
        }
    }

    //Coroutine
    IEnumerator WaitAndPassThisLevel()
    {
        //wait for 0.5 seconds
        yield return new WaitForSeconds(0.5f);
        GameManager.GetInstance().NextLevel();
        GenerateNewLevel();
        _isLevelGenerating = false;
    }

    private void SetGameObjectColor(GameObject go, Color color)
    {
        Renderer rend = go.GetComponent<Renderer>();
        rend.material = new Material(Shader.Find(GlobalVariables.ShaderName));
        rend.material.SetColor("_Color", color);
    }

    private void CreateGameObjectCaches()
    {
        GameObject wallObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        SetGameObjectColor(wallObject, GlobalVariables.WallColor);
        GameObject coinObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        SetGameObjectColor(coinObject, GlobalVariables.CoinColor);
        coinObject.GetComponent<Collider>().isTrigger = true;
        GameObject bonusObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        SetGameObjectColor(bonusObject, GlobalVariables.BonusColor);
        bonusObject.GetComponent<Collider>().isTrigger = true;
        GameObject fenceObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        SetGameObjectColor(fenceObject, GlobalVariables.FenceColor);
        GameObject enemyObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        SetGameObjectColor(enemyObject, GlobalVariables.EnemyColor);
        //performance optimization: create all needed objects once and set as caches
        for (int i = 0; i < GlobalVariables.CalMaxObjectsOneLevel(); i++)
        {
            var wall = Instantiate(wallObject);
            wall.transform.localScale = GlobalVariables.WallScale;
            wall.tag = GlobalVariables.TagWall;
            wall.SetActive(false);
            _wallsCache.Add(wall);

            var coin = Instantiate(coinObject);
            coin.transform.localScale = GlobalVariables.CoinScale;
            coin.tag = GlobalVariables.TagCoins;
            coin.SetActive(false);
            _coinsCache.Add(coin);
        }

        for (int i = 0; i < GlobalVariables.CalMaxBonusOneLevel(); i++)
        {
            var bonus = Instantiate(bonusObject);
            bonus.transform.localScale = GlobalVariables.BonusScale;
            bonus.tag = GlobalVariables.TagBonus;
            bonus.SetActive(false);
            _bonusCache.Add(bonus);
        }

        for (int i = 0; i < GlobalVariables.CalMaxFenceCell(); i++)
        {
            var fence = Instantiate(fenceObject);
            fence.transform.localScale = GlobalVariables.FenceScale;
            fence.tag = GlobalVariables.TagFence;
            fence.SetActive(false);
            _fenceCache.Add(fence);
        }

        for (int i = 0; i < GlobalVariables.CalMaxEnemy(); i++)
        {
            //TODO: create various enemies based on the number of level
            var enemyInstansiate = Instantiate(enemyObject);
            enemyInstansiate.tag = GlobalVariables.TagEnemy;
            enemyInstansiate.transform.localScale = GlobalVariables.EnemyScale;
            enemyInstansiate.SetActive(false);
            BasicEnemyCharacter enemyCharacter = (BasicEnemyCharacter)CharacterFactory.Create(enemyInstansiate);
            enemyCharacter.ResetCharacter();
            enemyCharacter.CharacterDead();
            _enemyCache.Add(enemyCharacter);
        }
        
        Destroy(wallObject);
        Destroy(coinObject);
        Destroy(bonusObject);
        Destroy(fenceObject);
        Destroy(enemyObject);
    }

    private void GenerateNewLevel()
    {
        
        ClearLevel();
        LevelGenerator.GenerateNewLevel(Vector2Int.zero, _wallsCache, _coinsCache, _bonusCache, _fenceCache, _enemyCache);
        _player.ResetCharacter();
    }

    private void ClearLevel()
    {
        foreach (var obj in _wallsCache)
        {
            obj.SetActive(false);
        }

        foreach (var obj in _coinsCache)
        {
            obj.SetActive(false);
        }

        foreach (var obj in _bonusCache)
        {
            obj.SetActive(false);
        }

        foreach (var obj in _enemyCache)
        {
            obj.CharacterDead();
        }
    }

    //TODO: make better UI
    private void OnGUI()
    {
        GUI.Label(new Rect(600, 300, 200, 40), "LEVEL " + GameManager.GetInstance().CurrentLevel);
        GUI.Label(new Rect(600, 350, 200, 40), "SCORES " + GameManager.GetInstance().CurrentScores + "/" + GameManager.GetInstance().ScoresToPass);
        GUI.Label(new Rect(600, 400, 200, 40), "TIME " + GameManager.GetInstance().CurrentTimeLimit);
    }
}