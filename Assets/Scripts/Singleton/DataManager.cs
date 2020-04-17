using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using DataBase;

public class DataManager : Singleton<DataManager>
{
    string path;
    string jsonString;

    private PlayerInfo _player;
    
    // Start is called before the first frame update
    void Start()
    {
	    InitDatabase();
	    _player = PlayerDb.GetPlayer();
    }

    void InitDatabase()
    {
	    InitCharacters();
	    InitLevels();
    }

    private void InitCharacters()
    {
	    path = Application.streamingAssetsPath + "/Character_init.json";
	    jsonString = File.ReadAllText(path);


	    var jObject = JObject.Parse(jsonString);

	    // Add Boss
	    List<BossInfo> allBoss = BossDb.GetAllBoss();
	    if (allBoss.Count < 1)
	    {
		    BossInfo[] bossInfos = jObject["Boss"].ToObject<BossInfo[]>();

		    foreach (BossInfo boss in bossInfos)
		    {
			    BossDb.AddBoss(boss);
		    }
	    }

	    // Add player
	    PlayerInfo playerInfos = PlayerDb.GetPlayer();
	    if (playerInfos != null) return;
	    PlayerInfo player = jObject["Player"].ToObject<PlayerInfo>();
	    PlayerDb.AddPlayer(player);
    }

    private void InitLevels()
    {
	    path = Application.streamingAssetsPath + "/Level_Init.json";
	    jsonString = File.ReadAllText(path);
	    var jObject = JObject.Parse(jsonString);
	    
	    List<Level> allLevel = LevelDb.GetAllLevels();
	    
	    if (allLevel.Count > 0) return;
	    Level[] levels = jObject["Levels"].ToObject<Level[]>();

	    foreach (Level level in levels)
	    {
		    LevelDb.AddLevel(level);
	    }
    }
    
    public BossInfo GetBossInfo(string bossName)
    {
	    return BossDb.GetBossByName(bossName);
    }

    public PlayerInfo GetPlayerInfo()
    {
	    return _player;
    }

    public Level GetLevelInfo(int levelNumber)
    {
	    Level level = LevelDb.GetLevelByNumber(levelNumber);
	    return level;
    }

    public int GetLevelCount()
    {
	    return LevelDb.GetAllLevels().Count;
    }
}
