using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orge : MonoBehaviour
{
    public int treadsLeft;
    public int speed;
    public Weapon mainGun;
    public List<Weapon> secondaryGuns = new List<Weapon>();
    public List<Weapon> missiles = new List<Weapon>();
    private int ramNumber;

    void Start()
    {
        InitializeWeapons();
        treadsLeft = 45;
        speed = 3;
        ramNumber = 0;
    }

    
    void Update()
    {
        
    }

    void InitializeWeapons()
    {
        mainGun = new Weapon(4, 3, 4);
        secondaryGuns.Add(new Weapon(3, 2, 3));
        secondaryGuns.Add(new Weapon(3, 2, 3));
        secondaryGuns.Add(new Weapon(3, 2, 3));
        secondaryGuns.Add(new Weapon(3, 2, 3));
        missiles.Add(new Weapon(6, 5, 2));
        missiles.Add(new Weapon(6, 5, 2));
    }

    public void AttackUnit(Unit unit, int weapon)//1-MainGun, 2-5 SecondaryGun1-4, 6-7 Missile1-2
    {
        int combatResult = 0;
        if(weapon == 1)
        {
            if (mainGun.isAttacked)
            {
                return;
            }
            combatResult = GameManager.instance.CalculateCombat(4, unit.defense);
            mainGun.isAttacked = true;
        }
        else if(weapon >= 2 && weapon <= 5)
        {
            if(secondaryGuns[weapon - 2].isAttacked)
            {
                return;
            }
            combatResult = GameManager.instance.CalculateCombat(3, unit.defense);
            secondaryGuns[weapon - 2].isAttacked = true;
        }
        else if(weapon == 6||weapon == 7)
        {
            if (missiles[weapon - 6].isUsedMissile)
            {
                return;
            }
            combatResult = GameManager.instance.CalculateCombat(6, unit.defense);
            missiles[weapon - 6].isUsedMissile = true;
        }
        if (unit.gameObject.tag.Equals("CommandPost"))
        {
            Instantiate(GameManager.instance.destroyFX, unit.transform.position, unit.transform.rotation);
            GameManager.instance.Lose();
            return;
        }
        if(combatResult == 1)
        {
            if(unit.disabled == 0)
            {
                unit.disabled++;
                unit.DisableFX();
            }
            else if(unit.disabled == 1)
            {
                Instantiate(GameManager.instance.destroyFX, unit.transform.position, unit.transform.rotation);
                Destroy(unit.gameObject);
            }
        }
        else if(combatResult == 2)
        {
            Instantiate(GameManager.instance.destroyFX, unit.transform.position, unit.transform.rotation);
            Destroy(unit.gameObject);
        }
    }

    public void RamUnit(Unit unit)
    {
        if(ramNumber >= 2)
        {
            return;
        }
        if (unit.gameObject.tag.Equals("HeavyTank"))
        {
            LoseTreads(3);
            Instantiate(GameManager.instance.destroyFX, unit.transform.position, unit.transform.rotation);
            Destroy(unit.gameObject);
        }
        else if (unit.gameObject.tag.Equals("CommandPost"))
        {
            Instantiate(GameManager.instance.destroyFX, unit.transform.position, unit.transform.rotation);
            GameManager.instance.Lose();
            return;
        }
        else
        {
            Instantiate(GameManager.instance.destroyFX, unit.transform.position, unit.transform.rotation);
            LoseTreads(1);
            Destroy(unit.gameObject);
        }
        ramNumber++;
    }

    public void LoseTreads(int lost)
    {
        treadsLeft -= lost;
        if(treadsLeft >= 31 && treadsLeft <= 45)
        {
            speed = 3;
        }
        else if(treadsLeft >= 16 && treadsLeft <= 30)
        {
            speed = 2;
        }
        else if (treadsLeft >= 1 && treadsLeft <= 15)
        {
            speed = 1;
        }
        else if (treadsLeft == 0)
        {
            speed = 0;
            GameManager.instance.Win();
        }
        UpdateOrgeInfo();
    }

    public void ResetOrgeWeapon()
    {
        mainGun.isAttacked = false;
        foreach(var i in secondaryGuns)
        {
            i.isAttacked = false;
        }
        ramNumber = 0;
    }

    public void UpdateOrgeInfo()
    {
        int mg, sg, mis;
        if (mainGun.isDestroyed)
        {
            mg = 0;
        }
        else
        {
            mg = 1;
        }
        sg = 0;
        mis = 0;
        foreach(var i in secondaryGuns)
        {
            if (!i.isDestroyed)
            {
                sg++;
            }
        }
        foreach(var i in missiles)
        {
            if (!i.isDestroyed)
            {
                mis++;
            }
        }
        GameManager.instance.UpdateOrgeInfo(mg, sg, mis, treadsLeft, speed);
    }
}
