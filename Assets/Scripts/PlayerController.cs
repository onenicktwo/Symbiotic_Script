using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cam;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSmoothTime = 0.1f;
    [SerializeField] private float airSpeed = 6f;
    [SerializeField] private float airAcceleration = 20f;
    [SerializeField] private float airBraking = 8f;

    [Header("Jump / Gravity")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float gravityMultiplier = 3f;
    [SerializeField] private LayerMask groundMask;

    private Rigidbody rb;
    private CapsuleCollider col;
    private Vector2 input;
    private float currentYawVelocity;
    private float verticalSpeed;
    private bool isGrounded;
    private bool hitCeiling;

    public GameObject small;
    public GameObject medium;
    public GameObject large;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        rb.freezeRotation = true;
        rb.useGravity = false;
    }

    private void Update()
    {
        ReadInput();
        HandleJumpInput();
    }

    private void FixedUpdate()
    {
        GroundCheck();
        CeilingCheck();
        ApplyMovement();
    }
    
    public void BecomeSmall()
    {
        col.center = new Vector3(0f, -0.5f, 0f);
        col.radius = 0.5f;
        col.height = 1f;
        small.SetActive(true);
        medium.SetActive(false);
        large.SetActive(false);

        jumpForce = 25f;
        moveSpeed = 12f;
    }

    public void BecomeMedium()
    {
        col.center = new Vector3(0f, 0f, 0f);
        col.radius = 0.5f;
        col.height = 2f;
        small.SetActive(false);
        medium.SetActive(true);
        large.SetActive(false);

        jumpForce = 15f;
        moveSpeed = 7f;
    }
    public void BecomeLarge()
    {
        col.center = new Vector3(0f, 0f, 0f);
        col.radius = 1f;
        col.height = 2f;
        small.SetActive(false);
        medium.SetActive(false);
        large.SetActive(true);

        jumpForce = 5f;
        moveSpeed = 5f;
    }

    private void ReadInput()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"),
                            Input.GetAxisRaw("Vertical"));
    }

    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
            verticalSpeed = jumpForce;
    }

    public void Respawn(Vector3 pos)
    {
        rb.velocity = Vector3.zero;
        transform.position = pos;
    }

    private void ApplyMovement()
    {
        Vector3 camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(cam.right, new Vector3(1, 0, 1)).normalized;
        Vector3 inputDir = (camForward * input.y + camRight * input.x).normalized;

        if (inputDir.sqrMagnitude > 0.001f)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg;
            float smoothed = Mathf.SmoothDampAngle(transform.eulerAngles.y,
                                                      targetAngle,
                                                      ref currentYawVelocity,
                                                      rotationSmoothTime);
            rb.MoveRotation(Quaternion.Euler(0f, smoothed, 0f));
        }

        Vector3 horizVel = rb.velocity;
        horizVel.y = 0f;

        if (isGrounded)
        {
            Vector3 desired = inputDir * moveSpeed;
            horizVel = desired;
        }
        else
        {
            if (inputDir.sqrMagnitude > 0.001f)
            {
                Vector3 desiredDir = inputDir;
                Vector3 desiredVel = desiredDir * airSpeed;
                Vector3 velocityDiff = desiredVel - horizVel;

                Vector3 accel = Vector3.Project(velocityDiff, desiredDir) * airAcceleration;
                Vector3 brake = Vector3.ProjectOnPlane(velocityDiff, desiredDir).normalized
                                 * airBraking * inputDir.magnitude;

                rb.AddForce((accel + brake), ForceMode.Acceleration);

                horizVel = rb.velocity; horizVel.y = 0f;
                horizVel = Vector3.ClampMagnitude(horizVel, airSpeed);
            }
            else
            {
                horizVel = Vector3.MoveTowards(horizVel, Vector3.zero, airBraking * Time.fixedDeltaTime);
            }
        }

        verticalSpeed += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
        if (isGrounded && verticalSpeed < 0) verticalSpeed = 0f;
        if (hitCeiling && verticalSpeed > 0f) verticalSpeed = 0f;

        rb.velocity = new Vector3(horizVel.x, verticalSpeed, horizVel.z);
    }

    private void GroundCheck()
    {
        float sphereRadius = col.radius * 0.9f;
        Vector3 bottomCenter = col.bounds.center + Vector3.down *
                               (col.bounds.extents.y - sphereRadius + 0.05f);

        isGrounded = Physics.CheckSphere(bottomCenter, sphereRadius, groundMask);
    }

    private void CeilingCheck()
    {
        float sphereRadius = col.radius * 0.9f;
        Vector3 topCenter = col.bounds.center + Vector3.up *
                            (col.bounds.extents.y - sphereRadius + 0.05f);

        hitCeiling = Physics.CheckSphere(topCenter, sphereRadius, groundMask);
    }

    private void OnDrawGizmosSelected()
    {
        if (col == null) col = GetComponent<CapsuleCollider>();
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.05f +
                              Vector3.down * ((col.height) - col.radius + 0.2f),
                              col.radius * 0.9f);
    }
}