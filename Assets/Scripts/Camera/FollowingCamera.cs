using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
	private int _levelHeight;
	private int _levelWidth;
	private float _sizeX;
	private float _sizeY;
	private GameObject _player;
	
	/**
	 * Save camera size
	 * Init camera position
	 */
	private void Start()
	{
		Camera camera = GetComponent<Camera>();
		
		_sizeY = 2*camera.orthographicSize;
		_sizeX = _sizeY * camera.aspect;

		transform.position = new Vector3(_sizeX/2,_sizeY/2,transform.position.z);
	}

	/**
	 * Save player and level bounds
	 */
	public void Init(GameObject player, int height, int width)
	{
		_player = player;
		_levelHeight = height;
		_levelWidth = width;
	}
	
    // Update is called once per frame
    void Update()
    {
	    if (_player)
	    {
		    FollowPlayer();
	    }
    }
	
    /**
     * Follow player
     * Block follow if camera bounds reach level bounds
     */
    private void FollowPlayer()
    {
	    Vector2 playerPos = _player.transform.position;
	    Vector3 camPos = transform.position;
	    Vector3 newPos = camPos;
	    if (playerPos.x >= _sizeX / 2 && playerPos.x <= _levelWidth - _sizeX / 2)
	    {
		    newPos.x = playerPos.x;
	    }

	    if (playerPos.y <= _levelHeight - _sizeY / 2 && playerPos.y >= _sizeY / 2)
	    {
		    newPos.y = playerPos.y;
	    }

	    if(!newPos.Equals(camPos))transform.position = newPos;
    }

    
}
