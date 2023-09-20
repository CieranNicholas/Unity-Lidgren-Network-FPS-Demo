using Lidgren.Network;
using System.Collections.Generic;
using UnityEngine;

public class NetworkClientInfo: MonoBehaviour
{
    public static NetworkClientInfo Instance { get; private set; }

    public string LocalPlayerID;
    public Dictionary<string, GameObject> Players = new();

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}
