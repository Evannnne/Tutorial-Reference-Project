using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // We're using the Singleton Pattern here
    public static PlayerController Instance { get; private set; }

    // How much health the player has
    public float health = 100;

    // How much damage your gun does
    public float damage = 20;

    // A value multiplied by mouse input to get total view rotation
    public float viewRotationSensitivity = 4;

    // How fast you move
    public float moveSpeed = 4;

    // Multiplies your base move speed when running
    public float sprintModifier = 2;

    // How intensely you jump
    public float jumpVelocity = 10;

    // How hard you shoot
    public float shootingForce = 100;

    // A reference to the Camera
    private Camera m_camera;

    // A reference to the rigidbody
    private Rigidbody m_rigidbody;

    // A reference to the collider
    private CapsuleCollider m_collider;

    // A reference to the gun's animator
    private Animator m_animator;


    #region Functions
    private IEnumerator Handle_Effect_EntityShot(Vector3 point, Color color)
    {
        // Create hit particles
        List<GameObject> spawned = new List<GameObject>();
        for(int i = 0; i < 50; i++)
        {
            GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            particle.transform.position = point;
            particle.AddComponent<Rigidbody>().velocity = Random.onUnitSphere * 1.5f;
            particle.GetComponent<MeshRenderer>().material.color = color;
            Destroy(particle.GetComponent<BoxCollider>());
            spawned.Add(particle);
        }

        // Shrink particles over course of one second
        float t = 0;
        while(t <= 1)
        {
            foreach (var go in spawned) go.transform.localScale = Vector3.one * Mathf.Lerp(0.075f, 0, t);
            t += Time.deltaTime;
            yield return null;
        }

        // Destroy particles
        foreach (var go in spawned) Destroy(go);
    }

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
    private void Handle_Movement()
    {
        // Get ArrowKey / WASD / Controller movement (way 1 of getting input)
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // Turn the inputs into a vector of magnitude 1
        Vector3 movement = new Vector3(moveX, 0, moveZ).normalized;

        // Apply speed
        movement *= moveSpeed;

        // Apply sprint with LShift/RShift (way 2 of getting input)
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) movement *= sprintModifier;

        // Transform from local space to world space
        movement = transform.TransformVector(movement);

        // Prevent over-writing of gravity
        movement.y = m_rigidbody.velocity.y;

        // Apply movement as velocity (one of two ways)
        m_rigidbody.velocity = movement;
    }
    private void Handle_Jumping()
    {
        // If the space key gets pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Find distance to raycast down
            float checkRange = m_collider.bounds.extents.y + 0.25f;

            // If we hit something below us
            if (Physics.Raycast(transform.position, Vector3.down, out _, checkRange))
            {
                // Set upwards velocity to a constant value
                var v = m_rigidbody.velocity;
                v.y = jumpVelocity;
                m_rigidbody.velocity = v;
            }
        }
    }
    private void Handle_Shooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Trigger animations
            m_animator.SetTrigger("Fire");

            // Fire a raycast to see what gets shot
            RaycastHit hit;
            if(Physics.Raycast(m_camera.transform.position, m_camera.transform.forward, out hit, 1000))
            {
                // If the target has a rigidbody, apply impact force from being shot
                if(hit.rigidbody != null) hit.rigidbody.AddForceAtPosition(transform.forward * shootingForce, hit.point, ForceMode.Impulse);

                // If the target is an enemy
                var enemy = hit.collider.gameObject.GetComponent<EnemyBehaviour>();
                if(enemy != null)
                {
                    //Show blood
                    StartCoroutine(Handle_Effect_EntityShot(hit.point, Color.red));
                    enemy.BroadcastMessage("Hit", damage, SendMessageOptions.DontRequireReceiver);
                }
                //Show sparks
                else StartCoroutine(Handle_Effect_EntityShot(hit.point, Color.yellow));
            }
        }
    }
    #endregion

    #region Events
    // This function is called on startup, when the Component is first spawned
    private void Awake()
    {
        Instance = this;
        m_camera = GetComponentInChildren<Camera>(); // Find the camera in the Player object and store a reference to it
        m_rigidbody = GetComponentInChildren<Rigidbody>();
        m_collider = GetComponentInChildren<CapsuleCollider>();
        m_animator = GetComponentInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        Cursor.visible = false; // Hide the cursor
    }

    // This function is called once every frame
    private void Update()
    {
        Handle_ViewRotation();
        Handle_Jumping();
        Handle_Shooting();
    }

    // This function is called at a fixed interval (50 times / sec by default)
    private void FixedUpdate()
    {
        Handle_Movement();
    }

    // This function is an event we defined ourselves, and trigger from other scripts
    private void Hit(float damage)
    {
        health -= damage;
        if(health <= 0) { } // We die!
    }
    #endregion
}
