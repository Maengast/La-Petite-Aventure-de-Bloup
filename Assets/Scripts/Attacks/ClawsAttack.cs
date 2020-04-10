using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawsAttack : Attack
{
    public ClawsAttack()
    {
        AttackModel.Name = "Claws";
    }
    public override void Lauch(Character character)
    {
        float damages = character.AttackMultiplier * AttackModel.Damage;
        Claws claws = character.gameObject.GetComponentInChildren<Claws>();
        if (claws != null)
        {
            claws.SetDammages(damages);
        }
    }
}
