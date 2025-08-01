using UnityEngine;
using System.Collections.Generic;

public class Recorder : MonoBehaviour
{
    public GameObject clonePrefab;
    public KeyCode recordKey = KeyCode.E;
    public KeyCode playKey = KeyCode.Q;

    private List<TimePoint> recording;
    private bool isRecording = false;
    private float recordingStartTime;
    private bool canSpawn = false;

    private Transform currSpawnPad;

    public static List<GameObject> activeClones = new List<GameObject>();

    void Update()
    {
        if (Input.GetKeyDown(recordKey))
        {
            if (!isRecording && canSpawn)
            {
                StartRecording();
            }
            else if (isRecording)
            {
                StopRecording();
            }
        }

        if (Input.GetKeyDown(playKey) && canSpawn && !isRecording)
        {
            ActivateCurrentClones();
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
        ActivateCurrentClones();
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
        Playback playback = clone.GetComponent<Playback>();
        playback.Init(recording);
        playback.Despawn();
        activeClones.Add(clone);
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

    public void ActivateCurrentClones()
    {
        foreach (GameObject clone in activeClones)
        {
            Playback playback = clone.GetComponent<Playback>();
            if (playback != null)
            {
                playback.Spawn();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Clone")
        {
            GetComponent<PlayerController>().Respawn(currSpawnPad.position + new Vector3(0f, 2f, 0f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SpawnPad")
        {
            canSpawn = true;
            currSpawnPad = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "SpawnPad")
        {
            canSpawn = false;
        }
    }
}