using Lidgren.Network;
using System.Numerics;
public enum PacketTypes
{
    PlayerPacket,
    PlayerSpawnPacket,
    PlayerDisconnectPacket,
    PlayerInputPacket,
    PlayerPositionPacket,
    InputPayloadPacket,
    StatePayloadPacket
}

public interface IPacket
{
    void PacketToNetOutGoingMessage(NetOutgoingMessage message);
    void NetIncomingMessageToPacket(NetIncomingMessage message);
}

public abstract class Packet : IPacket
{
    public abstract void PacketToNetOutGoingMessage(NetOutgoingMessage message);
    public abstract void NetIncomingMessageToPacket(NetIncomingMessage message);
}

public class PlayerPacket : Packet
{
    public string Player { get; set; }

    public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
    {
        message.Write((byte)PacketTypes.PlayerPacket);
        message.Write(Player);
    }

    public override void NetIncomingMessageToPacket(NetIncomingMessage message)
    {
        Player = message.ReadString();
    }
}

public class PlayerSpawnPacket : Packet
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public string Player { get; set; }

    public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
    {
        message.Write((byte)PacketTypes.PlayerSpawnPacket);
        message.Write(X);
        message.Write(Y);
        message.Write(Z);
        message.Write(Player);
    }

    public override void NetIncomingMessageToPacket(NetIncomingMessage message)
    {
        X = message.ReadFloat();
        Y = message.ReadFloat();
        Z = message.ReadFloat();
        Player = message.ReadString();
    }
}

public class PlayerInputPacket : Packet
{
    public string Player { get; set; }  
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public override void NetIncomingMessageToPacket(NetIncomingMessage message)
    {
        X = message.ReadFloat();
        Y = message.ReadFloat();
        Z = message.ReadFloat();
        Player = message.ReadString();
    }

    public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
    {
        message.Write((byte)PacketTypes.PlayerInputPacket);
        message.Write(X);
        message.Write(Y);
        message.Write(Z);
        message.Write(Player);
    }
}

public class PlayerPositionPacket : Packet
{
    public string Player { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public override void NetIncomingMessageToPacket(NetIncomingMessage message)
    {
        X = message.ReadFloat();
        Y = message.ReadFloat();
        Z = message.ReadFloat();
        Player = message.ReadString();
    }

    public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
    {
        message.Write((byte)PacketTypes.PlayerPositionPacket);
        message.Write(X);
        message.Write(Y);
        message.Write(Z);
        message.Write(Player);
    }
}

public class InputPayloadPacket : Packet
{
    public string Player { get; set; }
    public int Tick { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public override void NetIncomingMessageToPacket(NetIncomingMessage message)
    {
        X = message.ReadFloat();
        Y = message.ReadFloat();
        Z = message.ReadFloat();
        Player = message.ReadString();
        Tick = message.ReadInt16();
    }

    public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
    {
        message.Write((byte)PacketTypes.InputPayloadPacket);
        message.Write(X);
        message.Write(Y);
        message.Write(Z);
        message.Write(Player);
        message.Write(Tick);
    }

}

public class StatePayloadPacket : Packet
{
    public string Player { get; set; }
    public int Tick { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public override void NetIncomingMessageToPacket(NetIncomingMessage message)
    {
        X = message.ReadFloat();
        Y = message.ReadFloat();
        Z = message.ReadFloat();
        Player = message.ReadString();
        Tick = message.ReadInt16();
    }

    public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
    {
        message.Write((byte)PacketTypes.StatePayloadPacket);
        message.Write(X);
        message.Write(Y);
        message.Write(Z);
        message.Write(Player);
        message.Write(Tick);
    }

}

public class PlayerDisconnectPacket : Packet
{
    public string Player { get; set; }

    public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
    {
        message.Write((byte)PacketTypes.PlayerDisconnectPacket);
        message.Write(Player);
    }

    public override void NetIncomingMessageToPacket(NetIncomingMessage message)
    {
        Player = message.ReadString();
    }
}