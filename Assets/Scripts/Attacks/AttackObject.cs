using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObject : MonoBehaviour
{
    protected float _damages;
    public Character Launcher;
    public Vector3 Direction;



    public void SetDammages(float value)
    {
        _damages = value;
    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        Character character = other.collider.GetComponent<Character>();
        if (character && character != Launcher)
        {
            character.TakeDamages(_damages);
            Destroy(gameObject);
        }
    }
}
