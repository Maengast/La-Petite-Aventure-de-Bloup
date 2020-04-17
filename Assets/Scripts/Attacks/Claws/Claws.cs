using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claws : AttackObject
{
    public bool AreOut;
    private void Start()
    {
        AreOut = false;
    }
    protected override void OnCollisionEnter2D(Collision2D other)
    {
        Character character = other.collider.GetComponent<Character>();
        if (character && AreOut)
        {
            character.TakeDamages(Damages);
            AreOut = false;
        }
    }
}
