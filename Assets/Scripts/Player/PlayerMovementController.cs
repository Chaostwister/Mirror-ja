using Mirror;
using UnityEngine;

[RequireComponent(typeof(PlayerManager), typeof(Rigidbody))]
public class PlayerMovementController : NetworkBehaviour
{
    private PlayerManager manager => GetComponent<PlayerManager>();
    
    private Rigidbody rb => GetComponent<Rigidbody>();
    private Transform cam => manager.Cam;

    [SerializeField] private float forwardMovementAccel;
    [SerializeField] private float sidewaysMovementAccel;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float sprintMovementMultiplier = 2;

    private float _mouseX;
    private float _mouseY;

    [SerializeField] private float sens = 100;
    [SerializeField] private Vector2 yClamp;
    
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
}
