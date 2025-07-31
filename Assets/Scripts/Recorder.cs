using UnityEngine;
using System.Collections.Generic;

public class Recorder : MonoBehaviour
{
    public GameObject clonePrefab;
    public KeyCode recordKey = KeyCode.E;

    private List<TimePoint> recording;
    private bool isRecording = false;
    private float recordingStartTime;

    public static List<GameObject> activeClones = new List<GameObject>();

    void Update()
    {
        if (Input.GetKeyDown(recordKey))
        {
            if (isRecording)
            {
                StopRecording();
            }
            else
            {
                StartRecording();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetAllClones();
        }
    }

    void FixedUpdate()
    {
        if (isRecording)
        {
            float timeSinceRecordingStarted = Time.time - recordingStartTime;
            recording.Add(new TimePoint(timeSinceRecordingStarted, transform.position, transform.rotation));
        }
    }

    void StartRecording()
    {
        isRecording = true;
        recording = new List<TimePoint>();
        recordingStartTime = Time.time;
        Debug.Log("Started Recording!");
    }

    void StopRecording()
    {
        isRecording = false;
        Debug.Log("Stopped Recording! Spawning Clone.");
        SpawnClone();
    }

    void SpawnClone()
    {
        if (clonePrefab == null || recording.Count == 0)
        {
            Debug.LogError("Clone Prefab is not set or recording is empty!");
            return;
        }

        Vector3 startPos = recording[0].position;
        Quaternion startRot = recording[0].rotation;

        GameObject clone = Instantiate(clonePrefab, startPos, startRot);
        activeClones.Add(clone);

        Playback playback = clone.GetComponent<Playback>();
        if (playback != null)
        {
            playback.StartPlayback(new List<TimePoint>(recording));
        }
    }

    public void ResetAllClones()
    {
        Debug.Log("Resetting all clones.");
        foreach (GameObject clone in activeClones)
        {
            Destroy(clone);
        }
        activeClones.Clear();
    }
}