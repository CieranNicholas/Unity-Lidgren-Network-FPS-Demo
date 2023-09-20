//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;
//using Network;

//public class Billboard : MonoBehaviour
//{
//    [SerializeField]
//    private TextMeshPro text;
//    private Camera mainCamera;

//    private void Awake()
//    {
//        mainCamera = Camera.main;
//    }

//    private void Start()
//    {
//        text.text = StaticNetworkManager.LocalPlayerID;
//    }

//    private void Update()
//    {
//        text.transform.LookAt(text.transform.position + mainCamera.transform.forward);
//    }
//}
