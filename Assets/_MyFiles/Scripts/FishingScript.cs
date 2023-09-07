using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingScript : MonoBehaviour
{

    [Header("Fishing Settings")]

    public bool hasPole = true;
    public bool isCast = false;

    public float forceToPull;
    public Vector3 hitWorldPos;

    public Camera playerCam;
    public GameObject pullGoal;

    Rigidbody rbParent;

    private GameObject fish;
    public delegate void OnCollect(FishType caughtType);
    public static event OnCollect onCollectSender;

    public InputAction pullAction;
    public InputAction castAction;


    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<MeshRenderer>().enabled = true;
        playerCam = Camera.main;
        fish = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        #region Fishing
        //castAction.performed do cast;

        //nesting
        if (transform.parent != null && isCast)
        {
            rbParent = transform.parent.parent.GetComponent<Rigidbody>(); //fish rigidbody
            GetComponent<Collider>().enabled = false;
            if(pullAction.triggered)
            {
                Pull();
            }
            else
            {
                rbParent.isKinematic = true;
            }

        }
        #endregion
    }

    private void Cast()
    {
        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (transform.parent == null)
        {

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit");
                transform.position = hit.point;
                isCast = true;
            }
        }
        isCast = false;
    }

    private void Pull()
    {
        if (fish == null) return;
        fish = transform.parent.parent.gameObject;

        rbParent.isKinematic = false; //use physics
        if (Vector3.Distance(transform.parent.position, pullGoal.transform.position) > 1) //if far enough away
        {
            rbParent.transform.LookAt(pullGoal.transform);
            if (fish.GetComponent<FishAIScript>().IsResisting())
            {
                rbParent.AddRelativeForce(Vector3.forward * forceToPull, ForceMode.Impulse);
            }
        }
        else if (Vector3.Distance(transform.parent.position, pullGoal.transform.position) <= 1) // if close enough to collect
        {

            FishType fishtype = fish.GetComponent<FishType>();
            Debug.Log($"is {fishtype.firstCatch}");
            if (fishtype.firstCatch == true)
            {
                if (onCollectSender != null)
                {
                    Debug.Log("Sending Info..");
                    onCollectSender(fishtype);
                }
            }
            GameObject hookLoc = transform.parent.gameObject; //put in capsule to safely delete

            transform.parent.parent = null;
            transform.parent = null;

            Destroy(fish);
            Destroy(hookLoc);
            isCast = false;
            transform.rotation = Quaternion.identity;
            GetComponent<Collider>().enabled = true;

            return;
        }
    }
}
