using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Server : Peer
{
    public readonly NetServer netServer;
    public event Action<string> OnNetworkDebugMessage = null;

    public readonly List<string> ConnectedClients;
    public readonly Dictionary<string, NetConnection> PlayerConnections;
    public readonly Dictionary<string, Vector3> ConnectedClientsPositions;
    public readonly Dictionary<string, Quaternion> ConnectedClientsRotations;
    public event Action<List<NetConnection>> PlayerSpawn;
    public event Action<PlayerInputPacket> HandleClientInput;
    public event Action<InputPayloadPacket> OnClientMovement;
    

    public Server() : base()
    {
        instance = this;
        isServer = true;
        isClient = false;

        var config = CreateConfig();
        config.Port = AppPort;
        netPeer = netServer = new NetServer(config);

        PlayerConnections = new Dictionary<string, NetConnection>();
        ConnectedClients = new List<string>();
        ConnectedClientsPositions = new Dictionary<string, Vector3>();
    }

    public void StartServer()
    {
        netServer.Start();
        //thread = new Thread(MessagePump);
        //thread.Start();
    }

    public void StopServer()
    {
        netServer.Shutdown(string.Empty);
    }
    protected override void OnStatusChanged(NetIncomingMessage message)
    {
        Debug.Log(message.SenderConnection.Status);

        var player = NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier);

        switch (message.SenderConnection.Status)
        {
            case NetConnectionStatus.Connected:
                SendWelcomePacket(message.SenderConnection, player);
                SpawnPlayers(netServer.Connections, message.SenderConnection, player);
                break;
            case NetConnectionStatus.Disconnected:
                if(ConnectedClients.Contains(player))
                    ConnectedClients.Remove(player);
                if (ConnectedClientsPositions.ContainsKey(player))
                    ConnectedClientsPositions.Remove(player);
                if(PlayerConnections.ContainsKey(player))
                    PlayerConnections.Remove(player);
                break;
        }
    }

    #region welcome methods

    private void SendWelcomePacket(NetConnection local, string player)
    {
        Debug.Log($"Client connected, assigning local player ID: ${player}");
        ConnectedClients.Add(player);
        PlayerConnections.Add(player, local);
        ConnectedClientsPositions[player] = new Vector3();

        NetOutgoingMessage message = netServer.CreateMessage();
        new PlayerPacket() { Player = player }.PacketToNetOutGoingMessage(message);
        netServer.SendMessage(message, local, NetDeliveryMethod.ReliableOrdered, 0);
    }

    private void SpawnPlayers(List<NetConnection> allConnections, NetConnection local, string player)
    {

        // Spawn all the clients on the local player
        allConnections.ForEach(p =>
        {
            string uniqueID = NetUtility.ToHexString(p.RemoteUniqueIdentifier);
            if (player != uniqueID)
                SendSpawnPacketToLocal(local, uniqueID, ConnectedClientsPositions[uniqueID]);
        });

        // Spawn the local player on all clients
        System.Random random = new System.Random();
        SendSpawnPacketToAll(allConnections, player, new Vector3(random.Next(0, 5), 1, random.Next(0, 5)));

        // Spawn players for the server
        PlayerSpawn?.Invoke(allConnections);
    }

    private void SendSpawnPacketToLocal(NetConnection local, string player, Vector3 position)
    {

        Debug.Log(local);

        Debug.Log($"Sending user spawn packet for player {player}");

        ConnectedClientsPositions[player] = position;

        NetOutgoingMessage message = netServer.CreateMessage();
        new PlayerSpawnPacket()
        {
            Player = player,
            X = position.x,
            Y = position.y,
            Z = position.z,
        }.PacketToNetOutGoingMessage(message);
        netServer.SendMessage(message, local, NetDeliveryMethod.ReliableOrdered, 0);
    }

    private void SendSpawnPacketToAll(List<NetConnection> allClients, string player, Vector3 position)
    {
        Debug.Log($"Sending user spawn packet for player {player}");

        ConnectedClientsPositions[player] = position;

        NetOutgoingMessage message = netServer.CreateMessage();
        new PlayerSpawnPacket()
        {
            Player = player,
            X = position.x,
            Y = position.y,
            Z = position.z,
        }.PacketToNetOutGoingMessage(message);
        netServer.SendMessage(message, allClients, NetDeliveryMethod.ReliableOrdered, 0);
    }

    #endregion

    public void SendPlayerPositions()
    {
        foreach(var item in ConnectedClientsPositions)
        {
            string PlayerID = item.Key;
            Vector3 position = item.Value;

            NetOutgoingMessage message = netServer.CreateMessage();
            new PlayerPositionPacket()
            {
                Player = PlayerID,
                X = position.x,
                Y = position.y,
                Z = position.z,
            }.PacketToNetOutGoingMessage(message);

            netServer.SendMessage(message, netServer.Connections, NetDeliveryMethod.ReliableOrdered, 0);
        }
    }

    protected override void OnDataMessage(NetIncomingMessage message)
    {
        var packetType = message.ReadByte();
        Packet packet;

        switch(packetType)
        {
            case (byte)PacketTypes.PlayerInputPacket:
                packet = new PlayerInputPacket();
                packet.NetIncomingMessageToPacket(message);
                HandleClientInput?.Invoke((PlayerInputPacket)packet);
                break;
            case (byte)PacketTypes.InputPayloadPacket:
                packet = new InputPayloadPacket();
                packet.NetIncomingMessageToPacket(message);
                OnClientMovement?.Invoke((InputPayloadPacket)packet);
                break;
        }
    }

    protected override void OnDebugMessage(NetIncomingMessage message)
    {
        OnNetworkDebugMessage?.Invoke(message.ReadString());
    }

    public void SendMessage(NetOutgoingMessage message, NetConnection target,NetDeliveryMethod deliveryMethod)
    {
        netServer.SendMessage(message, target, deliveryMethod, 0);
    }
}
