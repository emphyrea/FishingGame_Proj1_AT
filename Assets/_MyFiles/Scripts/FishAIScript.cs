using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAIScript : MonoBehaviour
{
    enum State
    {
        Moving,
        Hooked,
        Resisting
    }

    State state;

    public List<Transform> target; //targets when standard movement
    private Transform newTarget;
    bool isMoving;
    public float speed = 5f;

    public Transform Hook;
    public GameObject hookobj; //hook stuff
    public Transform hookLoc;
    public Transform baitDetector;
    GameObject pullGoal = null;

    Rigidbody rb;
    private Transform water;

    public Material fishMat;

    private float Snaptime = 4; //timers
    private float Resisttimer = 4;

    public FishType fishSO;

    Coroutine resistingCoroutine;
    bool isRunningSnapTimer = false;

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
        else if (state == State.Hooked || state == State.Resisting)
        {
            newTarget = null;
           
            if (pullGoal == null)
            {
               pullGoal = hookLoc.GetComponentInChildren<FishingScript>().pullGoal;
            }
            rb.transform.LookAt(pullGoal.transform);
        }

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

    #region Resist and Hook

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

    internal void Hooked()
    {
        Resist();
    }

    private void Resist()
    {
        rb.isKinematic = true;
        StartCoroutine(ResistTimer(Resisttimer));
    }

    IEnumerator ResistTimer(float resistTimer)
    {
        state = State.Resisting;
        while (resistTimer > 0)
        {
            yield return new WaitForSeconds(1);
            resistTimer--;
        }

        if (resistTimer <= 0)
        {
            ResistTimerEnd();
            resistingCoroutine = null;
            yield return null;
        }
    }

    void ResistTimerEnd()
    {
        int randNum = RandomNum();
        if (randNum >= 4)
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

    void ResistEnded()
    {
        fishMat.color = Color.white;
        state = State.Hooked;
    }

    internal bool IsResisting()
    {
        return state == State.Resisting;
    }

    #endregion Resist and Hook

    internal void StartSnapTimer()
    {
        StartCoroutine(SnapTimer());
    }

    IEnumerator SnapTimer() // time till line "snaps" when fish is currently resisting
    {
        isRunningSnapTimer = true;
        yield return new WaitForSeconds(1);
        Snaptime--;      
        fishMat.color = Color.Lerp(Color.white, Color.red, 1/Snaptime);


        if (Snaptime <= 0)
        {
            Snaptime = 0;
            isRunningSnapTimer = false;
            ResistEnded();
        }

        if (fishMat.color == Color.red)
        {
            Hook.transform.SetParent(null);
            Hook.transform.rotation = Quaternion.identity;
            Hook.transform.GetComponent<Collider>().enabled = true;

            Destroy(transform.gameObject);
            isRunningSnapTimer = false;
        }

    } 


}
