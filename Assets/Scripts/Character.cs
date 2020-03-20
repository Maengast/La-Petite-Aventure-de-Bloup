using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //Speed
    public float XSpeed = 2.0f;
    public float FallSpeed = -2.0f;
    public float JumpSpeed = 5.0f;
    protected float YSpeed;
    protected float XCurrentSpeed = 0.0f;
    protected Vector2 LastPos;
    
    //Life
    public float MaxLife = 50;
    protected float Life = 0;

    private bool FaceRight = true;
    public bool IsGrounded = false;
    
    //Jump
    public float MaxJumpHeight = 8.0f;
    protected bool CanJump = true;
    protected bool OnJump = false;

    protected Rigidbody2D Rigidbody;
    protected Animator CharacterAnimator;
    
    protected virtual void Init()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        CharacterAnimator = GetComponent<Animator>();
        YSpeed = FallSpeed;
        LastPos = Rigidbody.position;
    }

    protected virtual void Move()
    {
        
    }
    
    protected virtual void Move(Vector2 moveDirection)
    {
        if((moveDirection.x > 0 && !FaceRight) || (moveDirection.x < 0 && FaceRight)) Flip();
        moveDirection.x *= XSpeed;
        moveDirection.y *= YSpeed;
        Rigidbody.MovePosition(Rigidbody.position + moveDirection * Time.deltaTime);
    }

    protected virtual void Move(Vector3 target)
    {
        
    }

    protected void Flip()
    {
        Debug.Log("Flip");
        if (FaceRight)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        FaceRight = !FaceRight;
    }

    protected virtual void Jump()
    {
        CanJump = false;
        YSpeed = JumpSpeed;
        OnJump = true;
        SetBoolAnim("OnJump",true);
    }

    protected virtual void EndJump()
    {
        OnJump = false;
        YSpeed = FallSpeed;
        SetBoolAnim("OnJump",false);
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
