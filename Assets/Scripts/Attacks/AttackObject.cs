using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObject : MonoBehaviour
{
    protected float Dammages;
    private Vector2 _screenBounds;
    public Character Launcher;
    public Vector3 Direction;
    private void Awake()
    {
        _screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }
    private void Update()
    {
        if (transform.position.x > _screenBounds.x *-2f || transform.position.x < _screenBounds.x * 2f)
        {
            Destroy(gameObject);
        }
    }
    public void SetDammages(float value)
    {
        Dammages = value;
    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        Character character = other.collider.GetComponent<Character>();
        if (character && character != Launcher)
        {
            character.TakeDamages(Dammages);
            Destroy(gameObject);
        }
    }
}
