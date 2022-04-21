using System.Diagnostics.Tracing;
using system;

namespace Server;

using ClassLibrary;

/// <summary>
///     Represents an async server.
/// </summary>
public partial class Server
{
    /// <summary>
    ///     Analyzes the client request and executes it.
    /// </summary>
    /// <param name="ar">Async <see cref="StateObject" />.</param>
    /// <returns>Instance of <see cref="Packet" /> containing the response from the <see cref="Server" />.</returns>
    public static Packet GetFromDatabase(IAsyncResult ar)
    {
        // Initialize the packet to default.
        var packet = new Packet();

        var state = (StateObject?)ar.AsyncState;
        if (state?.Packet is null) // Checking for errors.
        {
            // Setting the error value.
            // TODO : state is null
            return packet;
        }

        // TODO : get what the client asked from the database or whatever
        packet.Type = state.Packet.Type;
        packet.IdMessage = state.Packet.IdMessage;
        packet.IdPlayer = state.Packet.IdPlayer;

        // Check IdMessage : different action

        switch (state.Packet.IdMessage)
        {
            case Tools.IdMessage.Signup:
                AccountSignup(state.Packet, ref packet);
                break;
            case Tools.IdMessage.Login:
                AccountLogin(state.Packet, ref packet);
                break;
            case Tools.IdMessage.Logout: // impossible
                break;
            case Tools.IdMessage.Statistics:
                AccountStatistics(state.Packet, ref packet);
                break;
            
            case Tools.IdMessage.RoomList:
                RoomList(ref packet);
                break;
            case Tools.IdMessage.RoomCreate:
                RoomCreate(state.Packet, ref packet);
                PlayerJoin(state.Packet, ref packet);
                break;
            case Tools.IdMessage.RoomSettingsGet:
                RoomSettingsGet(state.Packet, ref packet);
                break;
            case Tools.IdMessage.RoomSettingsSet:
                RoomSettingsSet(state.Packet, ref packet);
                break;
            
            case Tools.IdMessage.PlayerJoin:
                PlayerJoin(state.Packet, ref packet);
                break;
            case Tools.IdMessage.PlayerLeave:
                PlayerLeave(state.Packet, ref packet);
                break;
            case Tools.IdMessage.PlayerReady:
                PlayerReady(state.Packet, ref packet);
                break;
            case Tools.IdMessage.PlayerCheat:
                PlayerCheat(state.Packet, ref packet);
                break;
            
            case Tools.IdMessage.RoomStart:
                GameStart(state.Packet, ref packet);
                break;
            case Tools.IdMessage.EndGame:
                GameEnd(state.Packet, ref packet);
                break;
            
            case Tools.IdMessage.TuileDraw:
                TuileDraw(state.Packet, ref packet);
                break;
            case Tools.IdMessage.TuilePlacement:
                packet.Error = TuilePlacement(state.Packet);
                break;
            case Tools.IdMessage.PionPlacement:
                packet.Error = PionPlacement(state.Packet);
                break;
            case Tools.IdMessage.TourValidation:
                TourValidation(state.Packet, ref packet);
                break;
            case Tools.IdMessage.TimerExpiration:
                TimerExpiration(ref packet);
                break;
            case Tools.IdMessage.CancelTuilePlacement:
                CancelTuilePlacement(state.Packet, ref packet);
                break;
            case Tools.IdMessage.CancelPionPlacement:
                CancelPionPlacement(state.Packet, ref packet);
                break;
            
            case Tools.IdMessage.Default:
            default:
                packet.Error = Tools.Errors.Unknown;
                break;
        }

        return packet;
    }

    /// <summary>
    ///     New user is creating an account.
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void AccountSignup(Packet packetReceived, ref Packet packet)
    {
        var db = new Database();
        try
        {
            // Adding new user to the database.
            db.Adduser(packetReceived.Data[0], packetReceived.Data[1], packetReceived.Data[2], 0, 1, 0, 0, 0, packetReceived.Data[3]);
        }
        catch (Exception ex)
        {
            // Something went wrong.
            Console.WriteLine("ERROR: Signup : " + ex);
            packet = new Packet
            {
                Error = Tools.Errors.Database
            };
        }
    }
    /// <summary>
    ///     Player is attempting to login.
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void AccountLogin(Packet packetReceived, ref Packet packet)
    {
        var db = new Database();
        try
        {
            // Check if the input data correspond to a user.
            var result = db.Identification(packetReceived.Data[0], packetReceived.Data[1]);
            // Data does not correspond to a user.
            if (result == -1) 
                packet.Error = Tools.Errors.Database;
            // Data does correspond : return the user's IdPlayer.
            else
                packet.IdPlayer = (ulong) result;
        }
        catch (Exception ex)
        {
            // Something went wrong.
            Console.WriteLine("ERROR: Login : " + ex);
            packet = new Packet
            {
                Error = Tools.Errors.Database
            };
        }
    }
    /// <summary>
    ///     Get the player's statistics from the database.
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void AccountStatistics(Packet packetReceived, ref Packet packet)
    {
        var db = new Database();
        try
        {
            // Put the statistics in the data field.
            packet.Data = db.GetStatistics(packetReceived.IdPlayer);
        }
        catch (Exception ex)
        {
            // Something went wrong.
            Console.WriteLine("ERROR: Statistics : " + ex);
            packet = new Packet
            {
                Error = Tools.Errors.Database
            };
        }
    }
    
    
    
