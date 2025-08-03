using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ButtonEvent : MonoBehaviour
{
    [Header("What gets triggered")]
    public GameObject door;
    public MovingPlatform movingPlatform;

    [Header("Size requirements")]
    public Recorder.CloneSize cloneSize;

    private readonly List<Recorder.CloneSize> objectsOnButton =
        new List<Recorder.CloneSize>();

    public void ActivateButton(Recorder.CloneSize size)
    {
        if (size < cloneSize) return;

        if (!objectsOnButton.Contains(size))
            objectsOnButton.Add(size);

        if (movingPlatform)
            movingPlatform.SetRaised(true);
        else if (door)
            door.SetActive(false);
    }

    public void DeactivateButton(Recorder.CloneSize size)
    {
        if (size < cloneSize) return;

        objectsOnButton.Remove(size);

        if (objectsOnButton.Count == 0)      // nobody left – release the button
        {
            if (movingPlatform)
                movingPlatform.SetRaised(false);
            else if (door)
                door.SetActive(true);
        }
    }
}
