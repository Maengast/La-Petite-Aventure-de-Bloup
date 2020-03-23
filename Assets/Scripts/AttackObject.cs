using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObject : MonoBehaviour
{
    protected float Dammages;
    

    protected void SetDammages(float value)
    {
        Dammages = value;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        Character character = other.GetComponent<Character>();
        if (character)
        {
            character.TakeDamages(Dammages);
        }
        Destroy(gameObject);
    }
}
