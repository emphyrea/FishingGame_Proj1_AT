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

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] targetobjs = GameObject.FindGameObjectsWithTag("Waypoint");
        foreach(GameObject obj in targetobjs)
        { 
            target.Add(obj.transform);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(!isMoving)
        {
            newTarget = target[Random.Range(0, target.Count)];
            isMoving = true;
        }
        transform.LookAt(newTarget);
        transform.position = Vector3.MoveTowards(transform.position, newTarget.position, speed * Time.deltaTime);

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
            Debug.Log("hook");
            other.gameObject.transform.SetParent(transform, true);
            other.gameObject.transform.position = transform.position;
            target.Clear();
            target.Add(escapePos);
            newTarget = escapePos;
        }
        else
            return;
    }
}
