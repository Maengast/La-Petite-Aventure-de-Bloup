using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Life_Point { get; set; }
    public int Attack_Multiplier { get; set; }
    public List<Attack> Attacks { get; set; }
}
