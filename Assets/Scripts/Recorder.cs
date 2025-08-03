using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public GameObject[] doors;

    public float maxTime = 60f;
    private float currTime = 0f;

    public TextMeshProUGUI timer;

    private PlayerController controller;

    public enum CloneSize
    {
        Small,
        Medium,
        Large
    };

    private CloneSize cloneSize = CloneSize.Medium;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        currTime = maxTime;
    }

    void Update()
    {
        if (isRecording)
        {
            currTime -= Time.deltaTime;
            float t = Time.time - recordingStartTime;
            bool interactPressed = Input.GetKeyDown(interactKey);

            if (interactPressed) Debug.Log("Interacted");
            recording.Add(new TimePoint(t, transform.position, transform.rotation, interactPressed));
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
                controller.BecomeMedium();
                ActivateCurrentClones();
            }
        }

        if (Input.GetKeyDown(resetKey) && !isRecording)
        {
            if (activeClones.Count > 0)
            {
                controller.Respawn(currSpawnPad.position + new Vector3(0f, 2f, 0f));
                currTime = maxTime;
                ResetAllDoors();
                ResetAllClones();
                controller.BecomeMedium();
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

    void StartRecording()
    {
        switch (cloneSize)
        {
            case CloneSize.Small:
                controller.BecomeSmall(); break;
            case CloneSize.Medium:
                controller.BecomeMedium(); break;
            case CloneSize.Large:
                controller.BecomeLarge(); break;
        }
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
        playback.Init(recording, cloneSize);
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

    void ResetAllDoors()
    {
        foreach (GameObject door in doors)
        {
            door.SetActive(true);
        }
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

    private void OnTriggerEnter(Collider other)
    {
        if ((other.tag == "SpawnPad" || other.tag == "SmallSpawnPad" || other.tag == "LargeSpawnPad") && !isRecording)
        {
            if (currSpawnPad != other.transform)
            {
                currSpawnPad = other.transform;

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

        if (other.tag == "EndPad")
        {
            SceneManager.LoadScene("GUI");
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
        if (other.tag == "SpawnPad")
        {
            canSpawn = false;
        }
        if (other.tag == "LargeSpawnPad")
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