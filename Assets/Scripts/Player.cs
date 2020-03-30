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
	    base.Update();
	    
	    if (IsGrounded)
        {
	        _movementDirection.x = Input.GetAxis("Horizontal");
            SetBoolAnim("IsRunning", _movementDirection.x>0 || _movementDirection.x<0);
            
            if (Input.GetButtonDown("Jump"))
            {
	            Jump();
            }
        }
        Move(_movementDirection);
        
    }

    private void FixedUpdate()
    {
        CalcCurrentSpeed();
        
        
    }
}
