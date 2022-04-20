using System;

namespace ClassLibrary
{
    /// <summary>
    ///     Object used when data is sent or received through the sockets between client and server.
    /// </summary>
    public class Packet
    {
        /// <summary>
        ///     The length of a serialized instance of <see cref="Packet" /> can take up to this exact value.
        /// </summary>
        public const int MaxPacketSize = 512;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Packet" /> class as default.
        /// </summary>
        public Packet()
        {
            this.Type = false;
            this.Final = true;
            this.Data = Array.Empty<string>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Packet" /> class with all set values.
        /// </summary>
        /// <param name="type">One of the <see cref="Packet" /> values.</param>
        /// <param name="idMessage">One of the <see cref="Packet" /> values.</param>
        /// <param name="error">One of the <see cref="Packet" /> values.</param>
        /// <param name="final">One of the <see cref="Packet" /> values.</param>
        /// <param name="idPlayer">One of the <see cref="Packet" /> values.</param>
        /// <param name="data">One of the <see cref="Packet" /> values.</param>
        public Packet(bool type, Tools.IdMessage idMessage,
            Tools.Errors error, bool final, ulong idPlayer, string[] data)
        {
            this.Type = type;
            this.IdMessage = idMessage;
            this.Error = error;
            this.Final = final;
            this.IdPlayer = idPlayer;
            this.Data = data;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Packet" /> class used to communicate from client
        ///     to server.
        /// </summary>
        /// <param name="type">One of the <see cref="Packet" /> values.</param>
        /// <param name="idMessage">One of the <see cref="Packet" /> values.</param>
        /// <param name="final">One of the <see cref="Packet" /> values.</param>
        /// <param name="idPlayer">One of the <see cref="Packet" /> values.</param>
        /// <param name="data">One of the <see cref="Packet" /> values.</param>
        public Packet(bool type, Tools.IdMessage idMessage, bool final,
            ulong idPlayer, string[] data)
        {
            this.Type = type;
            this.IdMessage = idMessage;
            this.Final = final;
            this.IdPlayer = idPlayer;
            this.Data = data;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Packet" /> class used to communicate from server
        ///     to client.
        /// </summary>
        /// <param name="type">One of the <see cref="Packet" /> values.</param>
        /// <param name="error">One of the <see cref="Packet" /> values.</param>
        /// <param name="final">One of the <see cref="Packet" /> values.</param>
        /// <param name="idPlayer">One of the <see cref="Packet" /> values.</param>
        /// <param name="data">One of the <see cref="Packet" /> values.</param>
        public Packet(bool type, Tools.Errors error, bool final, ulong idPlayer,
            string[] data)
        {
            this.Type = type;
            this.Error = error;
            this.Final = final;
            this.IdPlayer = idPlayer;
            this.Data = data;
        }

        /// <summary>
        ///     Indicates wether the instance object <see cref="Packet" /> is coming from a client or from the
        ///     server.
        /// </summary>
        /// <value>False - from client.</value>
        /// <value>True - from server.</value>
        /// <remarks>Default value is false.</remarks>
        public bool Type { get; set; }

        /// <summary>
        ///     <see cref="Tools.IdMessage" /> indicates the ID for a specific instance object of
        ///     <see cref="Packet" />.
        /// </summary>
        /// <value>Default is "Default" = 0.</value>
        public Tools.IdMessage IdMessage { get; set; }

        /// <summary>
        /// <see cref="Tools.Errors"/> indicates the error for a specific instance object of <see cref="Packet"/>.
        /// </summary>
        /// <value>Default is "None" = 0.</value>
        public Tools.Errors Error { get; set; }

        /// <summary>
        ///     Indicates whether the instance object <see cref="Packet" /> is the final one of a list.
        /// </summary>
        /// <value>Default is true.</value>
        public bool Final { get; set; }

        /// <summary>
        ///     The user's ID within the instance object <see cref="Packet" />.
        /// </summary>
        public ulong IdPlayer { get; set; }

        /// <summary>
        ///     The data within the instance object <see cref="Packet" />.
        /// </summary>
        /// <value>Default is empty.</value>
        public string[] Data { get; set; }

        /// <summary>
        ///     Converts the values of this instance to its equivalent string representation.
        /// </summary>
        /// <param name="this">The packet object to which the method is being applied.</param>
        /// <returns>The string representation of the value of this instance under the JSON format.</returns>
        public override string ToString() => "Type:" + this.Type + "; "
                                             + "IdMessage:" + this.IdMessage + "; "
                                             + "Error:" + this.Error + "; "
                                             + "Final:" + this.Final + ";"
                                             + "IdPlayer:" + this.IdPlayer + "; "
                                             + "Data:" + string.Join(" ", this.Data) + ";";
    }
}


