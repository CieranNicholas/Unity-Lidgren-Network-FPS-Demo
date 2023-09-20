//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Lidgren;

//namespace Network
//{
//    public class StaticNetworkManager
//    {
//        public static string LocalPlayerID { get; set; }
//        public static Client Client { get; set; }
//        public static Dictionary<string, GameObject> Players { get; set; }
//        public static void InitGameManager(int port, string server, GameObject playerPrefab)
//        {
//            Debug.Log("Starting static game manager");

//            LocalPlayerID = "";
//            Client = new Client(port, server, playerPrefab);
//            Players = new Dictionary<string, GameObject>();
//        }
//    }
//}