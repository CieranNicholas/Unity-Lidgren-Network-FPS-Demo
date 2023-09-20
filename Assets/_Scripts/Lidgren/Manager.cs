using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Manager : MonoBehaviour
{
    [SerializeField]
    private ServerBehaviour serverBehaviour;
    [SerializeField]
    private ClientBehaviour clientBehaviour;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private TextMeshPro textMeshPro;
    [SerializeField]
    private Canvas canvas;

    private void Awake()
    {
        //GameObject.DontDestroyOnLoad(this);
        serverBehaviour = GetComponent<ServerBehaviour>();
        clientBehaviour = GetComponent<ClientBehaviour>();
    }
    public void StartHost()
    {
        serverBehaviour.enabled = true;
        canvas.enabled = false;

        gameObject.AddComponent<PredictionServer>();
    }

    public void StartClient()
    {
        clientBehaviour.enabled = true;
        canvas.enabled = false;

        //gameObject.AddComponent<PredictionClient>();
    }
}