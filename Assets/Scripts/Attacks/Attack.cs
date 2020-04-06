using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public abstract class Attack : ScriptableObject, IAttack
{


    public string Name { get; set; }
    public int Damage { get; set; }
    public int Cost { get; set; }
    public int Level { get; set; }
    public string Type { get; set; }
    public float Cool_Down { get; set; }
    public string Description { get; set; }

    public float AttackRange;

    public int Id_Character;

    public abstract void Lauch(Character character);
}
