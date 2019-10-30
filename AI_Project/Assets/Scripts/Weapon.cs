using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{

    public int attack;
    public int range;
    public int defense;
    public bool isDestroyed;//is this weapon destroyed
    public bool isAttacked;//is this weapon already attacked this turn
    public bool isUsedMissile;//is this weapon a missile and it's already attacked

    public Weapon() { }

    public Weapon(int a, int r, int d)
    {
        attack = a;
        range = r;
        defense = d;
        isDestroyed = false;
        isAttacked = false;
        isUsedMissile = false;
    }
}
