namespace ClassLibrary;

using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;

/// <summary>
///     Class involving methods used by an instance of <see cref="Packet" /> while communicating.
/// </summary>
public static class Tools
{
    /// <summary>
    ///     Indicates which type of error has occurred.
    /// </summary>
    public enum Errors
    {
        None = 0,
        Unknown = -1,
        Socket = 1,
        Format = 2,
        ConfigFile = 3,
        Receive = 4,
        Data = 5,
        Permission = 6,
        Success = 7,
        Database = 8,
        ToBeDetermined = 999
    }

    /// <summary>
    ///     Indicates which type of data is used by an instance of the <see cref="Packet" /> class.
    /// </summary>
    public enum IdMessage : byte
    {
        Default = 0,
        Login = 1,
        Logout = 2,
        Signup = 3,
        Statistics = 4,
        RoomList = 5,
        RoomCreate = 6,
        RoomJoin = 7,
        RoomLeave = 8,
        RoomReady = 9,
        RoomSettings = 10,
        RoomStart = 11,
        TuileDraw = 12,
        TuilePlacement = 13,
        PionPlacement = 14,
        CancelTuilePlacement = 15,
        CancelPionPlacement = 16,
        TourValidation = 17,
        TimerExpiration = 18,
        WarningCheat = 19,
        KickFromGame = 20,
        LeaveGame = 21,
        EndGame = 22
    }

    /// <summary>
    ///     Converts an instance of <see cref="Packet" /> to a byte array (serialized).
    /// </summary>
    /// <param name="packet">Instance of <see cref="Packet" /> which is being serialized.</param>
    /// <param name="error">Stores the <see cref="Errors" /> value.</param>
    /// <returns>
    ///     A byte array corresponding to an instance of <see cref="Packet" /> which has been
    ///     serialized.
    /// </returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    public static byte[] PacketToByteArray(this Packet packet, ref Errors error)
    {
        // Check if enum "Errors" do exist.
        if (!Enum.IsDefined(typeof(Errors), error))
        {
            throw new InvalidEnumArgumentException(nameof(error), (int)error, typeof(Errors));
        }

        // Initialize a byte array to empty.
        var packetAsBytes = Array.Empty<byte>();

        // Catch : could not serialize the "Packet" instance.
        try
        {
            // Converts an instance of "Packet" to a JSON string.
            var packetAsJsonString = JsonConvert.SerializeObject(packet);
            // Converts a JSON string to byte array.
            packetAsBytes = Encoding.ASCII.GetBytes(packetAsJsonString);

            // No error has occured.
            error = Errors.None;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            // Setting the error value.
            error = Errors.Socket;
        }

        return packetAsBytes;
    }

    /// <summary>
    ///     Converts a byte array to an instance of <see cref="Packet" /> (deserialized).
    /// </summary>
    /// <param name="byteArray">Byte array which is being deserialized.</param>
    /// <param name="error">Stores the <see cref="Errors" /> value.</param>
    /// <returns>
    ///     The instance of <see cref="Packet" /> corresponding to a byte array which has been
    ///     deserialized.
    /// </returns>
    /// <exception cref="InvalidEnumArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static Packet ByteArrayToPacket(this byte[] byteArray, ref Errors error)
    {
        // Check if enum "Errors" do exist.
        if (!Enum.IsDefined(typeof(Errors), error))
        {
            throw new InvalidEnumArgumentException(nameof(error), (int)error, typeof(Errors));
        }

        // Initialize a Packet to default.
        var packet = new Packet();

        try
        {
            // Converts a byte array to a JSON string.
            var packetAsJson = Encoding.ASCII.GetString(byteArray);
            // Converts a JSON string to an instance of "Packet".
            packet = JsonConvert.DeserializeObject<Packet>(packetAsJson) ??
                     throw new ArgumentNullException(packetAsJson);

            // No error has occured.
            error = Errors.None;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            // Setting the error value.
            error = Errors.Socket;
        }

        return packet;
    }

