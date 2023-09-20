//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using Network;

//public class UI_Manager : MonoBehaviour
//{
//    private Server server;
//    [Header("Server Settings")]
//    [SerializeField]
//    private int maxPlayers = 28;
//    [SerializeField]
//    private int s_port = 1337;

//    [Header("Client Settings")]
//    [SerializeField]
//    private string IP = "127.0.0.1";
//    [SerializeField]
//    private int c_port = 1337;
//    [SerializeField]
//    private GameObject playerPrefab;


//    private void Awake()
//    {
//        GameObject.DontDestroyOnLoad(this);
//    }

//    public void StartGameAsHost()
//    {
//        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);

//        server = new(maxPlayers, s_port, playerPrefab);
//    }

//    public void StartGameAsClient()
//    {
//        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);

//        StaticNetworkManager.InitGameManager(c_port, IP, playerPrefab);
//    }

//    private void OnApplicationQuit()
//    {
//        if(server != null)
//            server.Shutdown();
//    }
//}
