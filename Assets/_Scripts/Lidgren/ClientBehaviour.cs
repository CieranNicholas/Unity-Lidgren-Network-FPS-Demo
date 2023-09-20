using Lidgren.Network;
using TMPro;
using UnityEngine;
using Cinemachine;

public class ClientBehaviour : MonoBehaviour
{
    public Client Client;

    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private CinemachineVirtualCamera cm;
    [SerializeField]
    public GameObject localPlayerObject;

    private void OnEnable()
    {
        Client = new Client();
        Client.StartClient();

        Client.PlayerSpawn += SpawnPlayer;
        Client.PlayerPosition += UpdatePlayerPosition;
    }

    private void OnDisable()
    {
        Client.Disconnect();
        Client.StopClient();

        Client.PlayerSpawn -= SpawnPlayer;
        InputManager.Instance.SendMovementInput -= SendInputToServer;

    }

    private void Update()
    {
        if (Client != null)
        {
            Client.MessagePump();
        }
    }

    private void SpawnPlayer(PlayerSpawnPacket packet)
    {
        Vector3 position = new Vector3(packet.X, packet.Y, packet.Z);
        Debug.Log($"Spawning player {packet.Player}");
        Debug.Log($"Position: {position}");
        Debug.Log($"LocalPlayerID: {NetworkClientInfo.Instance.LocalPlayerID}");


        GameObject playerObj = Instantiate(playerPrefab, position, Quaternion.identity);

        NetworkClientInfo.Instance.Players.Add(packet.Player, playerObj);

        TextMeshPro text;
        text = playerObj.GetComponentInChildren<TextMeshPro>();
        text.text = packet.Player;

        if (packet.Player == NetworkClientInfo.Instance.LocalPlayerID)
        {
            playerObj.transform.name = "Local Player";
            playerObj.AddComponent<InputManager>();

            cm.Follow = playerObj.transform.GetChild(0).transform;
            localPlayerObject = playerObj;
        }
        else
        {
            playerObj.transform.name = packet.Player;
        }
        
        InputManager.Instance.SendMovementInput += SendInputToServer;
        gameObject.AddComponent<PredictionClient>();
    }

    public void SendInputToServer(Vector3 _input)
    {
        NetOutgoingMessage message = Client.CreateMessage();
        new PlayerInputPacket()
        {
            Player = NetworkClientInfo.Instance.LocalPlayerID,
            X = _input.x,
            Y = _input.y,
            Z = _input.z,
        }.PacketToNetOutGoingMessage(message);

        Client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
    }

    public void UpdatePlayerPosition(PlayerPositionPacket packet)
    {
        Vector3 targetPosition = new Vector3(packet.X, packet.Y, packet.Z);
        // Kinda slidy, need to sort
        localPlayerObject.transform.position = Vector3.Lerp(localPlayerObject.transform.position, targetPosition, Time.deltaTime * 10f);
        //localPlayerObject.transform.position = position;
    }

}