    /// <summary>
    ///     List of rooms.
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void RoomList(ref Packet packet)
    {
        // Attempt to get the list of rooms and some data for each room.
        var data = GestionnaireThreadCom.GetRoomList();
        
        if (data.Length > 0)
        {
            // Copy the list of rooms in packet.Data
            packet.Data = data;
        }
        else
        {
            // Something went wrong.
            packet.Error = Tools.Errors.RoomList;
        }
    }
    /// <summary>
    ///     User is creating a room.
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void RoomCreate(Packet packetReceived, ref Packet packet)
    {
        // Attempt to insert a new room.
        var data = GestionnaireThreadCom.CreateRoom(packetReceived.IdPlayer);

        if (data.Count > 0)
        {
            // Sending the room's ID back to the client.
            packet.Data[0] = data[0].ToString();
            // Sending the new server's port (i.e. room port) back to the client.
            packet.Data[1] = data[1].ToString();
        }
        else
        {
            // Something went wrong.
            packet.Error = Tools.Errors.RoomCreate;
        }
    }

    /// <summary>
    ///     Get the settings of a specific room.
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void RoomSettingsGet(Packet packetReceived, ref Packet packet)
    {
        // Attempt to get the room settings.
        var data = GestionnaireThreadCom.SettingsRoom(packetReceived.Data[0]);
        
        if (data.Length > 0)
        {
            // Copy the list of rooms in packet.Data
            packet.Data = data;
        }
        else
        {
            // Something went wrong.
            packet.Error = Tools.Errors.RoomSettings;
        }
    }
    /// <summary>
    ///     Update the settings of a specific room.
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void RoomSettingsSet(Packet packetReceived, ref Packet packet)
    {
        // Attempt to update a room.
        GestionnaireThreadCom.UpdateRoom(packetReceived.Data[0], packetReceived.IdPlayer, packetReceived.Data.Skip(0).ToArray());
    }



    /// <summary>
    ///     Player is joining the room.
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void PlayerJoin(Packet packetReceived, ref Packet packet)
    {
        // Attempt to add a player to the room.
        var playerStatus = PlayerJoinRoom(packetReceived.Data[0], packetReceived.IdPlayer);
        if (playerStatus == Tools.PlayerStatus.Success)
        {
            // Player has successfully joined the room.
            // Need to get the server's port for this specific room.
            packet.Data[0] = GestionnaireThreadCom.GetPortFromPartyID(packetReceived.Data[0]).ToString();
            // TODO : ensuite client switch port, thread serveur detecte nouveau joueur et broadcast
        }
        else
        {
            // Something went wrong.
            packet = new Packet
            {
                Error = Tools.Errors.RoomJoin
            };
        }
    }
    /// <summary>
    ///     Player is leaving the room/game.
    /// </summary>
    /// <remarks>Player might also be forcefully removed.</remarks>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void PlayerLeave(Packet packetReceived, ref Packet packet)
    {
        // Attempt to remove a player from the room.
        var playerStatus = PlayerLeaveRoom(packetReceived.Data[0], packetReceived.IdPlayer);
        if (playerStatus != Tools.PlayerStatus.Success)
        {
            // Something went wrong.
            packet = new Packet
            {
                Error = Tools.Errors.RoomLeave
            };
        }
        // TODO : ensuite thread serveur detecte fin client et broadcast
    }
    /// <summary>
    ///     Player switched its status.
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void PlayerReady(Packet packetReceived, ref Packet packet)
    {
        // Attempt to update a player status within the room.
        var playerStatus = PlayerReadyRoom(packetReceived.Data[0], packetReceived.IdPlayer);
        if (playerStatus != Tools.PlayerStatus.Success)
        {
            // Something went wrong.
            packet = new Packet
            {
                Error = Tools.Errors.PlayerReady
            };
        }
        // TODO : ensuite thread serveur broadcast
    }
    /// <summary>
    ///     Player cheated.
    /// </summary>
    /// <remarks>Player might get kicked out the game in case he cheated too much.</remarks>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void PlayerCheat(Packet packetReceived, ref Packet packet)
    {
        // Attempt to update the number of times a player has cheated in a specific game.
        var playerStatus = PlayerCheatRoom(packetReceived.Data[0], packetReceived.IdPlayer);
        // Player has reached the limit
        if (playerStatus == Tools.PlayerStatus.Kicked)
        {
            // TODO : kick
        }
        else if (playerStatus != Tools.PlayerStatus.Success)
        {
            // Something went wrong.
            packet = new Packet
            {
                Error = Tools.Errors.ToBeDetermined
            };
        }
    }



