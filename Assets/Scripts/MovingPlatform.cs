using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Travel")]
    [Tooltip("Target position is start + localOffset")]
    public Vector3 localOffset = new Vector3(0f, 4f, 0f);
    public float speed = 2f;

    private Vector3 startPos;
    private Vector3 endPos;
    private bool goUp = false;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        startPos = transform.position;
        endPos = startPos + transform.TransformVector(localOffset);
    }

    public void SetRaised(bool raised)
    {
        goUp = raised;
    }

    void FixedUpdate()
    {
        Vector3 target = goUp ? endPos : startPos;

        Vector3 next = Vector3.MoveTowards(rb.position,
                                            target,
                                            speed * Time.fixedDeltaTime);
        rb.MovePosition(next);
    }
}