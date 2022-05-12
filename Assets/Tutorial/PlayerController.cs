using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // A value multiplied by mouse input to get total view rotation
    public float viewRotationSensitivity = 4;

    // A value modified by move direction to get total velocity
    public float moveSpeed = 4;

    // A reference to the Camera
    private Camera m_camera;

    // A reference to the rigidbody
    private Rigidbody m_rigidbody;

    #region Functions
    private void Handle_ViewRotation()
    {
        // Get mouse movement horizontally and vertically
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        // Apply sensitivity to get final rotation
        float rotationX = mouseX * viewRotationSensitivity;
        float rotationY = mouseY * viewRotationSensitivity;

        // Rotate the body
        transform.Rotate(Vector3.up * rotationX);

        // Add rotation to camera (multiply by -1 to change from counter-clockwise to clockwise)
        var euler = m_camera.transform.localEulerAngles;
        euler.x += rotationY * -1;

        // Clamp the rotation to be between[-90, 90] (so you can't look upside down)
        if (euler.x > 180) euler.x -= 360;
        if (euler.x < -180) euler.x += 360;
        euler.x = Mathf.Clamp(euler.x, -90, 90);

        // Apply rotation
        m_camera.transform.localEulerAngles = new Vector3(euler.x, euler.y, euler.z);
    }
    private void Handle_Move()
    {
        // Get ArrowKey / WASD / Controller movement
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // Turn the inputs into a vector of magnitude 1
        Vector3 movement = new Vector3(moveX, 0, moveZ).normalized;

        // Apply speed
        movement *= moveSpeed;

        // Transform from local space to world space
        movement = transform.TransformVector(movement);

        // Prevent over-writing of gravity
        movement.y = m_rigidbody.velocity.y;

        // Apply movement as velocity (one of two ways)
        m_rigidbody.velocity = movement;
    }
    #endregion

    #region Events
    // This function is called on startup, when the Component is first spawned
    private void Awake()
    {
        m_camera = GetComponentInChildren<Camera>(); // Find the camera in the Player object and store a reference to it
        m_rigidbody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        Cursor.visible = false; // Hide the cursor
    }

    // This function is called once every frame
    private void Update()
    {
        Handle_ViewRotation();
    }

    // This function is called at a fixed interval (50 times / sec by default)
    private void FixedUpdate()
    {
        Handle_Move();
    }
    #endregion
}
