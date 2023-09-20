using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InputPayload
{
    public int tick;
    public Vector3 inputVector;
    public string player;
}

public struct StatePayload
{
    public int tick;
    public Vector3 position;
    public string player;
}

public class Prediction : MonoBehaviour
{
    public float timer;
    public int currentTick;
    public float minTimeBetweenTicks;
    public const float SERVER_TICK_RATE = 30f;
    public const int BUFFER_SIZE = 1024;

}
