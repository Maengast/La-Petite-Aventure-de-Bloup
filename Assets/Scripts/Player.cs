using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{

    private Vector2 Movement;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (IsGrounded)
        {
            Movement.x = Input.GetAxis("Horizontal");
            SetBoolAnim("IsRunning", Movement.x>0 || Movement.x<0);
        }
        
        Movement.y = (OnJump)? 1 : (IsGrounded)? 0 : -1;

        if (CanJump && Input.GetButtonDown("Jump"))
        {
            Jump();
            
        }
        else if (OnJump && transform.position.y >= MaxJumpHeight)
        {
            EndJump();
        }
    }

    private void FixedUpdate()
    {
        CalcCurrentSpeed();
        
        Move(Movement);
    }
}
