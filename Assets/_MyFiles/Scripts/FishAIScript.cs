using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAIScript : MonoBehaviour
{
    enum State
    {
        Moving,
        Escaping,
        Resisting
    }

    public List<Transform> target;
    public Transform Hook;
    public GameObject hookobj;
    public float speed = 5f;

    private Transform newTarget;
    public Transform baitDetector;
    public Transform hookLoc;
    State state;

    Rigidbody rb;

    bool isMoving;

    private Transform water;

    public Material fishMat;

    private float Pullingtimer = 5;
    private float Resisttimer = 4;

    public FishType fishSO;

    Coroutine resistingCoroutine;

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

        rb.isKinematic = true;

        Physics.IgnoreCollision(transform.GetComponent<Collider>(), water.GetComponent<Collider>());

        state = State.Moving;
    }


    // Update is called once per frame
    void Update()
    {
        if (state == State.Moving)
        {
            Move();
        }
        else if (state == State.Escaping)
        {
            newTarget = null;
        }

    }

    private void Resist()
    {
        StartCoroutine(ResistTimer(Resisttimer));
    }

    private void Move()
    {   if(!isMoving || newTarget == null)
        {
            newTarget = target[Random.Range(0, target.Count)];
        }

        transform.LookAt(newTarget);
        isMoving = true;
        transform.position = Vector3.MoveTowards(transform.position, newTarget.position, speed * Time.deltaTime);

        if(transform.position == newTarget.position)
        {
            isMoving = false;
        }
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
            other.gameObject.transform.SetParent(hookLoc, true);
            other.gameObject.transform.position = hookLoc.position;

            GetComponent<Collider>().enabled = false;
            target.Clear();
            Hooked();
        }
    }


    IEnumerator PullTimer() // time till line "snaps" when fish is currently resisting
    {
        yield return new WaitForSeconds(1);
        Pullingtimer--;      
        Debug.Log($"pulling time is:{Pullingtimer}");
        if (Pullingtimer <= 0)
        {
            Pullingtimer = 0;
            ResistEnded();
        }

        if (fishMat.color == Color.red)
        {
            Hook.transform.SetParent(null);
            Hook.transform.rotation = Quaternion.identity;
            Hook.transform.GetComponent<Collider>().enabled = true;

            Destroy(transform.gameObject);
        }

    }

    void ResistEnded()
    {
        //rb.AddRelativeForce(Vector3.forward * 2f, ForceMode.Force);
        fishMat.color = Color.white;
        state = State.Escaping;
    }

    void ResistTimerEnd()
    {
        int randNum = RandomNum();
        if (randNum == 5)
        {
            StartCoroutine(ResistTimer(Resisttimer));
            
        }
        else
        {
            ResistEnded();
        }
    }

    int RandomNum()
    {
        return Random.Range(1, 6);
        
    }

    IEnumerator ResistTimer(float resistTimer)
    {
        state = State.Resisting;
        fishMat.color = Color.red;
        while (resistTimer > 0)
        {
            yield return new WaitForSeconds(1);
            resistTimer--;
            //rb.AddRelativeForce(Vector3.back * 4f, ForceMode.Force);
            Debug.Log($"resist time is: {resistTimer}");
        }

        if(resistTimer <= 0)
        {
            ResistTimerEnd();
            resistingCoroutine = null;
            yield return null;
        }
    }

    internal void Hooked()
    {
        Resist();
    }

    internal bool IsResisting()
    {
        return state == State.Resisting;
    }
}
