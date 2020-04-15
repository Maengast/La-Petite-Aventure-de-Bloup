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
    // Start is called before the first frame update
    void Start()
    {
        InitDatabase();


    }

    void InitDatabase()
    {
        path = Application.streamingAssetsPath + "/Character_init.json";
        jsonString = File.ReadAllText(path);


        var jObject = JObject.Parse(jsonString);

        // Add Boss
        List<BossInfo> allBoss = BossDb.GetAllBoss();

        if (allBoss.Count == 0)
        {
            BossInfo[] bossInfos = jObject["Boss"].ToObject<BossInfo[]>();

            foreach (BossInfo boss in bossInfos)
            {
                BossDb.AddBoss(boss);
            }
        }

        // Add player
        List<PlayerInfo> playerInfos = PlayerDb.GetPlayers();
        if (playerInfos.Count == 0)
        {
            PlayerInfo player = jObject["Player"].ToObject<PlayerInfo>();
            Debug.Log(player.Name);
            PlayerDb.AddPlayer(player);
        }

    }

    public Level GetLevelInfo(int levelNumber)
    {
	    Level level = new Level();
	    level.Number = levelNumber;
	    level.Height = 20;
	    level.Width = 100;
	    level.TrapsCount = 5;
	    level.BonusCount = 3;
	    return level;
    }
}
