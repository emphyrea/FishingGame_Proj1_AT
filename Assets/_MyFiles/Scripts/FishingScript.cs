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

    Rigidbody rbParent;

    private GameObject fish;

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

        if (transform.parent != null && isCast)
        {
            rbParent = transform.parent.parent.GetComponent<Rigidbody>(); //fish rigidbody
            GetComponent<Collider>().enabled = false;

            if (Input.GetMouseButton(1))
            {
               rbParent.isKinematic = false; //use physics
                if (Vector3.Distance(transform.parent.position, pullGoal.transform.position) > 1) //if far enough away
                {
                    rbParent.transform.LookAt(pullGoal.transform);
                    rbParent.AddRelativeForce(Vector3.forward * forceToPull, ForceMode.Impulse);
                }
                else if(Vector3.Distance(transform.parent.position, pullGoal.transform.position) <= 1) // if close enough to collect
                {
                    fish = transform.parent.parent.gameObject;
                    FishType fishtype = fish.GetComponent<FishType>();
                    if (fishtype.firstCatch == true)
                    {
                        fishtype.firstCatch = false;
                        CollectionScript.Instance.OnCollect(fishtype);
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
            else
            {
                rbParent.isKinematic = true;
            }

        }
        #endregion
    }


}
