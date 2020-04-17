using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
	private PlayerInfo _info;
    // Start is called before the first frame update
    void Start()
    {
	    _info = _levelManager.GetPlayerInfo();
	    SetCharacterStats(_info);
        Init();
    }

    // Update is called once per frame
    protected override void Update()
    {
	    //Do nothing if not alive
	    base.Update();
	    
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

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            AttackModel attackModel = AttackInventory.GetAttackByName("CircleSaw");
            if (attackModel != null)
            {
                Attack attack = AttackFactory.GetAttack(attackModel);
                Attack(attack);
            }
        }

    }

    protected override void Die()
    {
	    _levelManager.CharacterDie(true);
	    base.Die();
    }
}
