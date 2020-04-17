using System;
using System.Collections;
using System.Collections.Generic;
using DataBase;
using UnityEngine;
using Grid = PathFinder.Grid;

public class LevelManager : MonoBehaviour
{
	//Static field
	public static readonly int BossSize = 2;
	public static readonly int PlayerSize = 1;
	
	[Header("Prefab")]
	public GameObject PlayerPrefab;
	public GameObject PathFinding;

	[Header("Camera")] 
	public GameObject FollowingCamera;
	
    private int _levelNumber;
    private Level _levelInfos;
    
    //Singleton
    private GameManager _gameManager;
    private DataManager _dataManager;
    
    //PlayerInfo
    private PlayerInfo _playerInfos;
    private BossInfo _bossInfo;
    
    void Start()
    {
	    _dataManager = DataManager.Instance;
	    _gameManager = GameManager.Instance;

	    //get all info needed
	    _levelNumber = /*_gameManager.GetCurrentLevel();*/ 1;
	    _levelInfos = _dataManager.GetLevelInfo(_levelNumber);
	    _playerInfos = _dataManager.GetPlayerInfo();
	    _bossInfo = _dataManager.GetBossInfo(_levelInfos.BossName);
	    //Same jump height
	    _bossInfo.JumpHeight = _playerInfos.JumpHeight;
	    //Set Max jump distance rounded to smaller integer
	    _levelInfos.MaxJumpDistance = new Coordinate(Mathf.FloorToInt(_playerInfos.Speed),Mathf.FloorToInt(_playerInfos.JumpHeight));
	    
	    CreateLevel(); //Create Level

    }
	
    /**
     * Create the level
     */
    private void CreateLevel()
    {
	    LevelGenerator levelGenerator = gameObject.GetComponent<LevelGenerator>();
	    levelGenerator.GenerateLevel(_levelInfos, OnLevelGenerated);
    }
    
    /**
     * Callback call when level is generated complete or incomplete
     * When its incomplete GameManager reload the level
     */
    private void OnLevelGenerated(TileObject endTile, TileObject startTile, bool levelComplete)
    {
	    if (!levelComplete)
	    {
		    StopAllCoroutines();
		    _gameManager.LoadLevel(_levelNumber);
		    return;
	    }
	    
	    //Create grid and path finding for IA
	    CreateIaPathfinding();
	    //instantiate player
	    GameObject player = InstantiateCharacter(PlayerPrefab,startTile);
	    //instantiate Boss
		InstantiateCharacter(Resources.Load(_levelInfos.BossName) as GameObject, endTile);
	    
	    //Setup Camera
	    FollowingCamera.AddComponent<FollowingCamera>();
	    FollowingCamera.GetComponent<FollowingCamera>().Init(player,_levelInfos.Height,_levelInfos.Width);
    }
	
    /**
     * Instantiate Object to create a grid and pathfinding for IA
     */
    private void CreateIaPathfinding()
    {
	    GameObject pathfinding = Instantiate(PathFinding);
	    pathfinding.transform.position = Vector3.zero;
	    //create grid Area
	    Area gridArea = new Area();
	    gridArea.Origin = new Coordinate(0,0);
	    gridArea.Size = new Coordinate(_levelInfos.Width, _levelInfos.Height);
	    //init grid
	    //Take max jump distance to setup path finding
	    pathfinding.GetComponent<Grid>().InitGrid(gridArea,BossSize,Mathf.Max(_levelInfos.MaxJumpDistance.x,_levelInfos.MaxJumpDistance.x));
    }
    
    /**
     * Instantiate Boss and Player function
     */
    private GameObject InstantiateCharacter(GameObject characterPrefab,TileObject tile)
    {
	    Vector2 position = new Vector2(tile.Origin.x + tile.Size.x / 2, tile.Origin.y + tile.Size.y);
	    GameObject character = Instantiate(characterPrefab, position, Quaternion.identity);
	    character.GetComponent<Character>().SetLevelManager(this);
	    return character;
    }

    /**
     * Inform GameManager EndGame
     * Game over if playerDied
     */
    public void CharacterDie(bool isPlayer)
    {
	    _gameManager.EndGame(isPlayer);
    }
    /**
     * Return character info for pathfinding and each Character Instance
     */
    public BossInfo GetBossInfo()
    {
	    return _bossInfo;
    }

    public PlayerInfo GetPlayerInfo()
    {
	    return _playerInfos;
    }
}
