using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle : MonoBehaviour
{

    public List<GameObject> units = new List<GameObject>();
    private GameManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        ToggleInfluenceOn();
        ToggleInfluenceOff();
        if (manager.isTopDown == false || manager.isStarted == false)
        {
            foreach (GameObject item in units)
            {
                if (item == null)
                {
                    units.Remove(item);
                }
                else
                {
                    item.transform.Find("Influence").gameObject.SetActive(false);
                }
            }
        }

    }

    void ToggleInfluenceOn ()
    {
        if(manager.isTopDown == true && manager.isStarted == true)
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                AddToList();
                foreach (GameObject item in units)
                {
                    if (item == null)
                    {
                        units.Remove(item);
                    }
                    else
                    {
                        item.transform.Find("Influence").gameObject.SetActive(true);

                        if (item.GetComponent<Orge>() != null)
                        {
                            item.GetComponent<Orge>().UpdateInfluenceCircle();
                        }
                        if (item.GetComponent<Unit>() != null)
                        {
                            item.GetComponent<Unit>().UpdateInfluenceCircle();
                        }
                    }
                }
            }
        }

        
    }

    void ToggleInfluenceOff()
    {
        if (manager.isTopDown == true && manager.isStarted == true)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                AddToList();
                foreach (GameObject item in units)
                {
                    if (item == null)
                    {
                        units.Remove(item);
                    }
                    else
                    {
                        item.transform.Find("Influence").gameObject.SetActive(false);
                    }
                }
            }
        }  
    }

    void AddToList()
    {
        GameObject[] HeavyTank = GameObject.FindGameObjectsWithTag("HeavyTank");
        GameObject[] Howitzer = GameObject.FindGameObjectsWithTag("Howitzer");
        GameObject[] Missle = GameObject.FindGameObjectsWithTag("MissileTank");
        GameObject[] GEV = GameObject.FindGameObjectsWithTag("GEV");
        GameObject Orge = GameObject.FindGameObjectWithTag("Orge");
        foreach (GameObject go in HeavyTank)
        {
            if (!units.Contains(go))
            {
                units.Add(go);
            }
        }

        foreach (GameObject go in Howitzer)
        {
            if (!units.Contains(go))
            {
                units.Add(go);
            }
        }

        foreach (GameObject go in Missle)
        {
            if (!units.Contains(go))
            {
                units.Add(go);
            }
        }

        foreach (GameObject go in GEV)
        {
            if (!units.Contains(go))
            {
                units.Add(go);
            }
        }
        if(!units.Contains(Orge))
        {
            units.Add(Orge);
        }

    }
}
