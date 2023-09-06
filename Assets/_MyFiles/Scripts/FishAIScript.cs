using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAIScript : MonoBehaviour
{
    public List<Transform> target;
    public Transform Hook;
    public GameObject hookobj;
    public Transform escapePos;
    bool isMoving = false;
    public float speed = 5f;
    float collisionTime;

    private Transform newTarget;
    public Transform baitDetector;
    public Transform hookLoc;
    private bool Escaping = false;
    private bool isResisting = false;

    Rigidbody rb;

    private Transform water;

    public Material fishMat;

    private float Pullingtimer = 5;
    private float Resisttimer = 4;
    private int randNum;

    public FishType fishSO;

    // Start is called before the first frame update
    void Start()
    {

        fishMat = transform.GetComponent<Renderer>().material;
        rb = transform.GetComponent<Rigidbody>();

        water = GameObject.FindWithTag("Water").transform; //if doing multiple, change as below

        GameObject[] targetobjs = GameObject.FindGameObjectsWithTag("Waypoint");
        foreach(GameObject obj in targetobjs)
        { 
            target.Add(obj.transform);
            Physics.IgnoreCollision(transform.GetComponent<Collider>(), obj.GetComponent<Collider>());
        }

        Physics.IgnoreCollision(transform.GetComponent<Collider>(), water.GetComponent<Collider>());

    }

    // Update is called once per frame
    void Update()
    {
        if(!isMoving)
        {
            newTarget = target[Random.Range(0, target.Count)];
            isMoving = true;
        }
        if(!Escaping)
        {
            transform.LookAt(newTarget);
            transform.position = Vector3.MoveTowards(transform.position, newTarget.position, speed * Time.deltaTime);
        }
        else
        {
            StartCoroutine(ResistTimer(Resisttimer));

            
            // StartCoroutine(PullTimer());

            if (isResisting)
            {
                if (Input.GetMouseButton(1))
                {
                    Pullingtimer -= Time.deltaTime;
                    Debug.Log($"pulling time is:{Pullingtimer}");
                    if (Pullingtimer <= 0)
                    {
                        Pullingtimer = 0;
                        PullTimerEnd();
                    }
                    fishMat.color = Color.Lerp(fishMat.color, Color.red, Time.deltaTime / Pullingtimer);

                    if (Input.GetMouseButtonUp(1))
                    {
                        fishMat.color = Color.white;
                        Pullingtimer = 5;
                    }

                    if (fishMat.color == Color.red)
                    {
                        Hook.transform.SetParent(null);
                        Hook.transform.rotation = Quaternion.identity;
                        Hook.transform.GetComponent<Collider>().enabled = true;

                        isResisting = false;
                        Destroy(transform.gameObject);
                    }
                }
            }

        }

        if(transform.position == newTarget.position)
        {
            isMoving = false;
        }

        //OnTriggerEnter(hookobj.GetComponent<BoxCollider>());
    }
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(baitDetector.transform.position, 1);
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("collided");
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "Hook")
        {
            Escaping = true;
            Debug.Log("hooked");

            other.gameObject.transform.SetParent(hookLoc, true);
            other.gameObject.transform.position = hookLoc.position;

            GetComponent<Collider>().enabled = false;
            target.Clear();
            //target.Add(escapePos);
            //newTarget = escapePos;
        }
        else
            return;
    }


    IEnumerator PullTimer()
    {
        yield return new WaitForSeconds(1);
        Pullingtimer--;      
        Debug.Log($"pulling time is:{Pullingtimer}");
        if (Pullingtimer <= 0)
        {
            Pullingtimer = 0;
            PullTimerEnd();
        }

        if (fishMat.color == Color.red)
        {
            Hook.transform.SetParent(null);
            Hook.transform.rotation = Quaternion.identity;
            Hook.transform.GetComponent<Collider>().enabled = true;

            isResisting = false;
            Destroy(transform.gameObject);
        }

    }

    void PullTimerEnd()
    {
        rb.AddRelativeForce(Vector3.forward * 0f, ForceMode.Force);
        fishMat.color = Color.white;
        isResisting = false;
    }

    IEnumerator ResistTimerEnd()
    {
        isResisting = false;
        RandomNum();
        if (randNum == 5)
        {
            StartCoroutine(ResistTimer(Resisttimer));
        }
        else
        {
            isResisting = false;
            PullTimerEnd();

            yield return new WaitForSeconds(1);
            RandomNum();
        }
    }

    int RandomNum()
    {
        randNum = Random.Range(1, 6);
        return randNum;
    }

    IEnumerator ResistTimer(float resistTimer)
    {
        rb.AddRelativeForce(Vector3.back * 2f, ForceMode.Force);

        while (resistTimer > 0)
        {
            isResisting = true;
            yield return new WaitForSeconds(1);
            resistTimer--;
            Debug.Log($"resist time is: {resistTimer}");
        }
        if(resistTimer <= 0)
        {
            StartCoroutine(ResistTimerEnd());
        }
    }
}
