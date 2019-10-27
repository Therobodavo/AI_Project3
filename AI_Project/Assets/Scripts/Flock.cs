using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{

    public FlockManager myManager;
    float speed;
    bool turning = false;


    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(myManager.minSpeed, myManager.maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        //Is it outside the bounds?
        Bounds b = new Bounds(myManager.transform.position, myManager.limit * 2);

        RaycastHit hit = new RaycastHit();
        Vector3 direction = Vector3.zero;

        //Turn around
        if (!b.Contains(transform.position))
        {
            turning = true;
            direction = myManager.transform.position - transform.position;
        }

        else if (Physics.Raycast(transform.position, this.transform.forward * 10, out hit))
        {
            turning = true;
            //Debug.DrawRay(this.transform.position, this.transform.forward * 10, Color.red);
        } 

        else
        {
            turning = false;
            direction = Vector3.Reflect(this.transform.forward, hit.normal);
        }

        if(turning)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), myManager.rotation * Time.deltaTime);
        }
        else
        {
            FlockingRules();
        }
        
        transform.Translate(0, 0, Time.deltaTime * speed);
    }

    void FlockingRules()
    {
        GameObject[] gos;
        gos = myManager.allBirds;

        Vector3 vcenter = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.01f;
        float nDistance;
        int groupSize = 0;

        foreach(GameObject go in gos)
        {
            if(go!=this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                if(nDistance <= myManager.neighbor)
                {
                    vcenter += go.transform.position;
                    groupSize++;

                    if(nDistance < 1.0f)
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }

                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }
        }
        if(groupSize > 0)
        {
            vcenter = vcenter / groupSize + (myManager.goalPosition - this.transform.position);
            speed = gSpeed / groupSize;

            Vector3 direction = (vcenter + vavoid) - transform.position;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                     Quaternion.LookRotation(direction), 
                                                     myManager.rotation * Time.deltaTime);
        }
    }
}
