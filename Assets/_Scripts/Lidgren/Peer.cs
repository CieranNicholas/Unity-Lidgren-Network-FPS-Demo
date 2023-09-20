using Lidgren.Network;
using System.Diagnostics;
using System.Threading;
using UnityEngine;


public abstract class Peer
{
    public NetPeer netPeer;
    public static Peer instance;
    public static bool isServer, isClient;
    public const string AppID = "MyGame";
    public const int AppPort = 25000;
    public byte host_id;

    public const byte REMOTE_CALL_FLAG = 0;
    public const byte USER_COMMAND_FLAG = 1;
    public const byte ACTOR_STATE_FLAG = 2;
    public const byte ACTOR_EVENT_FLAG = 3;

    public Thread thread;


    public Peer() : base ()
    {

    }

    public void MessagePump()
    {
        NetIncomingMessage message;
        while ((message = netPeer.ReadMessage()) != null)
        {
            switch (message.MessageType)
            {
                case NetIncomingMessageType.Data:
                    OnDataMessage(message);
                    break;
                case NetIncomingMessageType.StatusChanged:
                    OnStatusChanged(message);
                    break;
                case NetIncomingMessageType.VerboseDebugMessage:
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.ErrorMessage:
                case NetIncomingMessageType.Error:
                    OnDebugMessage(message);
                    break;
            }
            netPeer.Recycle(message);
        }
    }

    public NetPeerConfiguration CreateConfig()
    {
        return new NetPeerConfiguration(AppID);
    }

    public virtual NetOutgoingMessage CreateMessage()
    {
        return netPeer.CreateMessage();
    }

    protected virtual void OnDataMessage(NetIncomingMessage message)
    {
    }

    protected virtual void OnStatusChanged(NetIncomingMessage message)
    {
    }

    protected virtual void OnDebugMessage(NetIncomingMessage message)
    {
    }

}