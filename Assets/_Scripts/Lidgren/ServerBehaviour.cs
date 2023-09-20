using UnityEngine;
using System.Threading;
using System.Collections.Generic;
using Lidgren.Network;
using TMPro;

public class ServerBehaviour : MonoBehaviour
{
    public Server Server;

    [SerializeField]
    private GameObject playerPrefab;

    private void OnEnable()
    {
        Server = new Server();
        Server.StartServer();

        Server.PlayerSpawn += SpawnPlayers;
        Server.HandleClientInput += HandleClientInput;
        Server.OnClientMovement += OnClientMovement;
    }

    private void FixedUpdate()
    {
        if(Server != null)
        {
            Server.MessagePump();
            //Server.SendPlayerPositions();
        }
    }

    private void OnDisable()
    {
        Server.PlayerSpawn -= SpawnPlayers;
        Server.HandleClientInput -= HandleClientInput;
    }

    private void SpawnPlayers(List<NetConnection> players)
    {
        players.ForEach(player =>
        {
            string ID = NetUtility.ToHexString(player.RemoteUniqueIdentifier);
            Vector3 position = Server.ConnectedClientsPositions[ID];
            if(position == null)
                position = Vector3.zero;
            GameObject playerObj = Instantiate(playerPrefab, position, Quaternion.identity);
            playerObj.transform.name = ID;
            playerObj.GetComponentInChildren<TextMeshPro>().text = ID;
        });
    }

    private void HandleClientInput(PlayerInputPacket packet)
    {
        string uniqueID = packet.Player;
        Vector3 movementInput = new Vector3(packet.X, packet.Y, packet.Z);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in players)
        {
            if(player.transform.name == uniqueID)
            {
                CharacterController controller = player.GetComponent<CharacterController>();
                controller.Move(movementInput * 10f *Time.deltaTime);

                Server.ConnectedClientsPositions[uniqueID] = player.transform.position;
            }
        }
    }

    private void OnClientMovement(InputPayloadPacket packet)
    {
        InputPayload inputPayload = new InputPayload()
        {
            tick = packet.Tick,
            inputVector = new Vector3(packet.X, packet.Y, packet.Z),
            player = packet.Player
        };
        PredictionServer.Instance.inputQueue.Enqueue(inputPayload);
    }
}
