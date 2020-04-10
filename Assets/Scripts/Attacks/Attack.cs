using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public abstract class Attack : ScriptableObject, IAttack
{

    public string Name;

    public AttackModel AttackModel;
    public abstract void Lauch(Character character);
}