    /// <summary>
    ///     Split an instance of <see cref="Packet" /> into a list of instances.
    /// </summary>
    /// <param name="original">Instance of <see cref="Packet" /> which is being split.</param>
    /// <param name="error">Stores the <see cref="Errors" /> value.</param>
    /// <returns>The list of <see cref="Packet" /></returns>
    public static List<Packet> Split(this Packet original, ref Errors error)
    {
        // Initialize a list of Packet.
        var packets = new List<Packet>();

        // Get the original packet length to check if a split is mandatory.
        var originalBytesLength = original.PacketToByteArray(ref error).Length;
        if (error != Errors.None) // Checking for errors.
        {
            // Setting the error value.
            // TODO : PacketToByteArray => handle error
            return packets; // List is empty.
        }

        // Check if a split is mandatory.
        if (originalBytesLength < Packet.MaxPacketSize)
        {
            // Not mandatory : add the original packet to the list and return.
            packets.Add(original);
            return packets;
        }

        // Split is mandatory.

        // Copying the original data and getting its length.
        var dataString = string.Join(string.Empty, original.Data);
        var packetLength = original.Data.Length;

        // Copying the rest : common header for each packet of the list.
        var header = new Packet(original.Type, original.IdMessage, original.Error, false, original.IdPlayer, Array.Empty<string>());

        // Serializing the header.
        var headerBytes = header.PacketToByteArray(ref error);
        if (error != Errors.None) // Checking for errors.
        {
            // Setting the error value.
            // TODO : PacketToByteArray => handle error
            return packets; // List is empty.
        }

        // Get the headers length.
        var headerBytesLength = headerBytes.Length;
        // Get the maximum length of the data field (depending on the headers length).
        var headerBytesMaxLength = Packet.MaxPacketSize - headerBytesLength;

        // Serializing the original data field and getting its length.
        var dataBytes = Encoding.ASCII.GetBytes(dataString);
        var dataBytesTotalLength = dataBytes.Length;

        // Deserializing the header.
        var packet = headerBytes.ByteArrayToPacket(ref error);
        if (error != Errors.None) // Checking for errors.
        {
            // Setting the error value.
            // TODO : ByteArrayToPacket => handle error
            return new List<Packet>(); // List is empty.
        }

        int dataLength;
        while (true)
        {
            try
            {
                dataLength = Encoding.ASCII.GetBytes(original.Data[0]).Length;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            if (dataLength < headerBytesMaxLength - 3)
            {
                packet.Data = new List<string>(packet.Data.ToList()) { original.Data[0] }.ToArray();
                packetLength--;
                headerBytesMaxLength = headerBytesMaxLength - dataLength - 3;
                original.Data = original.Data.Where((source, index) => index != 0).ToArray();
            }
            else
            {
                var chaine = original.Data[0][..(headerBytesMaxLength - 5)];
                // original.Data[0].Substring(0, headerBytesMaxLength - 5);
                packet.Data = new List<string>(packet.Data.ToList()) { chaine }.ToArray();
                original.Data[0] = original.Data[0][(headerBytesMaxLength - 5)..];
                // original.Data[0].Substring(headerBytesMaxLength - 5);

                packet.Final = false;
                packets.Add(packet);
                packet = headerBytes.ByteArrayToPacket(ref error);
                packet.Data = new List<string>(packet.Data.ToList()) { "" }.ToArray();
                if (error != Errors.None)
                {
                    // TODO : ByteArrayToPacket => handle error
                    return new List<Packet>();
                }

                headerBytesMaxLength = Packet.MaxPacketSize - headerBytesLength - 4;
            }

            if (packetLength == 0)
            {
                packet.Final = true;
                packets.Add(packet);
                break;
            }
        }

        return packets;
    }

    /// <summary>
    ///     Concatenate a list of instances of <see cref="Packet" /> into a single instance.
    /// </summary>
    /// <param name="packets">Instances of <see cref="Packet" /> which is being concatenated.</param>
    /// <param name="error">Stores the <see cref="Errors" /> value.</param>
    /// <returns>The concatenated list of <see cref="Packet" />.</returns>
    public static Packet Concatenate(this List<Packet> packets, ref Errors error)
    {
        // Initialize the list to default : minimum size is 1.
        var original = packets[0];

        // All packets in the list except the first one.
        foreach (var packet in packets.Skip(1))
        {
            // Catch : could not concatenate the data
            try
            {
                // Adding packet.Data to original.Data
                original.Data = original.Data.Concat(packet.Data).ToArray();

                // No error has occured.
                error = Errors.None;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Setting the error value.
                error = Errors.Data;
            }
        }

        return original;
    }
}
