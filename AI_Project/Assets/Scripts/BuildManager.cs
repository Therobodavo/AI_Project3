using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    public GameObject commandPostButton;
    public GameObject heavyTankButton;
    public GameObject missileTankButton;
    public GameObject howitzerButton;
    public GameObject gevButton;
    public GameObject commandPostPrefab;
    public GameObject heavyTankPrefab;
    public GameObject missileTankPrefab;
    public GameObject howitzerPrefab;
    public GameObject gevPrefab;
    public GameObject error;
    public Text pointsText;
    public Button resetButton;
    public Button startButton;
    public GameObject warning;

    private GameObject currentChoose;
    private UnitKind currentChooseKind;
    private List<GameObject> units = new List<GameObject>();
    private int howitzerCount = 0;
    private int points = 20;
    private bool isPlacedCommandPost = false;

    public enum UnitKind
    {
        CommandPost,
        HeavyTank,
        MissileTank,
        Howitzer,
        GEV
    }

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        error.SetActive(false);
        pointsText.text = "Points: " + points.ToString();
        resetButton.onClick.AddListener(ResetUnits);
        startButton.onClick.AddListener(StartGame);
        HideWarning();
    }

    
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit hit;
            if (Physics.Raycast(mousePos, Vector3.down, out hit))
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject == commandPostButton && isPlacedCommandPost == false)
                    {
                        currentChoose = Instantiate(commandPostPrefab, mousePos, transform.rotation);
                        currentChooseKind = UnitKind.CommandPost;
                    }
                    else if (hit.collider.gameObject == heavyTankButton && points >= 1)
                    {
                        currentChoose = Instantiate(heavyTankPrefab, mousePos, transform.rotation);
                        currentChooseKind = UnitKind.HeavyTank;
                    }
                    else if (hit.collider.gameObject == missileTankButton && points >= 1)
                    {
                        currentChoose = Instantiate(missileTankPrefab, mousePos, transform.rotation);
                        currentChooseKind = UnitKind.MissileTank;
                    }
                    else if (hit.collider.gameObject == howitzerButton && points >= 2 && howitzerCount < 5)
                    {
                        currentChoose = Instantiate(howitzerPrefab, mousePos, transform.rotation);
                        currentChooseKind = UnitKind.Howitzer;
                    }
                    else if (hit.collider.gameObject == gevButton && points >= 1)
                    {
                        currentChoose = Instantiate(gevPrefab, mousePos, transform.rotation);
                        currentChooseKind = UnitKind.GEV;
                    }
                }
            }
        }
        if (Input.GetMouseButton(0))
        {
            if (currentChoose != null)
            {
                currentChoose.transform.position = new Vector3(mousePos.x, 5.0f, mousePos.z);
                if (IsBlock(mousePos))
                {
                    error.SetActive(true);
                    error.transform.position = new Vector3(mousePos.x, 30.0f, mousePos.z);
                }
                else
                {
                    error.SetActive(false);
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (currentChoose != null && IsBlock(mousePos))
            {
                error.SetActive(false);
                Destroy(currentChoose);
                currentChoose = null;
            }
            else if (currentChoose != null && !IsBlock(mousePos))
            {
                units.Add(currentChoose);
                currentChoose = null;
                if (currentChooseKind == UnitKind.CommandPost)
                {
                    isPlacedCommandPost = true;
                }
                else if (currentChooseKind == UnitKind.HeavyTank)
                {
                    points -= 1;
                }
                else if (currentChooseKind == UnitKind.MissileTank)
                {
                    points -= 1;
                }
                else if (currentChooseKind == UnitKind.Howitzer)
                {
                    points -= 2;
                    howitzerCount++;
                }
                else if (currentChooseKind == UnitKind.GEV)
                {
                    points -= 1;
                }
                pointsText.text = "Points: " + points.ToString();
            }
        }
    }


    private bool IsBlock(Vector3 position)
    {
        for(int i = 0; i < units.Count; i++)
        {
            float distance = Vector2.Distance(new Vector2(position.x, position.z), new Vector2(units[i].transform.position.x, units[i].transform.position.z));
            if (distance < 1.8f)
            {
                return true;
            }
        }
        float ratioWidth = (position.x - MapScanner.instance.startPoint.x) / MapScanner.instance.cellWidth;
        float ratioLength = (position.z - MapScanner.instance.startPoint.z) / MapScanner.instance.cellLength;
        int x, y;
        if (ratioWidth - (int)ratioWidth < 0.5f)
        {
            x = (int)ratioWidth - 1;
        }
        else
        {
            x = (int)ratioWidth;
        }
        if (ratioLength - (int)ratioLength < 0.5f)
        {
            y = (int)ratioLength - 1;
        }
        else
        {
            y = (int)ratioLength;
        }
        if (x >= 50 || y >= 70 || x < 0 || y < 0) 
        {
            return true;
        }
        if (MapScanner.instance.grid[x, y].isObstacle || MapScanner.instance.grid[x, y].position.y > 6.0f)
        {
            return true;
        }
        return false;
    }

    private void ResetUnits()
    {
        points = 20;
        pointsText.text = "Points: " + points.ToString();
        howitzerCount = 0;
        isPlacedCommandPost = false;
        foreach(var i in units)
        {
            Destroy(i);
        }
        units.Clear();

    }

    private void StartGame()
    {
        if (!isPlacedCommandPost)
        {
            warning.SetActive(true);
            Invoke("HideWarning", 0.5f);
            return;
        }
        GameManager.instance.StartGame();
    }
    
    private void HideWarning()
    {
        warning.SetActive(false);
    }
}
