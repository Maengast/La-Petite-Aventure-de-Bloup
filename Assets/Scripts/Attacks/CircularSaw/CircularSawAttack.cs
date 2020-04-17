using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularSawAttack : Attack
{
    Transform _firePoint;
    int _numberOfCircleSaw;
    CircleCollider2D _collider;

    private void Awake()
    {
        _numberOfCircleSaw = 2;
        Name = "CircleSaw";
    }
    public override void Launch(Character character)
    {
        float damages = character.AttackMultiplier * AttackModel.Damage;
        _firePoint = character.transform.Find("FirePoint");
        _collider = AttackModel.ObjectPrefab.GetComponent<CircleCollider2D>();
        for (int i = 0; i< _numberOfCircleSaw; i++)
        {
            Vector3 position = _firePoint.position + (character.transform.right * i * _collider.radius * 2);
            GameObject circleSawObj = Instantiate(AttackModel.ObjectPrefab,position , _firePoint.rotation) as GameObject;
            circleSawObj.name = AttackModel.Name;
            CircularSaw circularSaw = circleSawObj.GetComponent<CircularSaw>();
            circularSaw.SetDammages(damages / _numberOfCircleSaw);
            circularSaw.Launcher = character;
            circularSaw.Direction = character.transform.right;
    }
    }



}
