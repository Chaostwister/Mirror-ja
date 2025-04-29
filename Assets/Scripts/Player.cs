using System;
using System.Runtime.InteropServices;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using static Helper;

[RequireComponent(typeof(Rigidbody))]
public class Player : NetworkBehaviour
{
    private Rigidbody rb => GetComponent<Rigidbody>();
    [SerializeField] private Transform cam;

    [SerializeField] private float forwardMovementAccel;
    [SerializeField] private float sidewaysMovementAccel;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float sprintMovementMultiplier = 2;

    private float _mouseX;
    private float _mouseY;

    [SerializeField] private float sens = 100;
    [SerializeField] private Vector2 yClamp;

    [SerializeField] private GameObject item;

    public bool inUI;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            cam.gameObject.SetActive(false);
            enabled = false;
        }

        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        TempHandleUi();


        if (item == null && Input.GetMouseButtonDown(0) && Physics.Raycast(cam.position, cam.forward, out var hit))
        {
            hit.transform.TryGetComponent(out Item item);
            if (item == null) return;

            NetworkServer.Destroy(item.gameObject);
        }
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
        Camera();
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
            print("not called on server????");
            return;
        }

        obj.RemoveClientAuthority();
        obj.AssignClientAuthority(connectionToClient);
        print(obj.connectionToClient);
    }
}