using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UIElements;
using static Recorder;

[RequireComponent(typeof(Rigidbody))]
public class Playback : MonoBehaviour
{
    private List<TimePoint> recording;
    private int playbackIndex = 0;
    private bool isPlaying = false;
    private float playbackStartTime;

    private Rigidbody rb;
    private CapsuleCollider col;
    private int spawnLayer = 9;
    private int despawnLayer = 8;

    private CloneSize cloneSize = CloneSize.Medium;

    public GameObject small;
    public GameObject medium;
    public GameObject large;

    private Transform currLever;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();  
    }

    public void Init(List<TimePoint> rec, CloneSize cloneSize)
    {
        recording = rec;
        this.cloneSize = cloneSize;
    }

    public void Spawn()
    {
        if (isPlaying) return;
        switch (cloneSize)
        {
            case CloneSize.Small:
                BecomeSmall(); break;
            case CloneSize.Medium:
                BecomeMedium(); break;   
            case CloneSize.Large:
                BecomeLarge(); break;
        }
        gameObject.layer = spawnLayer;
        isPlaying = true;
        playbackStartTime = Time.time;
        playbackIndex = 0;
    }

    public void Despawn()
    {
        gameObject.layer = despawnLayer;

        small.SetActive(false);
        medium.SetActive(false);
        large.SetActive(false);

        Vector3 startPos = recording[0].position;
        Quaternion startRot = recording[0].rotation;
        rb.velocity = Vector3.zero;
        rb.rotation = startRot;
        transform.position = startPos;
        transform.rotation = startRot;
        isPlaying =false;
    }

    void FixedUpdate()
    {
        if (!isPlaying) return;

        float simTime = Time.time - playbackStartTime;

        while (playbackIndex < recording.Count - 1 &&
               recording[playbackIndex + 1].timeStamp <= simTime)
        {
            playbackIndex++;

            if (recording[playbackIndex].interact && currLever != null)
            {
                Debug.Log("Clone Interact");
                currLever.GetComponent<LeverEvent>().LeverOn();
            }
        }

        if (playbackIndex >= recording.Count - 1)
        {
            Despawn();
            return;
        }

        TimePoint cur = recording[playbackIndex];
        TimePoint nxt = recording[playbackIndex + 1];

        rb.MovePosition(cur.position);
        rb.MoveRotation(cur.rotation);

        float dt = Mathf.Max(nxt.timeStamp - cur.timeStamp, Time.fixedDeltaTime);

        rb.velocity = (nxt.position - cur.position) / dt;

        Quaternion delta = nxt.rotation * Quaternion.Inverse(cur.rotation);
        delta.ToAngleAxis(out float angDeg, out Vector3 axis);
        if (angDeg > 180f) angDeg -= 360f;
        rb.angularVelocity = axis.normalized * angDeg * Mathf.Deg2Rad / dt;
    }

    public void BecomeSmall()
    {
        col.center = new Vector3(0f, -0.5f, 0f);
        col.radius = 0.5f;
        col.height = 1f;
        small.SetActive(true);
        medium.SetActive(false);
        large.SetActive(false);
    }

    public void BecomeMedium()
    {
        col.center = new Vector3(0f, 0f, 0f);
        col.radius = 0.5f;
        col.height = 2f;
        small.SetActive(false);
        medium.SetActive(true);
        large.SetActive(false);
    }
    public void BecomeLarge()
    {
        col.center = new Vector3(0f, 0f, 0f);
        col.radius = 1f;
        col.height = 2f;
        small.SetActive(false);
        medium.SetActive(false);
        large.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Button")
        {
            other.GetComponent<ButtonEvent>().ActivateButton(cloneSize);
        }

        if (other.tag == "Lever")
        {
            Debug.Log("lever entered");
            currLever = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Button")
        {
            other.GetComponent<ButtonEvent>().DeactivateButton(cloneSize);
        }

        if (other.tag == "Lever")
        {
            currLever = null;
        }
    }
}