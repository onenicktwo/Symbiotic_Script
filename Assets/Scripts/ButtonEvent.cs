using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEvent : MonoBehaviour
{
    public GameObject door;

    public Recorder.CloneSize cloneSize;

    private List<Recorder.CloneSize> objectsOnButton = new List<Recorder.CloneSize>();

    public void ActivateButton(Recorder.CloneSize size)
    {
        if(size >= cloneSize)
        {
            objectsOnButton.Add(size);
            door.SetActive(false);
        }
    }

    public void DeactivateButton(Recorder.CloneSize size)
    {
        if (size >= cloneSize)
        {
            objectsOnButton.Remove(size);
            if(objectsOnButton.Count <= 0 )
            {
                door.SetActive(true);
            }
        }
    }
}
