using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public InputAction MoveAction;

    public float walkSpeed = 3f;
    public float turnSpeed = 360f; // degrees per second
    public float inputDeadzone = 0.02f;

    Rigidbody m_Rigidbody;
    Animator m_Animator;

    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = GetComponentInChildren<Animator>(); // safe if Animator is on child
    }

    void OnEnable() => MoveAction.Enable();
    void OnDisable() => MoveAction.Disable();

    void FixedUpdate()
    {
        Vector2 input = MoveAction.ReadValue<Vector2>();

        // Build movement vector from input
        m_Movement.Set(input.x, 0f, input.y);

        // Deadzone to stop sliding/drift when no keys are pressed
        if (m_Movement.sqrMagnitude < inputDeadzone * inputDeadzone)
        {
            if (m_Animator) m_Animator.SetBool("IsWalking", false);
            return;
        }

        m_Movement.Normalize();

        // Optional: update animation
        if (m_Animator) m_Animator.SetBool("IsWalking", true);

        // Rotate toward movement direction
        Vector3 desiredForward = Vector3.RotateTowards(
            transform.forward,
            m_Movement,
            Mathf.Deg2Rad * turnSpeed * Time.fixedDeltaTime, // RotateTowards uses radians
            0f
        );

        m_Rotation = Quaternion.LookRotation(desiredForward);
        m_Rigidbody.MoveRotation(m_Rotation);

        // Move using the movement vector (NOT transform.forward)
        m_Rigidbody.MovePosition(
            m_Rigidbody.position + m_Movement * walkSpeed * Time.fixedDeltaTime
        );
    }
}
