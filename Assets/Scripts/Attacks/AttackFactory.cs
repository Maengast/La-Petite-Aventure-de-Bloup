using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using DataBase;

public static class AttackFactory 
{
    static Dictionary<string, Type> _attacksTypeByName;
    static Dictionary<string, AttackModel> _attacksModel;
    static bool _initilized => _attacksTypeByName != null;

    static void InitFactory()
    {
        if (_initilized)
        {
            return;
        }
        GetDatabaseAttack();
        var attackTypes = Assembly.GetAssembly(typeof(Attack)).GetTypes().Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Attack)));

        _attacksTypeByName = new Dictionary<string, Type>();
        // Get attacks names and put them into Dictionnary
        foreach(var type in attackTypes)
        {
            Attack attack = ScriptableObject.CreateInstance(type) as Attack;
            _attacksTypeByName.Add(attack.Name, type);
        }

    }
    public static Attack GetAttack(AttackModel attackModel)
    {
        InitFactory();
        if (_attacksTypeByName.ContainsKey(attackModel.Name))
        {
            Type type = _attacksTypeByName[attackModel.Name];
            Attack attack = ScriptableObject.CreateInstance(type) as Attack;
            attack.AttackModel = attackModel;
            return attack;    
        }
        return null;
    }


    static void GetDatabaseAttack()
    {
        List<AttackModel> attacks = AttackDb.GetAllAttacks();
        _attacksModel = new Dictionary<string, AttackModel>();
        // Get attacks names and put them into Dictionnary
        foreach (AttackModel model in attacks)
        {
            _attacksModel.Add(model.Name, model);
        }

    }
}
