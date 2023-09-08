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
    void Update()
    {
        #region Fishing
        castAction.performed += ctx => { Cast(ctx); };

        if (transform.parent != null && isCast)
        {
            rbParent = transform.parent.parent.GetComponent<Rigidbody>(); //fish rigidbody
            GetComponent<Collider>().enabled = false;
            pullAction.performed += ctx => { Pull(ctx); };

            pullAction.canceled += ctx => 
            {
                if (transform.parent != null && rbParent != null) { rbParent.isKinematic = true; }
                return;
            }; //otherwise keeps sliding around
        }
        #endregion
    }

    public void OnEnable()
    {
        pullAction.Enable();
        castAction.Enable();
    }

    private void Cast(InputAction.CallbackContext context)
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
                return;
            }
        }

    }

    private void Pull(InputAction.CallbackContext context)
    {
        if (transform.parent == null)
        { return; }

        if(fish == null)
        {
            fish = transform.parent.parent.gameObject;
        }

        if (Vector3.Distance(transform.parent.position, pullGoal.transform.position) > 1) //if far enough away
        {
            if (fish.GetComponent<FishAIScript>().IsResisting() == false)
            {
                rbParent.isKinematic = false; //use physics
                Vector3 direction = (transform.parent.position - pullGoal.transform.position).normalized; ; //from pull from hookLoc to goal
                direction.y = 0;
                rbParent.AddRelativeForce(direction * forceToPull, ForceMode.Impulse);
            }
            else
            {
                fish.GetComponent<FishAIScript>().StartSnapTimer();
            }
        }
        else if (Vector3.Distance(transform.parent.position, pullGoal.transform.position) <= 1) // if close enough to collect
        {
            FishType fishtype = fish.GetComponent<FishType>();
            Debug.Log($"is {fishtype.firstCatch}");
            Debug.Log($"is {fishtype.fishName}");
            if (fishtype.firstCatch == true)
            {
                if (onCollectSender != null)
                {
                    Debug.Log("Sending Info..");
                    onCollectSender(fishtype);
                }
            }
            GameObject hookLoc = transform.parent.gameObject; //put in capsule to safely delete

            transform.parent = null;
            transform.parent.parent = null;

            Destroy(hookLoc);
            Destroy(fish);
            isCast = false;
            transform.rotation = Quaternion.identity;
            GetComponent<Collider>().enabled = true;

            return;
        }
    }
}
