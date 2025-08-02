using UnityEngine;

public struct TimePoint
{
    public float timeStamp;
    public Vector3 position;
    public Quaternion rotation;
    public bool interact;

    public TimePoint(float time, Vector3 pos, Quaternion rot, bool act)
    {
        timeStamp = time;
        position = pos;
        rotation = rot;
        interact = act;
    }
}