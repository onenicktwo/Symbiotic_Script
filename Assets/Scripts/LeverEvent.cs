using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LeverEvent : MonoBehaviour
{
    public GameObject door;
    public float waitTime = 6f;
    private bool isActivated = false;
    public MovingPlatform movingPlatform;

    public void LeverOn()
    {
        if (door) 
            StartCoroutine(Lever());
        if (movingPlatform)
            movingPlatform.SetRaised(true);
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
