using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawTrap : MonoBehaviour
{
	public GameObject Saw;
	public float SpeedRotation = 180;
	public float Speed;
	public float SawDamages = 1;
	private Vector2 _tileSize;
	private List<Transform> _saws;

	private Vector2 _nextPos;

	private Transform _sawTransform;
    // Start is called before the first frame update
    void Start()
    {
	    _tileSize = transform.parent.GetComponent<BoxCollider2D>().size;
	    Saw.GetComponent<SpriteRenderer>().sortingOrder = LevelGenerator.TrapSortingOrder;
	    
	    _sawTransform = Saw.transform;
	    _sawTransform.localPosition = new Vector2( _tileSize.x/2,   _tileSize.y/2);
	    _nextPos = _sawTransform.localPosition;
	    _nextPos.x *= -1;
    }

    // Update is called once per frame
    void Update()
    {
	    _sawTransform.Rotate(Saw.transform.forward,SpeedRotation*Time.deltaTime);
		_sawTransform.localPosition = Vector2.MoveTowards(_sawTransform.localPosition,_nextPos,Speed*Time.deltaTime);
		if (_sawTransform.localPosition.Equals(_nextPos))
		{
			_nextPos.x *= -1;
		}
    }
	
    /**
     * When a saw hit the player
     * Give him damages
     * This callback is call because Saw are child of this object without rigidbody
     */
    private void OnTriggerEnter2D(Collider2D other)
    {
	    if (other.CompareTag("Player"))
	    {
		    Character character = other.GetComponent<Character>();
		    if (character)
		    {
			    other.GetComponent<Character>().TakeDamages(SawDamages);
		    }
	    }
    }
}
