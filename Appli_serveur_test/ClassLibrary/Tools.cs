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
        Database = 7,
        RoomList = 8,
        RoomJoin = 9,
        RoomLeave = 10,
        RoomCreate = 11,
        RoomSettings = 12,
        PlayerReady = 20,
        BadPort = 21,
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
        PlayerJoin = 7,
        PlayerLeave = 8,
        PlayerReady = 9,
        PlayerKick = 10,
        RoomSettingsGet = 11,
        RoomSettingsSet = 12,
        RoomStart = 13,
        TuileDraw = 14,
        TuilePlacement = 15,
        PionPlacement = 16,
        CancelTuilePlacement = 17,
        CancelPionPlacement = 18,
        TourValidation = 19,
        TimerExpiration = 20,
        PlayerCheat = 23,
        EndGame = 24
    }

    public enum PlayerStatus
    {
        Default = 0,
        Success = 1,
        Kicked = 2,
        Full = 3,
        Found = 4,
        Permissions = 5,
        NotFound = -1
    }

    public enum Timer
    {
        DixSecondes = 10,
        DemiMinute = 30,
        Minute = 60,
        DemiHeure = 1800,
        Heure = 3600
    }

    public enum Meeple
    {
        Quatre = 4,
        Huit = 8,
        Dix = 10
    }

    public enum Mode
    {
        Default = 0,
        TimeAttack = 1,
        Score = 2
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
                // get the data length
                dataLength = Encoding.ASCII.GetBytes(original.Data[0]).Length;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            // ajout d'un string dans le packet si il y a assez de place
            if (dataLength < headerBytesMaxLength - 3)
            {
                // ajoute la data dans le packet
                packet.Data = new List<string>(packet.Data.ToList()) { original.Data[0] }.ToArray();
                // r�duit le nombre de string qu'il reste � ajouter
                packetLength--;
                // r�duit la place disponible dans le reste du packet
                headerBytesMaxLength = headerBytesMaxLength - dataLength - 3;
                // supprime de original la data mis dans le packet
                original.Data = original.Data.Where((source, index) => index != 0).ToArray();
            }
            else
            {
                // r�cup�re ce qu'on peut encore rajouter dans le reste du packet
                var chaine = original.Data[0][..(headerBytesMaxLength - 5)];
                // original.Data[0].Substring(0, headerBytesMaxLength - 5);

                // ajoute la chaine r�cup�rer
                packet.Data = new List<string>(packet.Data.ToList()) { chaine }.ToArray();

                //supprimer dans original la chaine ajouter
                original.Data[0] = original.Data[0][(headerBytesMaxLength - 5)..];
                // original.Data[0].Substring(headerBytesMaxLength - 5);

                //packet non final
                packet.Final = false;
                //ajout du packet
                packets.Add(packet);
                //reinitialisation du packet pour le prochain ajout
                packet = headerBytes.ByteArrayToPacket(ref error);
                packet.Data = new List<string>(packet.Data.ToList()) { "" }.ToArray();
                if (error != Errors.None)
                {
                    // TODO : ByteArrayToPacket => handle error
                    return new List<Packet>();
                }
                // reinitialisation de la place dispo dans le prochain packet
                headerBytesMaxLength = Packet.MaxPacketSize - headerBytesLength - 4;
            }

            if (packetLength == 0)
            {
                // dernier packet
                packet.Final = true;
                // ajout du dernier pacekt
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
