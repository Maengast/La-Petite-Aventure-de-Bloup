using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class UILevel : UIComponent
{
	public Animator UILevelAnimator;
	public int LevelNumber;
	private GameManager.LevelState _levelState;


	private void Start()
	{
		if(!UILevelAnimator)UILevelAnimator = GetComponent<Animator>();
	}

	public override void Init()
    {
	    _levelState = GameManager.Instance.GetLevelState(LevelNumber);

	    if (_levelState != GameManager.LevelState.Close)
	    {
		    UILevelAnimator.SetBool("Wait",false);
		    UILevelAnimator.SetBool(_levelState.ToString(), true);
	    }
    }
    
    public void PlayLevel()
    {
	    GameManager.Instance.LoadLevel(LevelNumber);
    }

    public override void Disable()
    {
	    UILevelAnimator.SetBool("Wait",true);
    }
}
