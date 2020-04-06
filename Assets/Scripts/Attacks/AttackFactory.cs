using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackFactory 
{

    public Attack GetAttack(string attackName)
    {
        switch (attackName)
        {
            case "Claws": 
                return new CircularSawAttack();
            default:
                return null;
        }
    }
}
