using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(CharacterCollisions))]

public class Character : MonoBehaviour
{
    //Speed
    //Different Speed values
    public float Speed = 8.0f;
    public float Gravity = 9.5f;
    public float MaxFallSpeed = 15.0f;
    
    protected Vector2 _movementDirection;
    private float currentYSpeed;
    
    //Life
    public float MaxLife = 50;
    protected float Life = 0;

    private bool _faceRight = true;
    public bool OnGround = false;
    
    //Jump
    public float JumpHeight = 5.0f;
    protected bool CanJump = true;
    protected bool InJump = false;

    protected Rigidbody2D Rigidbody;
    protected Animator CharacterAnimator;

    protected GameManager _gameManager;
    protected LevelManager _levelManager;
    
    /**
     * Initialise Character Instance
     * Get references of main components
     */
    protected virtual void Init()
    {
        if(!Rigidbody)Rigidbody = GetComponent<Rigidbody2D>();
        if(!CharacterAnimator)CharacterAnimator = GetComponent<Animator>();
        _gameManager = GameManager.Instance;
    }

    /**
     * FixedUpdate fonction to override for all Character Child
     * Set Y movement direction to manage movement on Jump, Fall and Grounded state 
     */
    protected virtual void FixedUpdate()
    {
	    Move(_movementDirection);
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
	    Vector2 positionOffset = Vector2.zero;
	    positionOffset.x = _movementDirection.x * Speed;
	    currentYSpeed = (OnGround)? 0 : IncrementSpeed(currentYSpeed, MaxFallSpeed, Gravity);
	    if (InJump)
	    {
		    positionOffset.y += Mathf.Sqrt(2 * Gravity * JumpHeight);
	    }
	    positionOffset.y -= 1 * currentYSpeed;
	    _movementDirection.y = Mathf.Sign(positionOffset.y);
	    Rigidbody.MovePosition(Rigidbody.position + positionOffset * Time.deltaTime);
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
	
    private float IncrementSpeed(float currentSpeed, float targetSpeed, float acceleration)
    {
	    if (currentSpeed >= targetSpeed)
	    {
		    return targetSpeed;
	    }
	    currentSpeed += acceleration * Time.deltaTime;
	    return (currentSpeed >= targetSpeed)? targetSpeed : currentSpeed;
    }
    
    /**
     * Flip Character Sprite
     */
    protected void Flip()
    {
	    float angle = (_faceRight) ? 180 : 0;
		transform.eulerAngles = new Vector3(0, angle, 0);
		_faceRight = !_faceRight;
    }
	
    /**
     * Start Character Jump
     */
    protected virtual void Jump()
    {
	    SwitchJumpState();
	    StartCoroutine("Jumping");
    }
	
    /**
     * Change current jump state
     * Switch animation consequently 
     */
    public void SwitchJumpState()
    {
	    InJump = !InJump;
	    SetBoolAnim("InJump",InJump);
    }
    
    /**
     * Coroutine called when Character Jump
     * End jump when starts falling
     */
    IEnumerator Jumping()
    {
	    while (InJump)
	    {
		    if (_movementDirection.y < 0 )
		    {
			    SwitchJumpState();
		    }
		    yield return new WaitForEndOfFrame();
	    }
	    //Reset Y speed to avoid a fall too fast
	    currentYSpeed = 0;
    }
    
    public void SetOnGround(bool value)
    {
        OnGround = value;
        CanJump = value;
        SetBoolAnim("OnGround",value);
    }
    
    public bool IsJumping()
    {
	    return InJump;
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

    public void SetLevelManager(LevelManager levelManager)
    {
	    _levelManager = levelManager;
    }
}
