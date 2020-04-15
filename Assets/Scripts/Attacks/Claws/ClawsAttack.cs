using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawsAttack : Attack
{
    public ClawsAttack()
    {
        Name = "Claws";
    }
    public override void Launch(Character character)
    {
        float damages = character.AttackMultiplier * AttackModel.Damage;
        Claws claws = character.gameObject.GetComponentInChildren<Claws>();
        if (claws != null)
        {
            claws.SetDammages(damages);
        }
    }
}
