using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Recorder : MonoBehaviour
{
    public GameObject clonePrefab;
    public KeyCode recordKey = KeyCode.E;
    public KeyCode playKey = KeyCode.Q;
    public KeyCode resetKey = KeyCode.R;
    public KeyCode interactKey = KeyCode.F;

    private List<TimePoint> recording;
    private bool isRecording = false;
    private float recordingStartTime;
    private bool canSpawn = false;

    private Transform currSpawnPad;
    private Transform currLever;

    public static List<GameObject> activeClones = new List<GameObject>();

    public float maxTime = 60f;
    private float currTime = 0f;

    public TextMeshProUGUI timer;

    public enum CloneSize
    {
        Small,
        Medium,
        Large
    };

    private CloneSize cloneSize = CloneSize.Medium;

    void Update()
    {
        if (isRecording)
        {
            currTime -= Time.deltaTime;
        }
        timer.text = currTime.ToString("#.00");

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

        if (Input.GetKeyDown(playKey))
        {
            if (!isRecording && canSpawn)
            {
                ActivateCurrentClones();
            }
        }

        if (Input.GetKeyDown(resetKey))
        {
            if (activeClones.Count > 0)
            {
                currTime = maxTime;
                ResetAllClones();
                GetComponent<PlayerController>().Respawn(currSpawnPad.position + new Vector3(0f, 2f, 0f));
            }
        }

        if(Input.GetKeyDown(interactKey))
        {
            if (currLever != null)
            {
                currLever.GetComponent<LeverEvent>().LeverOn();
            }
        }

        if (isRecording && currTime <= 0)
        {
            StopRecording();
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
                playback.Spawn(cloneSize);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SpawnPad" || other.tag == "SmallSpawnPad" || other.tag == "LargeSpawnPad")
        {
            if (isRecording)
            {
                StopRecording();
            }
            if (currSpawnPad != other.transform)
            {
                ResetAllClones();
                currSpawnPad = other.transform;
                currTime = maxTime;

                if (other.tag == "SmallSpawnPad")
                {
                    cloneSize = CloneSize.Small;
                }
                else if (other.tag == "SpawnPad")
                {
                    cloneSize = CloneSize.Medium;
                }
                else if (other.tag == "LargeSpawnPad")
                {
                    cloneSize = CloneSize.Large;
                }
            }
            canSpawn = true;
        }

        if (other.tag == "Button")
        {
            other.GetComponent<ButtonEvent>().ActivateButton(cloneSize);
        }

        if (other.tag == "Lever")
        {
            currLever = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "SmallSpawnPad")
        {
            canSpawn = false;
        }
        else if (other.tag == "SpawnPad")
        {
            canSpawn = false;
        }
        else if (other.tag == "LargeSpawnPad")
        {
            canSpawn = false;
        }

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