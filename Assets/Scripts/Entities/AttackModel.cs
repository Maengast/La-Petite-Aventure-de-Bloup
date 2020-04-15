using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Attacks")]
public class AttackModel : ScriptableObject
{

    public string Name;
    public int Damage;
    public int Cost;
    public int Level;
    public AttackType Type;
    public float Cool_Down;
    public string Description;
    public float AttackRange;
    public GameObject ObjectPrefab;
}

public enum AttackType
{
    Distance = 6,
    Rapproché = 5
}