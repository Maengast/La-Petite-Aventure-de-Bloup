using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CircularSaw : AttackObject
{
    public float RotationSpeed = 5.0f;
    public float Speed = 5;
    private Rigidbody2D _rb;
    private CircleCollider2D _collider;
    private void Start() 
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CircleCollider2D>();
        Invoke("DestroySaw", 5);
    }
    // Update is called once per frame
    void Update()
    {
         _rb.AddForce(Direction *Speed *  Vector2.right);                                    
        transform.Rotate(0f, 0f, Direction.x *RotationSpeed *360* Time.deltaTime);
    }
    void DestroySaw()
    {
        Destroy(gameObject);
    }
}
