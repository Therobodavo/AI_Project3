using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int attack;
    public int range;
    public int defense;
    public int movement;
    [HideInInspector] public float currentMovement;//the remaining movement
    [HideInInspector] public bool isAttacked;//is this unit already attacked this turn
    [HideInInspector] public int disabled;//is this unit disabled(0 = not disabled, 1 = disabled, 2 = disabled but will be activated next turn)

    public int target;//1 = MainGun, 2-5 = SecondaryGun1-4, 6-7 = Missile1-2, 8 = Tread

    private GameObject disableFX;
    void Start()
    {
        currentMovement = movement;
        isAttacked = false;
        disabled = 0;
    }

    //Use this method to reset the state of the unit when each turn starts
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
        disableFX = Instantiate(GameManager.instance.disableFX, transform.position, transform.rotation, transform);
    }

    public void DestroyDisableFX()
    {
        Destroy(disableFX);
        disableFX = null;
    }

    public void UpdateInfluenceCircle()
    {
        float r = currentMovement;
        if (!isAttacked)
        {
            r += range;
        }
        if (disabled != 0)
        {
            r = 0;
        }
        r *= 3;
        GameObject c = GetComponentInChildren<CapsuleCollider>().gameObject;
        if(c != null && c.tag.Equals("Influence"))
        {
            c.transform.localScale = new Vector3(r, 1, r);
        }
    }
}

