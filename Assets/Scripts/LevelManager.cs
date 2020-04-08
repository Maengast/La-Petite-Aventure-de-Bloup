using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
	public GameObject PlayerPrefab;
	
    public float LoadingWaitTime = 2.0f;
    private bool _isLoad = false;
    
    void Start()
    {
	    StartCoroutine("LoadingLevel");
	    //Create level
	    //find player spawn
	    //End load game
	    //instantiate player
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
	    GameManager.Instance.EndLoadLevel();
    }
    
    
}
