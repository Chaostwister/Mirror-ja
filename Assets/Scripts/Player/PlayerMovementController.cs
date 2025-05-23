using System;
using Mirror;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerManager), typeof(Rigidbody))]
public class PlayerMovementController : NetworkBehaviour
{
    private PlayerManager manager => GetComponent<PlayerManager>();

    private Rigidbody rb => GetComponent<Rigidbody>();
    private Transform cam => manager.Cam;

    [SerializeField] private float forwardMovementAccel;
    [SerializeField] private float sidewaysMovementAccel;
    [ReadOnly] [SerializeField] private float curMaxSpeed;
    [SerializeField] private float normalMaxSpeed;
    [SerializeField] private float sprintingMaxSpeed;
    [SerializeField] private float jumpForce;

    [SerializeField] private float mouseX;
    [SerializeField] private float mouseY;

    [SerializeField] private float mouseXLayer;
    [SerializeField] private float mouseYLayer;

    [SerializeField] private float sens = 100;
    [SerializeField] private Vector2 yClamp;

    private void Update()
    {
        if (!isLocalPlayer) return;
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    private void LateUpdate()
    {
        if (!isLocalPlayer) return;
        Camera();
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
                             vertical * forwardMovementAccel * transform.forward;
        

        curMaxSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintingMaxSpeed : normalMaxSpeed;

        if (movementVector != Vector3.zero)
        {
            rb.AddForce(movementVector, ForceMode.Acceleration);
            if (rb.linearVelocity.magnitude > curMaxSpeed)
            {
                var xzLinVelocity = rb.linearVelocity.normalized * curMaxSpeed;
                xzLinVelocity.y = rb.linearVelocity.y;
                rb.linearVelocity = xzLinVelocity;
            }
        }
        else
        {
            var xzVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            if (rb.linearVelocity.magnitude < forwardMovementAccel)
            {
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
            else rb.AddForce(xzVelocity * -forwardMovementAccel, ForceMode.Acceleration);
        }
    }


    public void AddCameraLayer(float x, float y)
    {
        mouseXLayer += x;
        mouseYLayer -= y;
    }
    
    private void Camera()
    {
        mouseX += Input.GetAxisRaw("Mouse X") * 0.05f * sens;
        mouseY -= Input.GetAxisRaw("Mouse Y") * 0.05f * sens;

        mouseY = Mathf.Clamp(mouseY, yClamp.x, yClamp.y);

        transform.rotation =
            Quaternion.Euler(transform.rotation.eulerAngles.x, mouseX + mouseXLayer, transform.rotation.eulerAngles.z);
        cam.transform.rotation =
            Quaternion.Euler(mouseY + mouseYLayer, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        mouseXLayer = 0;
        mouseYLayer = 0;
    }
}