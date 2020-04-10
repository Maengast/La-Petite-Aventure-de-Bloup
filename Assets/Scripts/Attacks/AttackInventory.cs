using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Inventory", menuName = "Attack Inventory")]
public class AttackInventory : ScriptableObject
{
    public string Name;
    public List<AttackModel> Attacks;


    public int GetAttacksCount()
    {
        return Attacks.Count;
    }

    public AttackModel GetAttackByName(string name)
    {
        return Attacks.Find(attack => attack.Name == name);
    }

    public void AddAttack(AttackModel attack)
    {
        Attacks.Add(attack);
    }
}
