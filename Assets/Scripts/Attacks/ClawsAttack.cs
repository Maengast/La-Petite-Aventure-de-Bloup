using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawsAttack : Attack
{
    public ClawsAttack()
    {
        Damage = 3;
        Name = "Claws";
        AttackRange = 1;
    }
    public override void Lauch(Character character)
    {
        Damage = character.Attack_Multiplier * Damage;
        Claws claws = character.gameObject.GetComponentInChildren<Claws>();
        if (claws != null)
        {
            claws.SetDammages(Damage);
        }
    }
}
