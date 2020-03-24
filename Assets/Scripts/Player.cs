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
    protected override void Update()
    {
	    if (IsGrounded)
        {
            _movementDirection.x = Input.GetAxis("Horizontal");
            SetBoolAnim("IsRunning", _movementDirection.x>0 || _movementDirection.x<0);
        }

	    if (CanJump && Input.GetButtonDown("Jump"))
        {
            Jump();
            
        }
	    
	    base.Update();
        
    }

    private void FixedUpdate()
    {
        CalcCurrentSpeed();
        
        Move(_movementDirection);
    }
}
