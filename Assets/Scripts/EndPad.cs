using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPad : MonoBehaviour
{
    public void Activate()
    {
        SceneManager.LoadScene("GUI");
    }
}
