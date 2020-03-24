using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //Speed
    //Different Speed values
    public float XSpeed = 2.0f;
    public float FallSpeed = -2.0f;
    public float JumpSpeed = 5.0f;
    protected float YSpeed;
    protected float XCurrentSpeed = 0.0f;
    protected Vector2 LastPos;
    protected Vector2 _movementDirection;
    
    //Life
    public float MaxLife = 50;
    protected float Life = 0;

    private bool _faceRight = true;
    public bool IsGrounded = false;
    
    //Jump
    public float MaxJumpHeight = 8.0f;
    protected bool CanJump = true;
    protected bool OnJump = false;

    protected Rigidbody2D Rigidbody;
    protected Animator CharacterAnimator;
    
    /**
     * Initialise Character Instance
     * Get references of main components
     */
    protected virtual void Init()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        CharacterAnimator = GetComponent<Animator>();
        YSpeed = FallSpeed;
        LastPos = Rigidbody.position;
    }

    /**
     * Update fonction to override for all Character Child
     * Set Y movement direction to manage movement on Jump, Fall and Grounded state 
     */
    protected virtual void Update()
    {
	    _movementDirection.y = (OnJump)? 1 : (IsGrounded)? 0 : -1;
    }

    protected virtual void Move()
    {
        
    }
    
    /**
     * Main function Move
     * Move Character along a directions vector multiply by speed
     * Flip sprite in function of X movement direction
     */
    protected virtual void Move(Vector2 moveDirection)
    {
        if((moveDirection.x > 0 && !_faceRight) || (moveDirection.x < 0 && _faceRight)) Flip();
        moveDirection.x *= XSpeed;
        moveDirection.y *= YSpeed;
        Rigidbody.MovePosition(Rigidbody.position + moveDirection * Time.deltaTime);
    }
	
    /**
     * Move Character to target position
     */
    protected virtual void Move(Vector3 target)
    {
	    Vector2 direction = (target - transform.position).normalized;
	    _movementDirection.x = direction.x;
	    Move(_movementDirection);
    }
	
    /**
     * Flip Character Sprite
     */
    protected void Flip()
    {
	    if (_faceRight)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        _faceRight = !_faceRight;
    }
	
    /**
     * Start Character Jump
     */
    protected virtual void Jump()
    {
	    //Set to false to prevent an other Jump
	    CanJump = false;
	    SwitchYMovement();
	    StartCoroutine("Jumping");
	    
    }
	
    /**
     * Coroutine call when Character Jump
     * Check Character position each Frame
     * End jump when its reached maxHeightJump
     */
    IEnumerator Jumping()
    {
	    while (OnJump)
	    {
		    if (transform.position.y >= MaxJumpHeight)
		    {
			    SwitchYMovement();
		    }
		    yield return new WaitForEndOfFrame();
	    }
    }
    
    /**
     * Switch Y Movement
     * Switch Character Jump State
     * Set Y Speed in depending on if character is in Fall or Jump State
     */
    private void SwitchYMovement()
    {
	    OnJump = !OnJump;
	    YSpeed = (OnJump) ? JumpSpeed : FallSpeed;
	    SetBoolAnim("OnJump",OnJump);
    }
    
    protected void CalcCurrentSpeed()
    {
        float velocity = (Rigidbody.position.x - LastPos.x) / Time.fixedDeltaTime;
        LastPos = Rigidbody.position;
        XCurrentSpeed = (velocity<0) ? velocity * -1 : velocity;
    }

    public void SetIsGrounded(bool value)
    {
        IsGrounded = value;
        CanJump = value;
        SetBoolAnim("IsGrounded",value);
    }

    public virtual void TakeDamages(float damages)
    {
        
    }

    protected virtual void Die()
    {
        
    }
    
    //Animation
    protected void SetBoolAnim(string name, bool value)
    {
        if(CharacterAnimator) CharacterAnimator.SetBool(name,value);
    }
    
    protected void SetTriggerAnim(string name)
    {
        if(CharacterAnimator) CharacterAnimator.SetTrigger(name);
    }

    protected void SetFloatAnim(string name, float value)
    {
        if(CharacterAnimator) CharacterAnimator.SetFloat(name, value);
    }
    
    protected void SetIntAnim(string name, int value)
    {
        if(CharacterAnimator) CharacterAnimator.SetInteger(name, value);
    }
}
