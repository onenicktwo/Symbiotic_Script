using UnityEngine;

public struct TimePoint
{
    public float timeStamp;
    public Vector3 position;
    public Quaternion rotation;

    public TimePoint(float time, Vector3 pos, Quaternion rot)
    {
        timeStamp = time;
        position = pos;
        rotation = rot;
    }
}