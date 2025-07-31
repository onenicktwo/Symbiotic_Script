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

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void StartPlayback(List<TimePoint> rec)
    {
        recording = rec;
        isPlaying = true;
        playbackStartTime = Time.time;
        playbackIndex = 0;
    }

    void FixedUpdate()
    {
        if (!isPlaying) return;

        float timeSincePlaybackStarted = Time.time - playbackStartTime;

        while (playbackIndex < recording.Count - 1 && recording[playbackIndex + 1].timeStamp <= timeSincePlaybackStarted)
        {
            playbackIndex++;
        }

        if (playbackIndex >= recording.Count - 1)
        {
            playbackIndex = 0;
            playbackStartTime = Time.time;
        }

        rb.MovePosition(recording[playbackIndex].position);
        rb.MoveRotation(recording[playbackIndex].rotation);
    }
}