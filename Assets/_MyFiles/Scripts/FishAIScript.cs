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

    Rigidbody rb;

    private Transform water;
    

    // Start is called before the first frame update
    void Start()
    {
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
            //rb.AddRelativeForce(Vector3.forward * speed, ForceMode.Impulse);
            transform.position = Vector3.MoveTowards(transform.position, newTarget.position, speed * Time.deltaTime);
        }
        else
        {
            //rb.AddRelativeForce(Vector3.back * speed, ForceMode.Impulse);
            //transform.position = Vector3.MoveTowards(transform.position, newTarget.position, speed * Time.deltaTime);
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
}
