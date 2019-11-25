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
    public int acceptableTotalAttackValueFromUnits = 6;
    public bool isRamHeavyTank = false;
    [Range(0, 100)]public int runAwayWeights = 50;

    public List<Transform> spawnPoints;

    [HideInInspector] public bool isGoingToAttack = false;

    private int ramNumber;
    private int remainMovement;

    void Start()
    {
        InitializeWeapons();
        treadsLeft = 45;
        speed = 3;
        ramNumber = 0;
        remainMovement = speed;
    }

    
    void Update()
    {
        //About how to detect units: Use FindObjectsOfTypes or OverlapSphere or etc
        //Debug.Log(remainMovement.ToString() + " " + ramNumber.ToString());

    }

    //Set the attack/range/defense of the weapons of Orge
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

    //Use this method to cause damage to unit by a certain weapon(doesn't include movement)
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
            Destroy(unit.gameObject);
            GameManager.instance.Lose();
            return;
        }
        if(combatResult == 0)
        {
            Instantiate(GameManager.instance.explosionFX, unit.transform.position, unit.transform.rotation);
        }
        else if(combatResult == 1)
        {
            if(unit.disabled == 0)
            {
                Instantiate(GameManager.instance.explosionFX, unit.transform.position, unit.transform.rotation);
                unit.disabled++;
                unit.DisableFX();
            }
            else if(unit.disabled == 1 || unit.disabled == 2)
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

    //Use this method to ram unit(doesn't include movement)
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
            Destroy(unit.gameObject);
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

    //Use this method to handle the treads damage(automatically decrease speed if need)
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

    //Use this method to reset the Orge's state when each turn starts
    public void ResetOrgeWeapon()
    {
        mainGun.isAttacked = false;
        foreach(var i in secondaryGuns)
        {
            i.isAttacked = false;
        }
        ramNumber = 0;
        remainMovement = speed;
    }

    //Update current Orge's state information to UI
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

    public void ChooseSpawnLocation()
    {
        int minDistance = 10000;
        int minIndex = 0;
        for(int i = 0; i < spawnPoints.Count; i++)
        {

            if (GetComponent<AStarPathFindingForOrge>().DistanceBetweenTwoPoints(spawnPoints[i].position, GameManager.instance.commandPost.transform.position) < minDistance)
            {
                minDistance = GetComponent<AStarPathFindingForOrge>().DistanceBetweenTwoPoints(spawnPoints[i].position, GameManager.instance.commandPost.transform.position);
                minIndex = i;
            }
        }
        transform.position = spawnPoints[minIndex].position;
    }

    public void AIDecision()
    {
        if (CanAttackCommandPost() != -1)
        {
            if(GetMaxAttackRange() == 0)
            {
                GetComponent<AStarPathFindingForOrge>().MoveTowardsCommandPost(CanAttackCommandPost());
                RamUnit(GameManager.instance.commandPost.GetComponent<Unit>());
            }
            else
            {
                GetComponent<AStarPathFindingForOrge>().MoveTowardsCommandPost(CanAttackCommandPost());
                AttackAfterMove();
            }
        }
        else
        {
            Ram();
        }
    }

    void DecisionAfterRam()
    {
        if (GetPossibleTotalAttackedValueFromUnits(transform.position) <= acceptableTotalAttackValueFromUnits)
        {
            Vector3 estimate = GetComponent<AStarPathFindingForOrge>().EstimatePostion(1, GameManager.instance.commandPost.transform.position);
            if (remainMovement >= 1 && !IsOverlapUnit(estimate) && GetPossibleTotalAttackedValueFromUnits(estimate) <= acceptableTotalAttackValueFromUnits)
            {
                estimate = GetComponent<AStarPathFindingForOrge>().EstimatePostion(2, GameManager.instance.commandPost.transform.position);
                if (remainMovement >= 2 && !IsOverlapUnit(estimate) && GetPossibleTotalAttackedValueFromUnits(estimate) <= acceptableTotalAttackValueFromUnits)
                {
                    estimate = GetComponent<AStarPathFindingForOrge>().EstimatePostion(3, GameManager.instance.commandPost.transform.position);
                    if (remainMovement >= 3 && !IsOverlapUnit(estimate) && GetPossibleTotalAttackedValueFromUnits(estimate) <= acceptableTotalAttackValueFromUnits)
                    {
                        GetComponent<AStarPathFindingForOrge>().MoveTowardsCommandPost(3);
                        AttackAfterMove();
                    }
                    else
                    {
                        GetComponent<AStarPathFindingForOrge>().MoveTowardsCommandPost(2);
                        AttackAfterMove();
                    }
                }
                else
                {
                    GetComponent<AStarPathFindingForOrge>().MoveTowardsCommandPost(1);
                    AttackAfterMove();
                }
            }
            else if(remainMovement >= 1)
            {
                int seed = Random.Range(0, 100);
                if (seed >= runAwayWeights)
                {
                    GetComponent<AStarPathFindingForOrge>().MoveTowardsCommandPost(1);
                    AttackAfterMove();
                }
                else
                {
                    GetComponent<AStarPathFindingForOrge>().RunAway(remainMovement, GetLocationOfClosestUnit(transform.position));
                    AttackAfterMove();
                }
            }
            else
            {
                Attack(transform.position);
            }
        }
        else
        {
            int seed = Random.Range(0, 100);
            if (seed >= runAwayWeights)
            {
                GetComponent<AStarPathFindingForOrge>().MoveTowardsCommandPost(1);
                AttackAfterMove();
            }
            else
            {
                GetComponent<AStarPathFindingForOrge>().RunAway(remainMovement, GetLocationOfClosestUnit(transform.position));
                AttackAfterMove();
            }
        }
    }

    int GetMaxAttackRange()
    {
        int maxAttackRange = 0;
        foreach (var i in secondaryGuns)
        {
            if (!i.isDestroyed && !i.isAttacked)
            {
                maxAttackRange = 3;
                break;
            }
        }
        if (!mainGun.isDestroyed && !mainGun.isAttacked)
        {
            maxAttackRange = 4;
        }
        foreach (var i in missiles)
        {
            if (!i.isDestroyed && !i.isUsedMissile)
            {
                maxAttackRange = 6;
                break;
            }
        }
        return maxAttackRange;
    }

    int CanAttackCommandPost()
    {
        int maxAttackRange = GetMaxAttackRange();
        if (Vector3.Distance(GameManager.instance.commandPost.transform.position, transform.position) <= maxAttackRange && !GameManager.instance.IsBlockedBetweenTwoPoints(GameManager.instance.commandPost.transform.position, transform.position))
        {
            return 0;
        }
        for(int i = 1; i <= speed; i++)
        {
            Vector3 estimatePoint = GetComponent<AStarPathFindingForOrge>().EstimatePostion(i, GameManager.instance.commandPost.transform.position);
            if (Vector3.Distance(GameManager.instance.commandPost.transform.position, estimatePoint) <= maxAttackRange && !GameManager.instance.IsBlockedBetweenTwoPoints(GameManager.instance.commandPost.transform.position, estimatePoint)) 
            {
                return i;
            }
        }
        return -1;

    }

    Vector3 GetLocationOfClosestUnit(Vector3 position)
    {
        int minDistance = 100;
        Vector3 temp = Vector3.zero;
        foreach(var i in GetUnitsInMainGunRange(position))
        {
            if (GetComponent<AStarPathFindingForOrge>().DistanceBetweenTwoPoints(position, i.transform.position) < minDistance) 
            {
                minDistance = GetComponent<AStarPathFindingForOrge>().DistanceBetweenTwoPoints(position, i.transform.position);
                temp = i.transform.position;
            }
        }
        return temp;
    }

    List<Unit> GetUnitsInRammingRange(Vector3 position, int distance)
    {
        List<Unit> units = new List<Unit>();
        foreach (var i in FindObjectsOfType<Unit>())
        {
            if (GetComponent<AStarPathFindingForOrge>().DistanceBetweenTwoPoints(i.transform.position, position) <= distance)
            {
                units.Add(i);
            }
        }
        return units;
    }

    List<Unit> GetUnitsInMainGunRange(Vector3 position)
    {
        List<Unit> units = new List<Unit>();
        foreach(var i in FindObjectsOfType<Unit>())
        {
            if(Vector3.Distance(i.transform.position, position) < 4.0f && !GameManager.instance.IsBlockedBetweenTwoPoints(i.transform.position, position))
            {
                units.Add(i);
            }
        }
        return units;
    }

    List<Unit> GetUnitsInSecondaryGunRange(Vector3 position)
    {
        List<Unit> units = new List<Unit>();
        foreach (var i in FindObjectsOfType<Unit>())
        {
            if (Vector3.Distance(i.transform.position, position) < 3.0f && !GameManager.instance.IsBlockedBetweenTwoPoints(i.transform.position, position))
            {
                units.Add(i);
            }
        }
        return units;
    }

    List<Unit> GetUnitsInMissileRange(Vector3 position)
    {
        List<Unit> units = new List<Unit>();
        foreach (var i in FindObjectsOfType<Unit>())
        {
            if (Vector3.Distance(i.transform.position, position) < 6.0f && !GameManager.instance.IsBlockedBetweenTwoPoints(i.transform.position, position))
            {
                units.Add(i);
            }
        }
        return units;
    }

    List<Unit> GetUnitsCanAttackOrge(Vector3 position)
    {
        List<Unit> units = new List<Unit>();
        foreach (var i in FindObjectsOfType<Unit>())
        {
            if (Vector3.Distance(i.transform.position, position) < i.range && !GameManager.instance.IsBlockedBetweenTwoPoints(i.transform.position, position))
            {
                units.Add(i);
            }
        }
        return units;
    }
    
    int GetPossibleTotalAttackedValueFromUnits(Vector3 position)
    {
        int sum = 0;
        foreach(var i in GetUnitsCanAttackOrge(position))
        {
            sum += i.attack;
        }
        return sum;
    }

    bool IsOverlapUnit(Vector3 location)
    {
        List<Unit> units = GetUnitsInMainGunRange(location);

        foreach (var i in units)
        {
            if(Vector3.Distance(location, i.transform.position) < 1.0f)
            {
                return true;
            }
        }
        return false;
    }

    public void AttackAfterMove()
    {
        isGoingToAttack = true;
    }

    public void Ram()
    {
        List<Unit> units = GetUnitsInRammingRange(transform.position, remainMovement);
        if(units.Count <= 0 || ramNumber >= 2)
        {
            DecisionAfterRam();
            return;
        }
        int minDistance = 10000;
        Unit temp = units[0];
        foreach (var i in units)
        {
            if (isRamHeavyTank && i.tag.Equals("HeavyTank"))
            {
                continue;
            }
            int distance = GetComponent<AStarPathFindingForOrge>().DistanceBetweenTwoPoints(transform.position, i.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                temp = i;
            }
        }
        if(remainMovement >= minDistance)
        {
            GetComponent<AStarPathFindingForOrge>().SetLocationAsTarget(temp.transform.position);
            RamUnit(temp);
            remainMovement -= minDistance;
            Invoke("Ram", 1.0f);
        }
        else
        {
            DecisionAfterRam();
            return;
        }
    }

    public void Attack(Vector3 position)
    {
        List<Unit> units = GetUnitsInMainGunRange(position);
        if (units.Count > 0 && !mainGun.isDestroyed)
        {
            int minDefense = 10;
            Unit temp = units[0];
            List<Unit> u = new List<Unit>();
            foreach (var i in units)
            {
                if(i.disabled == 1 || i.disabled == 2)
                {
                    u.Add(i);
                }
                if(i.defense < minDefense)
                {
                    minDefense = i.defense;
                    temp = i;
                }
            }
            if (u.Count > 0)
            {
                int maxDefense = 0;
                foreach (var i in u)
                {
                    if (i.defense > maxDefense)
                    {
                        maxDefense = i.defense;
                        temp = i;
                    }
                }
            }
            AttackUnit(temp, 1);
        }
        List<Unit> units2 = GetUnitsInSecondaryGunRange(position);
        for (int j = 0; j < secondaryGuns.Count; j++) 
        {
            if (units2.Count > 0 && !secondaryGuns[j].isDestroyed)
            {
                int minDefense = 10;
                Unit temp = units2[0];
                List<Unit> u = new List<Unit>();
                foreach (var i in units2)
                {
                    if (i.disabled == 1 || i.disabled == 2)
                    {
                        u.Add(i);
                    }
                    if (i.defense < minDefense)
                    {
                        minDefense = i.defense;
                        temp = i;
                    }
                }
                if (u.Count > 0)
                {
                    int maxDefense = 0;
                    foreach (var i in u)
                    {
                        if (i.defense > maxDefense)
                        {
                            maxDefense = i.defense;
                            temp = i;
                        }
                    }
                }
                AttackUnit(temp, j + 2);
            }
        }
        List<Unit> units3 = GetUnitsInMissileRange(position);
        for (int j = 0; j < missiles.Count; j++)
        {
            if (units3.Count > 0 && !missiles[j].isDestroyed && !missiles[j].isUsedMissile)
            {
                Unit temp = units3[0];
                foreach (var i in units3)
                {
                    if (i.defense == 0)
                    {
                        temp = i;
                        break;
                    }else if(i.defense == 3)
                    {
                        temp = i;
                    }
                }
                AttackUnit(temp, j + 6);
            }
        }
        isGoingToAttack = false;
        EndAction();
    }

    void EndAction()
    {
        GameManager.instance.StartNewTurn();
    }
    public void UpdateInfluenceCircle()
    {
        float r = remainMovement;
        r += GetMaxAttackRange();
        r *= 3;
        GameObject c = GetComponentInChildren<CapsuleCollider>().gameObject;
        if (c != null && c.tag.Equals("Influence"))
        {
            c.transform.localScale = new Vector3(r, 1, r);
            c.GetComponent<MeshRenderer>().sortingOrder = 10;
        }
    }
}
