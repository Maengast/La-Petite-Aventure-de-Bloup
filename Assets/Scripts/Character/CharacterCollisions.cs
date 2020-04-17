using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class CharacterCollisions : MonoBehaviour
{
	public Character CharacterInstance;
	public LayerMask GroundLayerMask;
	public Collider2D CharacterCollider;

	private Bounds _colliderBounds;
	private float  _heightOffset = 0.1f;
	private float _widthOffset;

	void Start()
    {
	    if(!CharacterInstance)CharacterInstance = GetComponent<Character>();
	    if(!CharacterCollider)CharacterCollider = GetComponent<BoxCollider2D>();
	    GroundLayerMask = LayerMask.GetMask("Ground");
	    _widthOffset = CharacterCollider.bounds.size.x / 2;
    }
    
    void Update()
    {
	    _colliderBounds = CharacterCollider.bounds;
	    CheckBottomCollision();
	    CheckTopCollision();
    }
	
    /**
     * Draw an area to detect any collision with the ground
     */
    private bool CheckCollisionWithGround(Vector2 originPoint, int dir)
    {
	    float colliderXSize = _colliderBounds.size.x - _widthOffset;
	    originPoint.x += _widthOffset / 2;
	    Vector2 targetPoint = new Vector2(originPoint.x + colliderXSize, originPoint.y + (dir * _heightOffset));
	    Collider2D hitCollider = Physics2D.OverlapArea(originPoint, targetPoint,GroundLayerMask);
	    
	    //Debug
	    Debug.DrawLine(originPoint,new Vector2(targetPoint.x,originPoint.y),Color.red);
	    Debug.DrawLine(originPoint,new Vector2(originPoint.x,targetPoint.y),Color.red);
	    Debug.DrawLine(new Vector2(targetPoint.x,originPoint.y),targetPoint,Color.red);
	    Debug.DrawLine(new Vector2(originPoint.x,targetPoint.y),targetPoint,Color.red);

	    return hitCollider;
    }
    
    /**
     * Check Collision with platforms under character
     */
    private void CheckBottomCollision()
    {
	    bool hit = CheckCollisionWithGround(_colliderBounds.min, -1);
	    if (hit && !CharacterInstance.OnGround && !CharacterInstance.IsJumping())
	    {
		    CharacterInstance.SetOnGround(true);
	    }
	    else if(!hit && CharacterInstance.OnGround)
	    {
		    CharacterInstance.SetOnGround(false);
	    }
    }
	
    /**
     * Check collision with platform above character
     */
    private void CheckTopCollision()
    {
	    bool hit = CheckCollisionWithGround(new Vector2(_colliderBounds.min.x,_colliderBounds.max.y), 1);
	    if (hit && CharacterInstance.IsJumping())
	    {
		    CharacterInstance.SwitchJumpState();
	    }
    }
}
