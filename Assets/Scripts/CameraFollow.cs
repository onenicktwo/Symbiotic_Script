using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetOffset = new Vector3(0, 1.8f, 0);

    [Header("Orbit")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float yawSpeed = 180f;
    [SerializeField] private float pitchSpeed = 180f;
    [SerializeField] private float pitchMin = -30f;
    [SerializeField] private float pitchMax = 75f;

    [Header("Smoothing")]
    [SerializeField] private float positionSmooth = 0.05f;

    private float yaw;
    private float pitch = 20f;
    private Vector3 currentVel;

    private void Start()
    {
        if (target == null) enabled = false;

        // Start camera yaw aligned with player forward
        yaw = target.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        yaw += Input.GetAxis("Mouse X") * yawSpeed * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * pitchSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pivot = target.position + targetOffset;
        Vector3 wanted = pivot - rot * Vector3.forward * distance;

        transform.position = Vector3.SmoothDamp(transform.position,
                                                wanted,
                                                ref currentVel,
                                                positionSmooth);
        transform.rotation = rot;
    }
}