using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform groundCheck;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundMask;

    [Header("Input")]
    [SerializeField] private KeyCode runKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    private Rigidbody rb;

    private Vector2 moveInput;
    private bool jumpRequested;
    private bool isRunning;
    private bool controlsEnabled;

    public bool ControlsEnabled => controlsEnabled;

    public Transform CameraTarget =>
        cameraTarget != null
            ? cameraTarget
            : transform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (cameraTransform == null)
        {
            Debug.LogError(
                $"{name} necesita una referencia a la Main Camera.",
                this
            );

            enabled = false;
            return;
        }

        if (groundCheck == null)
        {
            Debug.LogError(
                $"{name} necesita un GroundCheck.",
                this
            );

            enabled = false;
        }
    }

    private void Update()
    {
        if (!controlsEnabled)
            return;

        ReadInput();
    }

    private void FixedUpdate()
    {
        if (!controlsEnabled)
            return;

        Move();
        HandleJump();
    }

    private void ReadInput()
    {
        moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        isRunning = Input.GetKey(runKey);

        if (Input.GetKeyDown(jumpKey))
            jumpRequested = true;
    }

    private void Move()
    {
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Ignoramos la inclinación vertical de la cámara
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // WASD relativo a la cámara
        Vector3 moveDirection =
            cameraForward * moveInput.y +
            cameraRight * moveInput.x;

        if (moveDirection.sqrMagnitude > 1f)
            moveDirection.Normalize();

        float currentSpeed =
            isRunning ? runSpeed : walkSpeed;

        Vector3 velocity = rb.linearVelocity;

        velocity.x = moveDirection.x * currentSpeed;
        velocity.z = moveDirection.z * currentSpeed;

        rb.linearVelocity = velocity;

        RotateCharacter(moveDirection);
    }

    private void RotateCharacter(Vector3 moveDirection)
    {
        if (moveDirection.sqrMagnitude <= 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(moveDirection);

        Quaternion newRotation =
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

        rb.MoveRotation(newRotation);
    }

    private void HandleJump()
    {
        if (!jumpRequested)
            return;

        jumpRequested = false;

        if (!IsGrounded())
            return;

        rb.AddForce(
            Vector3.up * jumpForce,
            ForceMode.Impulse
        );
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
    }

    public void SetControlsEnabled(bool value)
    {
        controlsEnabled = value;

        moveInput = Vector2.zero;
        jumpRequested = false;
        isRunning = false;

        if (!value && rb != null)
        {
            Vector3 velocity = rb.linearVelocity;

            // Frenamos el movimiento horizontal
            // pero dejamos que siga actuando la gravedad.
            velocity.x = 0f;
            velocity.z = 0f;

            rb.linearVelocity = velocity;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}