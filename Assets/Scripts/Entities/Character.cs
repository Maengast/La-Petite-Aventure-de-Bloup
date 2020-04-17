using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

//Required components
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(CharacterCollisions))]

public class Character : MonoBehaviour
{
	protected const float Gravity = 9.5f;
    
	[Header("Character Components")]
	public Rigidbody2D Rigidbody;
	public Animator CharacterAnimator;
	public BarScript HealthBar;
	
	[Header("Jump Values")]
	public float JumpHeight = 5.0f;
	private bool _inJump = false;
	
    [Header("Speed Values")]
    public float Speed = 8.0f;
    public float MaxFallSpeed = 15.0f;
    
    private float currentFallSpeed;
    protected float currentSpeed;
    
    //Movement and Direction
    protected Vector2 _movementDirection;
    private bool _faceRight = true;
    public bool OnGround = false;
    
    //Character Life
    protected float MaxLife = 100;
    protected float Life = 0;
    
    //Character Attack
    public AttackInventory AttackInventory;
    public float AttackMultiplier;

    protected GameManager _gameManager;
    protected LevelManager _levelManager;
    
    /**
     * Initialise Character Instance
     * Get references of main components
     */
    protected void Init()
    {
        if(!Rigidbody)Rigidbody = GetComponent<Rigidbody2D>();
        if(!CharacterAnimator)CharacterAnimator = GetComponent<Animator>();
        _gameManager = GameManager.Instance;
        Life = MaxLife;
        if(HealthBar) HealthBar.SetMaxValue(MaxLife);
        currentSpeed = Speed;
    }

    protected void SetCharacterStats(CharacterInfo info)
    {
	    Speed = info.Speed;
	    MaxLife = info.MaxLife;
	    AttackMultiplier = info.Attack_Multiplier;
    }

    /**
     * Called every fixed frame-rate frame
     * Do physics calculations
     * Update character movements
     */
    protected virtual void FixedUpdate()
    {
	    Move();
    }
    
    /**
     * Main function Move
     * Move Character along a directions vector multiply by speed
     * Flip sprite in function of X movement direction
     */
    public virtual void Move()
    {
        //Debug.Log(_movementDirection.x);
        //flip sprite with character direction
        if ((_movementDirection.x > 0 && !_faceRight) || (_movementDirection.x < 0 && _faceRight)) Flip();
	    
	    Vector2 positionOffset = Vector2.zero;
	    //calc X movement
	    positionOffset.x = _movementDirection.x * currentSpeed;
	    
	    //Define character fall speed with gravity acceleration
	    currentFallSpeed = (OnGround)? 0 : IncrementSpeed(currentFallSpeed, MaxFallSpeed, Gravity);
	    positionOffset.y -= 1 * currentFallSpeed;
	    if (_inJump)
	    {
		    positionOffset.y += /*_movementDirection.y*/ Mathf.Sqrt(2 * Gravity * JumpHeight);
		    if (Mathf.Sign(positionOffset.y) < 0)
		    {
			    SwitchJumpState();
		    }
	    }
	    Rigidbody.MovePosition(Rigidbody.position + positionOffset * Time.deltaTime);
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
	    //StartCoroutine("Jumping");
    }

    /**
     * Change current jump state
     * Switch animation consequently 
     */
    public void SwitchJumpState()
    {
	    _inJump = !_inJump;
	    SetBoolAnim("InJump",_inJump);
	    if (!_inJump)
	    {
		    currentFallSpeed = 0;
	    }
    }
    
    /**
     * Coroutine called when Character Jump
     * End jump when starts falling
     */
    // private IEnumerator Jumping()
    // {
	   //  while (_inJump)
	   //  {
    //         if (_movementDirection.y < 0)
		  //   {
			 //    SwitchJumpState();
		  //   }
		  //   yield return new WaitForEndOfFrame();
	   //  }
	   //  //Reset Y speed to avoid a fall too fast
	   //  currentFallSpeed = 0;
    // }
    
    public virtual void SetOnGround(bool value)
    {
        OnGround = value;
        SetBoolAnim("OnGround",value);
    }
    
    public bool IsJumping()
    {
	    return _inJump;
    }

    public void Attack(IAttack attack)
    {

        attack.Launch(this);
    }
    public virtual void TakeDamages(float damages)
    {
        Life -= damages;
        HealthBar.SetValue(Life);
        if (Life <= 0)
            Die();
    }

    protected virtual void Die()
    {
        SetTriggerAnim("Die");
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
