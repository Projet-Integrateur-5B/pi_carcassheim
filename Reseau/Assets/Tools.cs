namespace Assets;

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
        RoomSettingsGet = 10,
        RoomSettingsSet = 11,
        RoomStart = 12,
        TuileDraw = 13,
        TuilePlacement = 14,
        PionPlacement = 15,
        CancelTuilePlacement = 16,
        CancelPionPlacement = 17,
        TourValidation = 18,
        TimerExpiration = 19,
        WarningCheat = 20,
        KickFromGame = 21,
        LeaveGame = 22,
        EndGame = 23
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
    public static List<Packet>? ByteArrayToPacket(this byte[] byteArray, ref Errors error)
    {
        // Check if enum "Errors" do exist.
        if (!Enum.IsDefined(typeof(Errors), error))
        {
            throw new InvalidEnumArgumentException(nameof(error), (int)error, typeof(Errors));
        }

        // Initialize a Packet to default.
        var packets = new List<Packet>();

        try
        {
            // Converts a byte array to a JSON string.
            var packetAsJson = Encoding.ASCII.GetString(byteArray);
            var packetAsJsonList = packetAsJson.Split('}');

            for (var i = 0; i < packetAsJsonList.Length - 1; i++)
            {
                // Converts a JSON string to an instance of "Packet".
                packets.Add(JsonConvert.DeserializeObject<Packet>(packetAsJsonList[i] + "}") ??
                            throw new ArgumentNullException(packetAsJson));
            }

            // No error has occured.
            error = Errors.None;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            // Setting the error value.
            error = Errors.Socket;
        }

        return packets;
    }
}
