using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int attack;
    public int range;
    public int defense;
    public int movement;
    [HideInInspector] public float currentMovement;
    [HideInInspector] public bool isAttacked;
    [HideInInspector] public int disabled;

    public int target;//1-MainGun, 2-5 SecondaryGun1-4, 6-7 Missile1-2, 8 Tread
    void Start()
    {
        currentMovement = movement;
        isAttacked = false;
        disabled = 0;
    }

    public void ResetState(bool isResetDisabled)
    {
        currentMovement = movement;
        isAttacked = false;
        target = 0;
        if (isResetDisabled)
        {
            disabled = 0;
        }
    }

    public void DisableFX()
    {
        Instantiate(GameManager.instance.disableFX, transform.position, transform.rotation, transform);
    }
}

