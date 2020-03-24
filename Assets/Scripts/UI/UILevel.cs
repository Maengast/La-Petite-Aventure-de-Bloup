using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class UILevel : MonoBehaviour
{
	public Animator UILevelAnimator;
	public int LevelNumber;
	
	// Start is called before the first frame update
    void Start()
    {
	    //UILevelAnimator = GetComponent<Animator>();
	    GameManager.LevelState levelState = GameManager.Instance.GetLevelState(LevelNumber);

	    if (levelState != GameManager.LevelState.Close)
	    {
		    UILevelAnimator.enabled = true;
	    }
	    
	    if (levelState == GameManager.LevelState.New)
	    {
		    UILevelAnimator.SetBool("new",true);
	    }
    }
    

    public void PlayLevel()
    {
	    GameManager.Instance.LoadLevel(LevelNumber);
    }
}
