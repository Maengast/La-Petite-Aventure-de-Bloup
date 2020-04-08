using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObject : MonoBehaviour
{
    protected float Dammages;
    private Vector2 screenBounds;
    protected Character Laucher;
    private void Awake()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }
    private void Update()
    {
        if (transform.position.x > screenBounds.x *-2f || transform.position.x < screenBounds.x * 2f)
        {
            Destroy(gameObject);
        }
    }
    public void SetDammages(float value)
    {
        Dammages = value;
    }

    public void SetLaucher(Character character)
    {
        Laucher = character;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        Character character = other.GetComponent<Character>();
        if (character && character != Laucher)
        {
            character.TakeDamages(Dammages);
            Destroy(gameObject);
        }
    }
}
