using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed;
    public GameObject helper;
    public GameObject openHelper;

    private bool isHelperOpen;
    void Start()
    {
        //isHelperOpen = false;
        //openHelper.SetActive(true);
        //helper.SetActive(false);
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    if (isHelperOpen)
        //    {
        //        isHelperOpen = false;
        //        openHelper.SetActive(true);
        //        helper.SetActive(false);
        //    }
        //    else
        //    {
        //        isHelperOpen = true;
        //        openHelper.SetActive(false);
        //        helper.SetActive(true);
        //    }
        //}
        if (Input.mousePosition.y > Screen.height * 0.95f && Input.mousePosition.y <= Screen.height)
        {
            transform.position = new Vector3(transform.position.x - moveSpeed * Time.deltaTime, transform.position.y, transform.position.z - moveSpeed * Time.deltaTime);
        }
        if (Input.mousePosition.y < Screen.height * 0.05f && Input.mousePosition.y >= 0)
        {
            transform.position = new Vector3(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y, transform.position.z + moveSpeed * Time.deltaTime);
        }
        if (Input.mousePosition.x > Screen.width * 0.95f && Input.mousePosition.x <= Screen.width)
        {
            transform.position = new Vector3(transform.position.x - moveSpeed * Time.deltaTime, transform.position.y, transform.position.z + moveSpeed * Time.deltaTime);
        }
        if (Input.mousePosition.x < Screen.width * 0.05f && Input.mousePosition.x >= 0)
        {
            transform.position = new Vector3(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y, transform.position.z - moveSpeed * Time.deltaTime);
        }
        if (transform.position.x > 65)
        {
            transform.position = new Vector3(65, transform.position.y, transform.position.z);
        }
        if (transform.position.x < 30)
        {
            transform.position = new Vector3(30, transform.position.y, transform.position.z);
        }
        if (transform.position.z > 85)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 85);
        }
        if (transform.position.z < 30)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 30);
        }
    }
}
