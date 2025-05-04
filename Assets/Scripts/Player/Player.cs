using System;
using System.Runtime.InteropServices;
using Mirror;
using Telepathy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using static Helper;

[RequireComponent(typeof(Rigidbody))]
public class Player : NetworkBehaviour
{
    private Rigidbody rb => GetComponent<Rigidbody>();
    [SerializeField] private Transform cam;
    public Transform Cam => cam;

    [SerializeField] private float forwardMovementAccel;
    [SerializeField] private float sidewaysMovementAccel;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float sprintMovementMultiplier = 2;

    private float _mouseX;
    private float _mouseY;

    [SerializeField] private float sens = 100;
    [SerializeField] private Vector2 yClamp;

    [SyncVar] [SerializeField] private string itemID = string.Empty;

    public bool inUI;

    private void Start()
    {
        if (isLocalPlayer)
        {
            cam.GetComponent<Camera>().enabled = true;
            cam.GetComponent<AudioListener>().enabled = true;
        }

        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    private void Update()
    {
        //print($"{(isServer? "Server":"Client")} : {itemData}");

        if (!isLocalPlayer) return;

        TempHandleUi();

        if (Input.GetKeyDown(KeyCode.G))
        {
            CmdSpawnItem("item sphere");
        }
    }

    private void LateUpdate()
    {
        if (!isLocalPlayer) return;
        Camera();
    }

    private void TempHandleUi()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            inUI = true;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (inUI)
            {
                inUI = false;
                return;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        Movement();
    }

    private void Movement()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        var movementVector = horizontal * sidewaysMovementAccel * transform.right +
                             vertical * forwardMovementAccel *
                             (Input.GetKey(KeyCode.LeftShift) ? sprintMovementMultiplier : 1) * transform.forward;

        if (movementVector != Vector3.zero)
        {
            rb.AddForce(movementVector, ForceMode.Acceleration);
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
        else
        {
            if (rb.linearVelocity.magnitude < forwardMovementAccel)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * 0;
            }
            else rb.AddForce(rb.linearVelocity * -forwardMovementAccel, ForceMode.Acceleration);
        }
    }


    private void Camera()
    {
        _mouseX += Input.GetAxisRaw("Mouse X") * 0.05f * sens;
        _mouseY -= Input.GetAxisRaw("Mouse Y") * 0.05f * sens;

        _mouseY = Mathf.Clamp(_mouseY, yClamp.x, yClamp.y);

        transform.rotation =
            Quaternion.Euler(transform.rotation.eulerAngles.x, _mouseX, transform.rotation.eulerAngles.z);
        cam.transform.rotation =
            Quaternion.Euler(_mouseY, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isLocalPlayer) return;

        collision.transform.TryGetComponent(out Item item);
        collision.transform.TryGetComponent(out NetworkIdentity itemNetID);
        if (item == null || itemNetID == null) return;

        CmdAssignAuth(itemNetID);
    }

    [Command]
    public void CmdAssignAuth(NetworkIdentity obj)
    {
        print($"CmdAssignAuth called. isClient: {isClient}, isServer: {isServer}");
        if (!isServer)
        {
            Debug.LogWarning("not called on server????");
            return;
        } // if cmd isn't called on server return

        if (obj == null)
        {
            Debug.LogWarning("Obj null in Assign Auth (probably doesnt matter tho)");
            return;
        }

        //Remove old auth and add auth for calling client
        obj.RemoveClientAuthority();
        obj.AssignClientAuthority(connectionToClient);
        print(obj.connectionToClient);
    }

    [Command]
    public void CmdSpawnItem(string id)
    {
        NetworkInstantiate(ItemDatabase.GetItem(id).prefab, connectionToClient);
    }
}