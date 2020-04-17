using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularSawAttack : Attack
{
    Transform firePoint;
    int numberOfCircleSaw = 2;
    private void Awake()
    {
        numberOfCircleSaw = 2;
        Name = "CircleSaw";
    }
    
    public override void Launch(Character character)
    {
        float damages = character.AttackMultiplier * AttackModel.Damage;
        firePoint = character.transform.Find("FirePoint");
        for (int i = 0; i<numberOfCircleSaw; i++)
        {
            GameObject circleSaw = Instantiate(AttackModel.ObjectPrefab, firePoint.position, firePoint.rotation) as GameObject;
            circleSaw.name = AttackModel.Name;
            circleSaw.GetComponent<CircularSaw>().SetDamages(damages / numberOfCircleSaw);
            circleSaw.GetComponent<CircularSaw>().SetLauncher(character);
        }

    }
}
