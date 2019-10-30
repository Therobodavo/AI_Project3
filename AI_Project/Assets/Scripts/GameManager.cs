﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Choose Panel(appear when unit is selected)
    public GameObject choosePanel;
    public GameObject aimButton;
    public GameObject attackButton;
    public GameObject moveButton;
    public GameObject rammingButton;

    //Attack Panel(appear when unit is aiming)
    public GameObject attackPanel;
    public GameObject mainGunButton;
    public List<GameObject> secondaryGunButtons;
    public List<GameObject> missileButtons;
    public GameObject treadButton;

    //Icons
    public GameObject targetPrefab;
    public GameObject errorPrefab;

    //Main UI
    public GameObject mainUI;
    public Text turnText;
    public Text orgeInfoText;
    public Button nextTurn;

    //Two different cameras and reference of UI when player is setting up units
    public GameObject mainCamera;
    public GameObject setupCamera;
    public GameObject setupUI;
    public GameObject setupIcons;

    //Reference of Orge
    public GameObject orge;

    //FXs
    public GameObject disableFX;
    public GameObject destroyFX;
    public GameObject explosionFX;

    [HideInInspector]public State state;
    [HideInInspector]public int turnNumber = 0;

    //the chosen unit
    private GameObject chooseUnit;

    //the state which show what player is doing
    public enum State
    {
        Idle,
        WaitForOrge,
        Selected,
        Aiming,
        Moving,
        Ramming
    }

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        #region Initialize
        choosePanel.SetActive(false);
        attackPanel.SetActive(false);
        state = State.Idle;
        aimButton.GetComponent<Button>().onClick.AddListener(ChooseAimTarget);
        attackButton.GetComponent<Button>().onClick.AddListener(AttackTarget);
        moveButton.GetComponent<Button>().onClick.AddListener(ChooseMoveTarget);
        rammingButton.GetComponent<Button>().onClick.AddListener(ChooseRamTarget);
        mainGunButton.GetComponent<Button>().onClick.AddListener(delegate () { this.AimWeapon(1); });
        secondaryGunButtons[0].GetComponent<Button>().onClick.AddListener(delegate () { this.AimWeapon(2); });
        secondaryGunButtons[1].GetComponent<Button>().onClick.AddListener(delegate () { this.AimWeapon(3); });
        secondaryGunButtons[2].GetComponent<Button>().onClick.AddListener(delegate () { this.AimWeapon(4); });
        secondaryGunButtons[3].GetComponent<Button>().onClick.AddListener(delegate () { this.AimWeapon(5); });
        missileButtons[0].GetComponent<Button>().onClick.AddListener(delegate () { this.AimWeapon(6); });
        missileButtons[1].GetComponent<Button>().onClick.AddListener(delegate () { this.AimWeapon(7); });
        treadButton.GetComponent<Button>().onClick.AddListener(delegate () { this.AimWeapon(8); });
        nextTurn.onClick.AddListener(EndTurn);
        SetUp();
        #endregion
    }


    void Update()
    {
        //Allow player be able to switch camera when playing
        if (Input.GetKeyDown(KeyCode.T) && turnNumber > 0)
        {
            SwitchCamera();
        }
        #region WhenPlayerClick
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                #region When player want to choose a unit or cancel choice
                if (hit.collider != null && (state == State.Idle || state == State.Selected))
                {
                    if (hit.collider.tag.Equals("HeavyTank") && hit.collider.gameObject.GetComponent<Unit>().disabled == 0)
                    {
                        chooseUnit = hit.collider.gameObject;
                        if (chooseUnit.GetComponent<Unit>().isAttacked)
                        {
                            choosePanel.SetActive(false);
                        }
                        else
                        {
                            choosePanel.SetActive(true);
                            moveButton.SetActive(true);
                            rammingButton.SetActive(true);
                        }
                        state = State.Selected;
                    }
                    else if (hit.collider.tag.Equals("MissileTank") && hit.collider.gameObject.GetComponent<Unit>().disabled == 0)
                    {
                        chooseUnit = hit.collider.gameObject;
                        choosePanel.SetActive(true);
                        if (chooseUnit.GetComponent<Unit>().isAttacked)
                        {
                            choosePanel.SetActive(false);
                        }
                        else
                        {
                            choosePanel.SetActive(true);
                            moveButton.SetActive(true);
                            rammingButton.SetActive(true);
                        }
                        state = State.Selected;
                    }
                    else if (hit.collider.tag.Equals("Howitzer") && hit.collider.gameObject.GetComponent<Unit>().disabled == 0)
                    {
                        chooseUnit = hit.collider.gameObject;
                        if (chooseUnit.GetComponent<Unit>().isAttacked)
                        {
                            choosePanel.SetActive(false);
                        }
                        else
                        {
                            choosePanel.SetActive(true);
                            moveButton.SetActive(false);
                            rammingButton.SetActive(false);
                        }
                        state = State.Selected;
                    }
                    else if (hit.collider.tag.Equals("GEV") && hit.collider.gameObject.GetComponent<Unit>().disabled == 0)
                    {
                        chooseUnit = hit.collider.gameObject;
                        choosePanel.SetActive(true);
                        moveButton.SetActive(true);
                        rammingButton.SetActive(true);
                        if (chooseUnit.GetComponent<Unit>().isAttacked)
                        {
                            attackButton.SetActive(false);
                            aimButton.SetActive(false);
                        }
                        else
                        {
                            attackButton.SetActive(true);
                            aimButton.SetActive(true);
                        }
                        state = State.Selected;
                    }
                    else
                    {
                        //when player want to cancel choice
                        if(chooseUnit != null)
                        {
                            ClearCircle(chooseUnit.transform);
                        }
                        chooseUnit = null;
                        moveButton.SetActive(true);
                        aimButton.SetActive(true);
                        attackButton.SetActive(true);
                        choosePanel.SetActive(false);
                        attackPanel.SetActive(false);
                        state = State.Idle;
                    }
                }
                #endregion
                #region When player want to choose a target loction for chosen unit
                if (state == State.Moving)
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground") && Vector3.Distance(hit.point, chooseUnit.transform.position) <= chooseUnit.GetComponent<Unit>().currentMovement) 
                    {
                        bool canReach = chooseUnit.GetComponent<AStarPathFindingForUnits>().SetLocationAsTarget(hit.point);
                        if (canReach)
                        {
                            targetPrefab.SetActive(true);
                            targetPrefab.transform.position = hit.point + new Vector3(0, 2, 0);
                            chooseUnit.GetComponent<Unit>().currentMovement -= Vector3.Distance(hit.point, chooseUnit.transform.position);
                            
                        }
                        else
                        {
                            targetPrefab.SetActive(false);
                            errorPrefab.SetActive(true);
                            errorPrefab.transform.position = Input.mousePosition;
                            Invoke("HideErrorPrefab", 0.5f);
                            state = State.Idle;
                        }
                    }
                    else
                    {
                        targetPrefab.SetActive(false);
                        errorPrefab.SetActive(true);
                        errorPrefab.transform.position = Input.mousePosition;
                        Invoke("HideErrorPrefab", 0.5f);
                        state = State.Idle;
                    }
                    ClearCircle(chooseUnit.transform);
                    chooseUnit = null;
                }
                #endregion
                #region When player want to choose a target to aim
                if (state == State.Aiming)
                {
                    RaycastHit temp;
                    bool isBlocked = false;
                    if (Physics.Linecast(hit.point, chooseUnit.transform.position, out temp))
                    { 
                        if (temp.collider.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
                        {
                            isBlocked = true;
                        }
                    }
                    if (hit.collider.tag.Equals("Orge") && Vector3.Distance(hit.point, chooseUnit.transform.position) <= chooseUnit.GetComponent<Unit>().range && (hit.point.y - chooseUnit.transform.position.y) < 4.0f && !isBlocked)
                    {
                        attackPanel.SetActive(true);
                        HandleAttackPanel();
                        attackPanel.transform.position = Input.mousePosition;
                        ClearCircle(chooseUnit.transform);
                    }
                    else
                    {
                        errorPrefab.SetActive(true);
                        errorPrefab.transform.position = Input.mousePosition;
                        Invoke("HideErrorPrefab", 0.5f);
                        ClearCircle(chooseUnit.transform);
                        state = State.Idle;
                        chooseUnit = null;
                    }
                }
                #endregion
                #region When player want to choose a target to ram
                if (state == State.Ramming)
                {
                    if (hit.collider.tag.Equals("Orge") && Vector3.Distance(hit.point, chooseUnit.transform.position) <= chooseUnit.GetComponent<Unit>().currentMovement)
                    {
                        bool canReach = chooseUnit.GetComponent<AStarPathFindingForUnits>().SetLocationAsTarget(hit.point);
                        if (canReach)
                        {
                            Debug.Log("Ramming");
                        }
                        else
                        {
                            errorPrefab.SetActive(true);
                            errorPrefab.transform.position = Input.mousePosition;
                            Invoke("HideErrorPrefab", 0.5f);
                            state = State.Idle;
                        }
                    }
                    else
                    {
                        errorPrefab.SetActive(true);
                        errorPrefab.transform.position = Input.mousePosition;
                        Invoke("HideErrorPrefab", 0.5f);
                        state = State.Idle;
                    }
                    ClearCircle(chooseUnit.transform);
                    chooseUnit = null;
                }
                #endregion
            }
        }
        #endregion
        //Update the position of choose panel to let it follow the chosen unit
        if (chooseUnit != null)
        {
            choosePanel.transform.position = Camera.main.WorldToScreenPoint(chooseUnit.transform.position);
        }
    }

    //Hide choose panel and allow player to choose a target location
    private void ChooseMoveTarget()
    {
        state = State.Moving;
        DrawCircle(chooseUnit.transform, chooseUnit.transform.position, chooseUnit.GetComponent<Unit>().currentMovement);
        choosePanel.SetActive(false);
    }

    //Hide choose panel and allow player to choose a ramming target
    private void ChooseRamTarget()
    {
        state = State.Ramming;
        DrawCircle(chooseUnit.transform, chooseUnit.transform.position, chooseUnit.GetComponent<Unit>().currentMovement);
        choosePanel.SetActive(false);
    }

    //Hide choose panel and allow player to choose to aim a target
    private void ChooseAimTarget()
    {
        state = State.Aiming;
        DrawCircle(chooseUnit.transform, chooseUnit.transform.position, chooseUnit.GetComponent<Unit>().range);
        choosePanel.SetActive(false);
    }

    //Attack the target of the unit
    //Handle the combined attack by letting the units which are aiming at the same target with current chosen unit to attack
    private void AttackTarget()
    {
        if(chooseUnit.GetComponent<Unit>().target == 0)
        {
            return;
        }
        Unit[] units = FindObjectsOfType<Unit>();
        int attack = 0;
        foreach(var i in units)
        {
            if(i.target == chooseUnit.GetComponent<Unit>().target)
            {
                attack += i.attack;
                i.isAttacked = true;
            }
        }
        Debug.Log(chooseUnit.GetComponent<Unit>().target.ToString() + " " + attack.ToString());
        //Calculate Combat
        int combatResult;
        if (chooseUnit.GetComponent<Unit>().target == 1)
        {
            combatResult = CalculateCombat(attack, orge.GetComponent<Orge>().mainGun.defense);
            if(combatResult == 2)
            {
                Instantiate(destroyFX, orge.transform.position, orge.transform.rotation);
                orge.GetComponent<Orge>().mainGun.isDestroyed = true;
            }
            else
            {
                Instantiate(explosionFX, orge.transform.position, orge.transform.rotation);
            }
        }
        else if (chooseUnit.GetComponent<Unit>().target >= 2 && chooseUnit.GetComponent<Unit>().target <= 5)
        {
            combatResult = CalculateCombat(attack, orge.GetComponent<Orge>().secondaryGuns[chooseUnit.GetComponent<Unit>().target - 2].defense);
            if (combatResult == 2)
            {
                Instantiate(destroyFX, orge.transform.position, orge.transform.rotation);
                orge.GetComponent<Orge>().secondaryGuns[chooseUnit.GetComponent<Unit>().target - 2].isDestroyed = true;
            }
            else
            {
                Instantiate(explosionFX, orge.transform.position, orge.transform.rotation);
            }
        }
        else if (chooseUnit.GetComponent<Unit>().target == 6 || chooseUnit.GetComponent<Unit>().target == 7)
        {
            combatResult = CalculateCombat(attack, orge.GetComponent<Orge>().missiles[chooseUnit.GetComponent<Unit>().target - 6].defense);
            if (combatResult == 2)
            {
                Instantiate(destroyFX, orge.transform.position, orge.transform.rotation);
                orge.GetComponent<Orge>().missiles[chooseUnit.GetComponent<Unit>().target - 6].isDestroyed = true;
            }
            else
            {
                Instantiate(explosionFX, orge.transform.position, orge.transform.rotation);
            }
        }
        else if (chooseUnit.GetComponent<Unit>().target == 8)
        {
            combatResult = CalculateCombat(1, 1);
            if(combatResult == 2)
            {
                Instantiate(destroyFX, orge.transform.position, orge.transform.rotation);
                orge.GetComponent<Orge>().LoseTreads(attack);
            }
            else
            {
                Instantiate(explosionFX, orge.transform.position, orge.transform.rotation);
            }
        }
        orge.GetComponent<Orge>().UpdateOrgeInfo();
        choosePanel.SetActive(false);
    }

    void HideErrorPrefab()
    {
        errorPrefab.SetActive(false);
    }

    #region Draw Circle Fucntion
    LineRenderer GetLineRenderer(Transform t)
    {
        LineRenderer lr = t.GetComponent<LineRenderer>();
        if(lr == null)
        {
            lr = t.gameObject.AddComponent<LineRenderer>();
        }
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        return lr;
    }

    void DrawCircle(Transform t, Vector3 center, float radius)
    {
        LineRenderer lr = GetLineRenderer(t);
        int pointAmount = 100;
        float eachAngle = 360f / pointAmount;
        Vector3 forward = t.forward;
        lr.positionCount = pointAmount + 1;
        for(int i = 0; i <= pointAmount; i++)
        {
            Vector3 pos = Quaternion.Euler(0f, eachAngle * i, 0f) * forward * radius + center;
            lr.SetPosition(i, pos);
        }
    }

    void ClearCircle(Transform t)
    {
        LineRenderer lr = GetLineRenderer(t);
        lr.positionCount = 0;
    }
    #endregion

    //Call this function to end current turn
    void EndTurn()
    {
        state = State.WaitForOrge;

        StartNewTurn();
    }

    //Call this function to start a new turn(should be called by Orge)
    public void StartNewTurn()
    {
        Unit[] units = FindObjectsOfType<Unit>();
        foreach(var i in units)
        {
            i.ResetState(false);
            if(i.disabled == 0)
            {
                i.disabled++;
            }
            else if(i.disabled == 1)
            {
                i.disabled = 0;
                i.DestroyDisableFX();
            }
        }
        turnNumber++;
        turnText.text = "Turn: " + turnNumber.ToString();
        state = State.Idle;
        if(chooseUnit != null)
        {
            ClearCircle(chooseUnit.transform);
        }
        chooseUnit = null;
        moveButton.SetActive(true);
        attackButton.SetActive(true);
        aimButton.SetActive(true);
        choosePanel.SetActive(false);
        attackPanel.SetActive(false);

        orge.GetComponent<Orge>().ResetOrgeWeapon();
    }

    //Call this function to start the game(also you can code something here, for example if you want to call a method when game started)
    public void StartGame()
    {
        mainCamera.SetActive(true);
        setupCamera.SetActive(false);
        setupUI.SetActive(false);
        setupIcons.SetActive(false);
        mainUI.SetActive(true);
        turnNumber = 1;
        turnText.text = "Turn: " + turnNumber.ToString();
        UpdateOrgeInfo(1, 4, 2, 45, 3);
    }

    //Update the information of Orge to UI
    public void UpdateOrgeInfo(int mainGunCount, int secondaryGunCount, int missilesCount, int treadsLeft, int speed)
    {
        orgeInfoText.text = "Orge Info\nMain Gun: " + mainGunCount.ToString() + "\nSecondary Gun: " + secondaryGunCount.ToString() + "\nMissiles: " + missilesCount.ToString() + "\n\nTreads Left: " + treadsLeft.ToString() + "\nSpeed: " + speed.ToString();
    }

    //Initialize UI
    public void SetUp()
    {
        mainCamera.SetActive(false);
        setupCamera.SetActive(true);
        setupUI.SetActive(true);
        setupIcons.SetActive(true);
        mainUI.SetActive(false);
        choosePanel.SetActive(false);
    }

    void SwitchCamera()
    {
        if (mainCamera.activeSelf)
        {
            mainCamera.SetActive(false);
            setupCamera.SetActive(true);
        }
        else
        {
            mainCamera.SetActive(true);
            setupCamera.SetActive(false);
        }
    }

    //Call this method when player choose a target for a unit
    void AimWeapon(int index)
    {
        chooseUnit.GetComponent<Unit>().target = index;
        chooseUnit.transform.LookAt(orge.transform.position);
        attackPanel.SetActive(false);
        chooseUnit = null;
        state = State.Idle;
    }

    //Handle the Attack Panel(if some weapons of Orge are already destroyed, then they won't appear in UI)
    void HandleAttackPanel()
    {
        for(int i = 0; i < secondaryGunButtons.Count; i++)
        {
            secondaryGunButtons[i].SetActive(!orge.GetComponent<Orge>().secondaryGuns[i].isDestroyed);
        }
        for (int i = 0; i < missileButtons.Count; i++)
        {
            missileButtons[i].SetActive(!orge.GetComponent<Orge>().missiles[i].isDestroyed);
        }
        if (orge.GetComponent<Orge>().mainGun.isDestroyed)
        {
            mainGunButton.SetActive(false);
        }
        else
        {
            mainGunButton.SetActive(true);
        }
        if(orge.GetComponent<Orge>().treadsLeft > 0)
        {
            treadButton.SetActive(true);
        }
        else
        {
            treadButton.SetActive(false);
        }
    }

    //Call this method when player wins
    public void Win()
    {
        Debug.Log("Win");
        //Do something when win
    }

    //Call this method when player loses
    public void Lose()
    {
        Debug.Log("Lose");
        //Do something when lose
    }

    //Use this method to handle the combat calculate
    //return value: 0 = NE, 1 = D, 2 = X
    public int CalculateCombat(int attack, int defense)
    {
        int dieRoll = (int)Random.Range(1, 7);
        if(defense == 0)
        {
            return 2;
        }
        else if ((float)(attack / defense) < 0.5f)
        {
            return 0;
        }
        else if (defense / attack == 2)
        {
            if(dieRoll <= 4)
            {
                return 0;
            }
            else if(dieRoll == 5)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        else if (attack == defense)
        {
            if (dieRoll <= 2)
            {
                return 0;
            }
            else if (dieRoll <= 4)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        else if (attack / defense == 2)
        {
            if (dieRoll == 1)
            {
                return 0;
            }
            else if (dieRoll <= 3)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        else if (attack / defense == 3)
        {
            if (dieRoll <= 2)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        else if (attack / defense == 4)
        {
            if (dieRoll == 1)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        else
        {
            return 2;
        }
    }
}
