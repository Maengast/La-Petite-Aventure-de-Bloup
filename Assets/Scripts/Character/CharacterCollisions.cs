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
	    _widthOffset = CharacterCollider.bounds.size.x / 3;
    }
    
    void Update()
    {
	    _colliderBounds = CharacterCollider.bounds;
	    CheckTopAndBottomCollisions();
	    //CheckSideCollisions();
    }
	
    /**
     * Draw an area to detect any collision with the ground
     */
    private bool CheckCollisionWithGround(Vector2 originPoint, int dirY, float sizeX, float sizeY)
    {
	    Vector2 targetPoint = new Vector2(originPoint.x + sizeX, originPoint.y + (dirY * sizeY));
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
     * Check collision with platforms above character
     */
    private void CheckTopAndBottomCollisions()
    {
	    float sizeX = _colliderBounds.size.x;
	    Vector2 origin = _colliderBounds.min;
	    //origin.x += _widthOffset / 2;
	    
	    //Bottom
	    bool hit = CheckCollisionWithGround(origin, -1, sizeX,_heightOffset );
	    if (hit && !CharacterInstance.OnGround)
	    {
		    CharacterInstance.SetOnGround(true);
	    }
	    else if(!hit && CharacterInstance.OnGround)
	    {
		    CharacterInstance.SetOnGround(false);
	    }
	    
	    //top
	    origin.y = _colliderBounds.max.y;
	    hit = CheckCollisionWithGround(origin, 1, sizeX,_heightOffset);
	    if (hit && CharacterInstance.IsJumping())
	    {
		    CharacterInstance.SwitchJumpState(false);
	    }
    }
    
    /**
     * Check collisions with platforms on each side of character
     */
    private void CheckSideCollisions()
    {
	    float sizeY = _colliderBounds.size.y - _heightOffset;
	    Vector2 origin = _colliderBounds.min;
	    origin.y += _heightOffset / 2;
	    
	    //Left
	    int dirX = 0;
	    bool hit = CheckCollisionWithGround(origin, 1, -_widthOffset,sizeY);
	    if (hit) dirX = -1;
	    else
	    {
		    //Right
		    origin.x = _colliderBounds.max.x;
		    hit = CheckCollisionWithGround(origin, 1, _widthOffset,sizeY);
		    if(hit)  dirX = 1;
	    }
	    
	    
	    CharacterInstance.CollisionOnSides(dirX);
	    if (hit && CharacterInstance.IsJumping())
	    {
		    CharacterInstance.SwitchJumpState(false);
	    }
    }
}
