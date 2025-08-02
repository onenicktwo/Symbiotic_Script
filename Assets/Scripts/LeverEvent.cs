using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverEvent : MonoBehaviour
{
    public GameObject door;
    public float waitTime = 3f;
    private bool isActivated = false;

    public void LeverOn()
    {
        StartCoroutine(Lever());
    }

    private IEnumerator Lever()
    {
        if (!isActivated)
        {
            isActivated = true;
            door.SetActive(false);
            yield return new WaitForSeconds(waitTime);
            door.SetActive(true);
            isActivated = false;
        }
    }
}
