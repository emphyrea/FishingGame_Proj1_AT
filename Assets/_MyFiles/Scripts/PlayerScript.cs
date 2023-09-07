using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [Header("Movement and Cam Settings")]
    public Camera playerCam;
    public float walkSpeed = 6f;

    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    public float curSpeedX = 0;
    public float curSpeedY = 0;

    Vector3 moveDir = Vector3.zero;
    float rotX = 0f;

    public bool canMove = true;

    CharacterController charController;

    // Start is called before the first frame update
    void Start()
    {
        charController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 Forward = transform.TransformDirection(Vector3.forward);
        Vector3 Right = transform.TransformDirection(Vector3.right);

        float moveDirY = moveDir.y;


        if(canMove)
        {
            curSpeedX = walkSpeed * Input.GetAxis("Vertical");
            curSpeedY = walkSpeed * Input.GetAxis("Horizontal");

            rotX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotX = Mathf.Clamp(rotX, -lookXLimit, lookXLimit);
            playerCam.transform.localRotation = Quaternion.Euler(rotX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        else
        {
            curSpeedX = 0;
            curSpeedY = 0;
        }
       
        moveDir = (Forward * curSpeedX) + (Right * curSpeedY);

        charController.Move(moveDir * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.I) && CollectionScript.Instance.CollectionMenu.enabled == false)
        {
            for (int i = 0; i < CollectionScript.Instance.CollectionMenu.transform.childCount; i++)
            {
                CollectionScript.Instance.fishImages[i].enabled = true;
            }
            CollectionScript.Instance.CollectionMenu.enabled = true;
        }
        else if (Input.GetKeyDown(KeyCode.I) && CollectionScript.Instance.CollectionMenu.enabled == true)
        {
            for (int i = 0; i < CollectionScript.Instance.CollectionMenu.transform.childCount; i++)
            {
                CollectionScript.Instance.fishImages[i].enabled = false;
            }
            CollectionScript.Instance.CollectionMenu.enabled = false;
        }
    }
}
