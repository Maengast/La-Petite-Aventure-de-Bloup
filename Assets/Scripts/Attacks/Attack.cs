using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : ScriptableObject
{

    public string Name;

    public AttackModel AttackModel;
    public abstract void Launch(Character character);
}
