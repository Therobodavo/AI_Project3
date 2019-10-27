using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{

    public int attack;
    public int range;
    public int defense;
    public bool isDestroyed;
    public bool isAttacked;
    public bool isUsedMissile;

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