    /// <summary>
    ///     lancement d'une room
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void GameStart(Packet packetReceived, ref Packet packet)
    {
        _ = packetReceived;
        // lancement de la partie
        // id room dans packetreceived.IdRoom
        // start le timer general
        // retourne l'ordre des joueurs et l'ordre des tuiles
        var list = new List<string>(packet.Data.ToList());
        for (var i = 0; i < 4; i++) // boucle a faire pour nb joueur present dans l'ordre de tirage
        {
            list.Add("pseudo joueur"); // pseudo du joueur X
        }

        packet.Data = list.ToArray();
        packet.Error = Tools.Errors.None; // remplacer par Unknown si erreur
    }
    /// <summary>
    ///     fin de la partie
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void GameEnd(Packet packetReceived, ref Packet packet)
    {
        _ = packetReceived;
        // partie finit, on choisis un emplacement X Y de tuile où sera afficher les scores
        // donner les score des joueurs ( peut importe l'ordre )
        var list = new List<string>(packet.Data.ToList())
        {
            "position X", // position afficher X
            "position Y" // position afficher Y
        };
        for (var i = 0; i < 4; i++) // boucle a faire pour nb joueur present
        {
            list.Add("pseudo joueur"); // pseudo du joueur X
            list.Add("score"); // score du joueur X
        }

        packet.Data = list.ToArray();
        packet.Error = Tools.Errors.None; // remplacer par Unknown si erreur
    }
    
    
    
    /// <summary>
    ///     tirage d'une tuile
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void TuileDraw(Packet packetReceived, ref Packet packet)
    {
        _ = packetReceived;
        // tirage de 3 tuiles pour le joueur
        var list = new List<string>(packet.Data.ToList())
        {
            "numero tuile", "numero tuile", "numero tuile"
        };

        packet.Data = list.ToArray();
        packet.Error = Tools.Errors.None; // remplacer par Unknown si erreur
    }

    /// <summary>
    ///     placement d'une tuile
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    public static Tools.Errors TuilePlacement(Packet packetReceived)
    {
        _ = packetReceived;
        // test si la tuile est bien a un emplacement valide
        return Tools.Errors.Unknown; // remplacer par Permission si validation du joueur sinon par Success
    }

    /// <summary>
    ///     placement d'un pion
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    public static Tools.Errors PionPlacement(Packet packetReceived)
    {
        _ = packetReceived;
        // test si la Pion est bien a un emplacement valide
        return Tools.Errors.Unknown; // remplacer par Permission si validation du joueur sinon par Success
    }

    /// <summary>
    ///     validation d'un tour
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void TourValidation(Packet packetReceived, ref Packet packet)
    {
        _ = packetReceived;
        // valide le tour
        // calculer les points du tour avec les zone qui viennent de se fermé
        var list = new List<string>(packet.Data.ToList()) // a faire seulement si une zone se ferme ( d'apres ce que j'ai compris )
        {
                "position X", // position pion où la zone a été fermé
                "position Y"
        };
        packet.Data = list.ToArray();
        packet.Error = Tools.Errors.Permission; // remplacer par Unknown si erreur dans la validation
    }

    /// <summary>
    ///     expiration d'un timer
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    public static void TimerExpiration(ref Packet packet)
    {
        // timer du joueur expirer, on passe au prochaine joueur
        packet.Error = Tools.Errors.None;
        packet.IdMessage = Tools.IdMessage.TimerExpiration;
    }

    /// <summary>
    ///     annulation d'une tuile
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void CancelTuilePlacement(Packet packetReceived, ref Packet packet)
    {
        _ = packetReceived;
        // cancel le placement de la tuile qui avais été validé par le joueur
        packet.Error = Tools.Errors.None; // remplacer par Permission si aucune erreur sinon par Unknown
    }

    /// <summary>
    ///     annulation d'un pion
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void CancelPionPlacement(Packet packetReceived, ref Packet packet)
    {
        _ = packetReceived;
        // cancel le placement du pion qui avais été validé par le joueur
        packet.Error = Tools.Errors.None; // remplacer par Permission si aucune erreur sinon par Unknown
    }
}
