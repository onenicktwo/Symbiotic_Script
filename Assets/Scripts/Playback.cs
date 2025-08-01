using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Playback : MonoBehaviour
{
    private List<TimePoint> recording;
    private int playbackIndex = 0;
    private bool isPlaying = false;
    private float playbackStartTime;

    private Rigidbody rb;
    private int spawnLayer = 9;
    private int despawnLayer = 8;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Init(List<TimePoint> rec)
    {
        recording = rec;
    }

    public void Spawn()
    {
        if (isPlaying) return;
        gameObject.layer = spawnLayer;
        transform.GetChild(0).gameObject.layer = spawnLayer;
        transform.GetChild(1).gameObject.layer = spawnLayer;
        isPlaying = true;
        playbackStartTime = Time.time;
        playbackIndex = 0;
    }

    public void Despawn()
    {
        gameObject.layer = despawnLayer;
        transform.GetChild(0).gameObject.layer = despawnLayer;
        transform.GetChild(1).gameObject.layer = despawnLayer;
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

        float simTime = Time.fixedTime - playbackStartTime;

        while (playbackIndex < recording.Count - 1 &&
               recording[playbackIndex + 1].timeStamp <= simTime)
        {
            playbackIndex++;
        }

        if (playbackIndex >= recording.Count - 1)
        {
            Despawn();
            return;
        }

        TimePoint cur = recording[playbackIndex];
        TimePoint nxt = recording[playbackIndex + 1];

        float dt = nxt.timeStamp - cur.timeStamp;
        if (dt <= 0) dt = Time.fixedDeltaTime;

        Vector3 v = (nxt.position - cur.position) / dt;
        rb.velocity = v;

        Quaternion deltaRot = nxt.rotation * Quaternion.Inverse(cur.rotation);
        deltaRot.ToAngleAxis(out float angleDeg, out Vector3 axis);
        if (angleDeg > 180) angleDeg -= 360;              // shortest path
        Vector3 angularVel = axis.normalized
                           * angleDeg * Mathf.Deg2Rad / dt;
        rb.angularVelocity = angularVel;
    }
}