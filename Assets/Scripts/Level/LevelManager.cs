using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
	public GameObject PlayerPrefab;
	public float LoadingWaitTime = 2.0f;
    private bool _isLoad = false;
    private int _levelNumber;
    
    void Start()
    {
	    StartCoroutine("LoadingLevel");
	    _levelNumber = /*GameManager.Instance.GetCurrentLevel();*/ 1;
	    CreateLevel();
	    //find player spawn
	    //End load game
	    //instantiate player
	    //Instantiate boss
    }

    private void CreateLevel()
    {
	    LevelGenerator levelGenerator = gameObject.GetComponent<LevelGenerator>();
	    levelGenerator.GenerateLevel(_levelNumber, OnLevelGenerated);
    }
    public void OnLevelGenerated(TileObject _bossTile, TileObject _playerTile, bool levelComplete)
    {
	    if (!levelComplete)
	    {
		    StopAllCoroutines();
		    GameManager.Instance.LoadLevel(_levelNumber);
	    }
	    _isLoad = true;
    }
    private void InstantiateCharacter(GameObject characterPrefab,Vector2 position)
    {
	    GameObject character = Instantiate(characterPrefab, position, Quaternion.identity);
	    character.GetComponent<Character>().SetLevelManager(this);
    }
    
    IEnumerator LoadingLevel()
    {
	    while (!_isLoad)
	    {
		    yield return new WaitForSecondsRealtime(LoadingWaitTime);
	    }
	    if(GameManager.Instance)GameManager.Instance.EndLoadLevel();
    }
    
    
}
