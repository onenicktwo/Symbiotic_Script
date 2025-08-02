using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEvent : MonoBehaviour
{
    public GameObject door;

    public void ActivateButton()
    {
        door.SetActive(false);
    }

    public void DeactivateButton()
    {
        door.SetActive(true);
    }
}
