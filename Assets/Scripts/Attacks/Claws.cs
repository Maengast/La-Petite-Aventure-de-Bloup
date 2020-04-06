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
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        Character character = other.GetComponent<Character>();
        if (character && AreOut)
        {
            character.TakeDamages(Dammages);
            AreOut = false;
        }
    }
}
