using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObject : MonoBehaviour
{
    protected float Damages;
    private Vector2 screenBounds;
    protected Character Launcher;

    // protected void Start()
    // {
	   //  screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    // }
    //
    // protected virtual void Init()
    // {
	   //  
    // }
    //
    // private void Update()
    // {
    //     if (transform.position.x > screenBounds.x *-2f || transform.position.x < screenBounds.x * 2f)
    //     {
    //         Destroy(gameObject);
    //     }
    // }
    public void SetDamages(float value)
    {
        Damages = value;
    }

    public void SetLauncher(Character character)
    {
        Launcher = character;
    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        Character character = other.collider.GetComponent<Character>();
        if (character && character != Launcher)
        {
            character.TakeDamages(Damages);
            Destroy(gameObject);
        }
    }
}
