using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataBase;

public class Testing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<Level> levels = LevelDb.GetAllLevels();
        foreach(Level l in levels){
            Debug.Log(l.Number);
        }
        Level o = LevelDb.GetLevelByNumber(12);
        Debug.Log("Récupération du niveau "+ o.Chests_Count);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
