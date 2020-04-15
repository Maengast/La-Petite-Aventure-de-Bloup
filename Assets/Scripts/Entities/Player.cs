using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
	
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    private void Update()
    {
	    // if (_gameManager.GetCurrentGameState() == GameManager.GameState.GameResult)
	    // {
		   //  return;
	    // }
	    
	    _movementDirection.x = Input.GetAxis("Horizontal");
	    if (OnGround)
        {
	        SetBoolAnim("IsRunning", _movementDirection.x>0 || _movementDirection.x<0);
	        
	        
            if (!IsJumping() && Input.GetButtonDown("Jump"))
            {
	            _movementDirection.y = 1;
	            Jump();
            }
        }

	    if (Input.GetKeyDown(KeyCode.A))
	    {
		    Die();
	    }
    }

    protected override void Die()
    {
	    _gameManager.EndGame(true);
	    base.Die();
    }
}
