using Lidgren.Network;
using System;
using System.Net;
using System.Threading;
using UnityEngine;

public class Client : Peer
{
    public readonly NetClient netClient;
    public event Action<string> OnNetworkDebugMessage = null;
    public event Action Connected = null, Disconnected = null;
    public event Action<PlayerSpawnPacket> PlayerSpawn;
    public event Action<PlayerPositionPacket> PlayerPosition;
    public event Action<StatePayloadPacket> StatePayload;

    public Client() : base ()
    {
        instance = this;
        isServer = false;
        isClient = true;

        var config = CreateConfig();
        netPeer = netClient = new NetClient(config);
    }

    public void StartClient()
    {
        netClient.Start();
        Connect("127.0.0.1");
        //thread = new Thread(MessagePump);
        //thread.Start();
    }

    public void StopClient()
    {
        netClient.Shutdown(string.Empty);

    }

    public void Connect(string connectionString)
    {
        if (connectionString.Contains(":"))
        {
            string[] tmpstringarray = connectionString.Split(':');
            netClient.Connect(tmpstringarray[0], int.Parse(tmpstringarray[1]));
        }
        else
        {
            netClient.Connect(connectionString, Peer.AppPort);
        }
    }

    public void Disconnect()
    {
        netClient.Disconnect("Client Disconnect");
    }

    protected override void OnStatusChanged(NetIncomingMessage message)
    {
        switch(message.SenderConnection.Status)
        {
            case NetConnectionStatus.Connected:
                Connected?.Invoke();
                break;
            case NetConnectionStatus.Disconnected:
                Disconnected?.Invoke();
                break;
        }
    }

    protected override void OnDataMessage(NetIncomingMessage message)
    {
        byte packetType = message.ReadByte();
        Packet packet;

        switch(packetType)
        {
            case (byte)PacketTypes.PlayerPacket:
                packet = new PlayerPacket();
                packet.NetIncomingMessageToPacket(message);
                PlayerPacket playerPacket = (PlayerPacket)packet;
                NetworkClientInfo.Instance.LocalPlayerID = playerPacket.Player;
                break;
            case (byte)PacketTypes.PlayerSpawnPacket:
                packet = new PlayerSpawnPacket();
                packet.NetIncomingMessageToPacket(message);
                PlayerSpawn?.Invoke((PlayerSpawnPacket)packet);
                break;
            case (byte)PacketTypes.PlayerPositionPacket:
                packet = new PlayerPositionPacket();
                packet.NetIncomingMessageToPacket(message);
                PlayerPosition?.Invoke((PlayerPositionPacket)packet);
                break;
            case (byte)PacketTypes.StatePayloadPacket:
                packet = new StatePayloadPacket();
                packet.NetIncomingMessageToPacket(message);
                StatePayload?.Invoke((StatePayloadPacket)packet);
                break;
        }
    }

    protected override void OnDebugMessage(NetIncomingMessage message)
    {
        OnNetworkDebugMessage?.Invoke(message.ReadString());
    }

    public void SendMessage(NetOutgoingMessage message, NetDeliveryMethod deliveryMethod)
    {
        netClient.SendMessage(message, deliveryMethod);
    }
}
