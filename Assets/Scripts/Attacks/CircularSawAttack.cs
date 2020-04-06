using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularSawAttack : Attack
{
    GameObject sawPrefab;
    Transform firePoint;
    int numberOfCircleSaw;
    private void Awake()
    {
        sawPrefab = Resources.Load("CircleSaw") as GameObject;
        numberOfCircleSaw = 2;
    }
    public CircularSawAttack()
    {
        Damage = 10;
        Name = "CircularSaw";
    }
    public override void Lauch(Character character)
    {
        Damage = character.Attack_Multiplier * Damage;
        firePoint = character.transform.Find("FirePoint");

        for(int i = 0; i<numberOfCircleSaw; i++)
        {
            GameObject circleSaw = Instantiate(sawPrefab, firePoint.position, firePoint.rotation) as GameObject;
            circleSaw.name = Name;
            circleSaw.GetComponent<CircularSaw>().SetDammages(Damage / numberOfCircleSaw);
            circleSaw.GetComponent<CircularSaw>().SetLaucher(character);
        }

    }
}
