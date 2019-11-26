using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCompensateRotation : MonoBehaviour
{
    public GameObject emptyPrefab;

    void Start()
    {
        CompensateRotation();
    }

    void Update()
    {
        CompensateRotation();
    }

    public void CompensateRotation()
    {
        Transform parent = GetComponentInParent<BoxCollider>().transform;
        if(parent == null)
        {
            return;
        }
        GameObject go = Instantiate(emptyPrefab, new Vector3(parent.position.x, parent.position.y + 16, parent.position.z), Quaternion.Euler(0, 0, 0));
        go.transform.SetParent(parent);
        transform.position = go.transform.position;
        transform.rotation = go.transform.rotation;
        Destroy(go);

    }
}
