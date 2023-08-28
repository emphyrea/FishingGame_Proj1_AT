using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingScript : MonoBehaviour
{

    [Header("Fishing Settings")]

    public bool hasPole = true;
    public bool isCast = false;

    public float forceToPull;
    public Vector3 hitWorldPos;

    public Camera playerCam;
    public GameObject pullGoal;

    Rigidbody rb;
    Rigidbody rbParent;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<MeshRenderer>().enabled = true;
        playerCam = Camera.main;

        rb = GetComponent<Rigidbody>();
        if(transform.parent != null)
        {
          rbParent = transform.parent.GetComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void Update()
    {


        #region Fishing

        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(transform.parent == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Hit");
                    transform.position = hit.point;
                    isCast = true;
                }
            }
            else
            {
                isCast = false;
            }

        }

        if (transform.parent != null)
        {
            rbParent = transform.parent.GetComponent<Rigidbody>();

            if (Input.GetMouseButtonDown(1))
            {
               rbParent.isKinematic = true;
            }
            else
            {
                rbParent.isKinematic = false;
            }
            if (rbParent.isKinematic == false)
            {
                Vector3 currentPos = pullGoal.transform.position - transform.position;
                rbParent.velocity = currentPos.normalized * forceToPull;

            }
        }
        #endregion
    }

    bool isMoving()
    {
        if(rb.velocity.magnitude > 1f)
        {
            return true;
        }
        return false;
    }

}
