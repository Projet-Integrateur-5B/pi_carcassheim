using System.Diagnostics.Tracing;
using System.Net;
using System.Net.Sockets;

namespace Server;

using ClassLibrary;
using system;

/// <summary>
///     Represents an async server.
/// </summary>
public partial class Server
{
    /// <summary>
    ///     Analyzes the client request and executes it.
    /// </summary>
    /// <param name="ar">Async <see cref="StateObject" />.</param>
    /// <param name="socket">Socket <see cref="Socket" />.</param>
    /// <returns>Instance of <see cref="Packet" /> containing the response from the <see cref="Server" />.</returns>
    public static Packet GetFromDatabase(IAsyncResult ar, Socket socket)
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
                AccountSignup(state.Packet, ref packet, socket);
                break;
            case Tools.IdMessage.Login:
                AccountLogin(state.Packet, ref packet, socket);
                break;
            case Tools.IdMessage.Logout: // impossible
                break;
            case Tools.IdMessage.Statistics:
                AccountStatistics(state.Packet, ref packet, socket);
                break;

            case Tools.IdMessage.RoomList:
                RoomList(state.Packet, ref packet, socket);
                break;
            case Tools.IdMessage.RoomCreate:
                RoomCreate(state.Packet, ref packet, socket);
                break;
            case Tools.IdMessage.RoomSettingsGet:
                RoomSettingsGet(state.Packet, ref packet, socket);
                break;
            case Tools.IdMessage.RoomSettingsSet:
                RoomSettingsSet(state.Packet, ref packet, socket);
                break;

            case Tools.IdMessage.PlayerJoin:
                PlayerJoin(state.Packet, ref packet, socket);
                break;
            case Tools.IdMessage.PlayerLeave:
                PlayerLeave(state.Packet, ref packet, socket);
                break;
            case Tools.IdMessage.PlayerReady:
                PlayerReady(state.Packet, ref packet, socket);
                break;
            case Tools.IdMessage.PlayerKick:
                PlayerKick(state.Packet, ref packet, socket);
                break;
            case Tools.IdMessage.PlayerCheat:
                PlayerCheat(state.Packet, ref packet, socket);
                break;

            case Tools.IdMessage.RoomStart:
                GameStart(state.Packet, ref packet, socket);
                break;
            case Tools.IdMessage.EndGame:
                GameEnd(state.Packet, ref packet, socket);
                break;

            case Tools.IdMessage.TuileDraw:
                TuileDraw(state.Packet, ref packet, socket);
                break;
            case Tools.IdMessage.TuilePlacement:
                packet.Error = TuilePlacement(state.Packet, socket);
                break;
            case Tools.IdMessage.PionPlacement:
                packet.Error = PionPlacement(state.Packet, socket);
                break;
            case Tools.IdMessage.TourValidation:
                TourValidation(state.Packet, ref packet, socket);
                break;
            case Tools.IdMessage.TimerExpiration:
                TimerExpiration(ref packet, socket);
                break;
            case Tools.IdMessage.CancelTuilePlacement:
                CancelTuilePlacement(state.Packet, ref packet, socket);
                break;
            case Tools.IdMessage.CancelPionPlacement:
                CancelPionPlacement(state.Packet, ref packet, socket);
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
    /// <param name="socket">Socket <see cref="Socket" />.</param>
    public static void AccountSignup(Packet packetReceived, ref Packet packet, Socket socket)
    {
        // Vérification que la communication est reçue par le serveur main
        int portListening = ((IPEndPoint)socket.LocalEndPoint).Port;
        if (portListening != 10000)
        {
            Console.WriteLine("ERROR: Thread_com received message instead of serveur_main, IdMessage : " + packetReceived.IdMessage);
            packet = new Packet
            {
                Error = Tools.Errors.BadPort
            };
            return;
        }

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
    /// <param name="socket">Socket <see cref="Socket" />.</param>
    public static void AccountLogin(Packet packetReceived, ref Packet packet, Socket socket)
    {
        // Vérification que la communication est reçue par le serveur main
        int portListening = ((IPEndPoint)socket.LocalEndPoint).Port;
        if (portListening != 10000)
        {
            Console.WriteLine("ERROR: Thread_com received message instead of serveur_main, IdMessage : " + packetReceived.IdMessage);
            packet = new Packet
            {
                Error = Tools.Errors.BadPort
            };
            return;
        }

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
                packet.IdPlayer = (ulong)result;
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
    /// <param name="socket">Socket <see cref="Socket" />.</param>
    public static void AccountStatistics(Packet packetReceived, ref Packet packet, Socket socket)
    {
        // Vérification que la communication est reçue par le serveur main
        int portListening = ((IPEndPoint)socket.LocalEndPoint).Port;
        if (portListening != 10000)
        {
            Console.WriteLine("ERROR: Thread_com received message instead of serveur_main, IdMessage : " + packetReceived.IdMessage);
            packet = new Packet
            {
                Error = Tools.Errors.BadPort
            };
            return;
        }

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
    /// <param name="socket">Socket <see cref="Socket" />.</param>
    public static void RoomList(Packet packetReceived, ref Packet packet, Socket socket)
    {
        // Vérification que la communication est reçue par le serveur main
        int portListening = ((IPEndPoint)socket.LocalEndPoint).Port;
        if (portListening != 10000)
        {
            Console.WriteLine("ERROR: Thread_com received message instead of serveur_main, IdMessage : " + packetReceived.IdMessage);
            packet = new Packet
            {
                Error = Tools.Errors.BadPort
            };
            return;
        }

        // Récupération du singleton gestionnaire
        GestionnaireThreadCom gestionnaire = GestionnaireThreadCom.GetInstance();

        try
        {
            // Attempt to get the list of rooms and some data for each room.
            string[] result = gestionnaire.GetRoomList();

            // Copy the list of rooms in packet.Data
            packet.Data = result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR: GetPortFromPartyID : " + ex);
            packet.Error = Tools.Errors.ToBeDetermined;
            return;
        }

    }
    /// <summary>
    ///     User is creating a room.
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    /// <param name="socket">Socket <see cref="Socket" />.</param>
    public static void RoomCreate(Packet packetReceived, ref Packet packet, Socket socket)
    {
        // Vérification que la communication est reçue par le serveur main
        int portListening = ((IPEndPoint)socket.LocalEndPoint).Port;
        if (portListening != 10000)
        {
            Console.WriteLine("ERROR: Thread_com received message instead of serveur_main, IdMessage : " + packetReceived.IdMessage);
            packet = new Packet
            {
                Error = Tools.Errors.BadPort
            };
            return;
        }

        // Récupération du singleton gestionnaire
        GestionnaireThreadCom gestionnaire = GestionnaireThreadCom.GetInstance();

        // Attempt to insert a new room.
        List<int> result = gestionnaire.CreateRoom(packetReceived.IdPlayer);

        if (result[0] == -1 || result[1] == -1 || result.Count != 2)
        {
            // Something went wrong.
            packet.Error = Tools.Errors.RoomCreate;
        }
        else
        {
            // Sending the room's ID back to the client. 
            packet.Data[0] = result[0].ToString();
            // Sending the new server's port (i.e. room port) back to the client.
            packet.Data[1] = result[1].ToString();
        }
    }

    /// <summary>
    ///     Get the settings of a specific room.
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    /// <param name="socket">Socket <see cref="Socket" />.</param>
    public static void RoomSettingsGet(Packet packetReceived, ref Packet packet, Socket socket)
    {
        // Vérification que la communication est reçue par un thread de com
        int portListening = ((IPEndPoint)socket.LocalEndPoint).Port;
        if (portListening == 10000)
        {
            Console.WriteLine("ERROR: Thread_com received message instead of serveur_main, IdMessage : " + packetReceived.IdMessage);
            packet = new Packet
            {
                Error = Tools.Errors.BadPort
            };
            return;
        }

        // Récupération du singleton gestionnaire
        GestionnaireThreadCom gestionnaire = GestionnaireThreadCom.GetInstance();

        // Attempt to get the room settings.
        var data = gestionnaire.SettingsRoom(packetReceived.Data[0]);

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
    /// <param name="socket">Socket <see cref="Socket" />.</param>
    public static void RoomSettingsSet(Packet packetReceived, ref Packet packet, Socket socket)
    {
        // Vérification que la communication est reçue par un thread de com
        int portListening = ((IPEndPoint)socket.LocalEndPoint).Port;
        if (portListening == 10000)
        {
            Console.WriteLine("ERROR: Thread_com received message instead of serveur_main, IdMessage : " + packetReceived.IdMessage);
            packet = new Packet
            {
                Error = Tools.Errors.BadPort
            };
            return;
        }

        // Récupération du singleton gestionnaire
        GestionnaireThreadCom gestionnaire = GestionnaireThreadCom.GetInstance();

        // Attempt to update a room.
        gestionnaire.UpdateRoom(packetReceived.Data[0], packetReceived.IdPlayer, packetReceived.Data.Skip(0).ToArray());
    }



    /// <summary>
    ///     Player is joining the room.
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    /// <param name="socket">Socket <see cref="Socket" />.</param>
    public static void PlayerJoin(Packet packetReceived, ref Packet packet, Socket socket)
    {
        // Vérification que la communication est reçue par le serveur main
        int portListening = ((IPEndPoint)socket.LocalEndPoint).Port;
        if (portListening != 10000)
        {
            Console.WriteLine("ERROR: Thread_com received message instead of serveur_main, IdMessage : " + packetReceived.IdMessage);
            packet = new Packet
            {
                Error = Tools.Errors.BadPort
            };
            return;
        }

        // Récupération du singleton gestionnaire
        GestionnaireThreadCom gestionnaire = GestionnaireThreadCom.GetInstance();

        // Attempt to add a player to the room.
        var port = gestionnaire.JoinPlayer(packetReceived.Data[0], packetReceived.IdPlayer);

        if (port != -1)
        {
            packet.Data[0] = port.ToString();
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
    /// <param name="packetReceived">Instance of <see cref="Packet" /> received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    /// <param name="socket">Socket <see cref="Socket" />.</param>
    public static void PlayerLeave(Packet packetReceived, ref Packet packet, Socket socket)
    {
        // Vérification que la communication est reçue par un thread de com
        int portListening = ((IPEndPoint)socket.LocalEndPoint).Port;
        if (portListening == 10000)
        {
            Console.WriteLine("ERROR: Thread_com received message instead of serveur_main, IdMessage : " + packetReceived.IdMessage);
            packet = new Packet
            {
                Error = Tools.Errors.BadPort
            };
            return;
        }
        
        // Récupération du singleton gestionnaire
        GestionnaireThreadCom gestionnaire = GestionnaireThreadCom.GetInstance();
        
        // Attempt from a player to leave the room.
        var playerStatus = gestionnaire.RemovePlayer(packetReceived.Data[0], packetReceived.IdPlayer);
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
    ///     Player is being kicked out of the room/game.
    /// </summary>
    /// <remarks>Player might also be forcefully removed.</remarks>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    /// /// <param name="socket">Socket <see cref="Socket" />.</param>
    public static void PlayerKick(Packet packetReceived, ref Packet packet, Socket socket)
    {
        // Vérification que la communication est reçue par un thread de com
        int portListening = ((IPEndPoint)socket.LocalEndPoint).Port;
        if (portListening == 10000)
        {
            Console.WriteLine("ERROR: Thread_com received message instead of serveur_main, IdMessage : " + packetReceived.IdMessage);
            packet = new Packet
            {
                Error = Tools.Errors.BadPort
            };
            return;
        }
        
        // Récupération du singleton gestionnaire
        GestionnaireThreadCom gestionnaire = GestionnaireThreadCom.GetInstance();
        
        // Parse idPlayerToKick from string to ulong.
        ulong idPlayerToKick;
        try
        {
            idPlayerToKick = ulong.Parse(packetReceived.Data[1]);
        }
        catch (Exception e)
        {
            // Something went wrong.
            packet = new Packet
            {
                Error = Tools.Errors.RoomLeave
            };
            return;
        }
        
        // Attempt to kick a player from the room.
        var playerStatus = gestionnaire.KickPlayer(packetReceived.Data[0], packetReceived.IdPlayer, idPlayerToKick);
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
    /// <param name="socket">Socket <see cref="Socket" />.</param>
    public static void PlayerReady(Packet packetReceived, ref Packet packet, Socket socket)
    {
        // Vérification que la communication est reçue par un thread de com
        int portListening = ((IPEndPoint)socket.LocalEndPoint).Port;
        if (portListening == 10000)
        {
            Console.WriteLine("ERROR: Thread_com received message instead of serveur_main, IdMessage : " + packetReceived.IdMessage);
            packet = new Packet
            {
                Error = Tools.Errors.BadPort
            };
            return;
        }
        
        // Récupération du singleton gestionnaire
        GestionnaireThreadCom gestionnaire = GestionnaireThreadCom.GetInstance();

        // Attempt to update a player status within the room.
        var playerStatus = gestionnaire.ReadyPlayer(packetReceived.Data[0], packetReceived.IdPlayer);
        if (playerStatus != Tools.PlayerStatus.Success)
        {
            // Something went wrong.
            packet = new Packet
            {
                Error = Tools.Errors.RoomLeave
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
    /// <param name="socket">Socket <see cref="Socket" />.</param>
    public static void PlayerCheat(Packet packetReceived, ref Packet packet, Socket socket)
    {
        // Récupération du singleton gestionnaire
        GestionnaireThreadCom gestionnaire = GestionnaireThreadCom.GetInstance();
        
        // Attempt to update the number of times a player has cheated in a specific game.
        // Count will be incremented but player can keep playing until the limit is reached.
        var playerStatus = gestionnaire.CheatPlayer(packetReceived.Data[0], packetReceived.IdPlayer);
        
        // Player has reached the limit and must be kicked out.
        if (playerStatus == Tools.PlayerStatus.Kicked)
        {
            // TODO : player is kicked
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
    /// <param name="socket">Socket <see cref="Socket" />.</param>
    public static void GameStart(Packet packetReceived, ref Packet packet, Socket socket)
    {
        // Vérification que la communication est reçue par un thread de com
        int portListening = ((IPEndPoint)socket.LocalEndPoint).Port;
        if (portListening == 10000)
        {
            Console.WriteLine("ERROR: Thread_com received message instead of serveur_main, IdMessage : " + packetReceived.IdMessage);
            packet = new Packet
            {
                Error = Tools.Errors.BadPort
            };
            return;
        }

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
    /// <param name="socket">Socket <see cref="Socket" />.</param>
    public static void GameEnd(Packet packetReceived, ref Packet packet, Socket socket)
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
    /// <param name="socket">Socket <see cref="Socket" />.</param>
    public static void TuileDraw(Packet packetReceived, ref Packet packet, Socket socket)
    {
        // Vérification que la communication est reçue par un thread de com
        int portListening = ((IPEndPoint)socket.LocalEndPoint).Port;
        if (portListening == 10000)
        {
            Console.WriteLine("ERROR: Thread_com received message instead of serveur_main, IdMessage : " + packetReceived.IdMessage);
            packet = new Packet
            {
                Error = Tools.Errors.BadPort
            };
            return;
        }

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
    /// <param name="socket">Socket <see cref="Socket" />.</param>
    public static Tools.Errors TuilePlacement(Packet packetReceived, Socket socket)
    {
        // Vérification que la communication est reçue par un thread de com
        int portListening = ((IPEndPoint)socket.LocalEndPoint).Port;
        if (portListening == 10000)
        {
            Console.WriteLine("ERROR: Thread_com received message instead of serveur_main, IdMessage : " + packetReceived.IdMessage);
            packet = new Packet
            {
                Error = Tools.Errors.BadPort
            };
            return;
        }

        _ = packetReceived;
        // test si la tuile est bien a un emplacement valide
        return Tools.Errors.Unknown; // remplacer par Permission si validation du joueur sinon par Success
    }

    /// <summary>
    ///     placement d'un pion
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    /// <param name="socket">Socket <see cref="Socket" />.</param>
    public static Tools.Errors PionPlacement(Packet packetReceived, Socket socket)
    {
        // Vérification que la communication est reçue par un thread de com
        int portListening = ((IPEndPoint)socket.LocalEndPoint).Port;
        if (portListening == 10000)
        {
            Console.WriteLine("ERROR: Thread_com received message instead of serveur_main, IdMessage : " + packetReceived.IdMessage);
            packet = new Packet
            {
                Error = Tools.Errors.BadPort
            };
            return;
        }

        _ = packetReceived;
        // test si la Pion est bien a un emplacement valide
        return Tools.Errors.Unknown; // remplacer par Permission si validation du joueur sinon par Success
    }

    /// <summary>
    ///     validation d'un tour
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    /// <param name="socket">Socket <see cref="Socket" />.</param>
    public static void TourValidation(Packet packetReceived, ref Packet packet, Socket socket)
    {
        // Vérification que la communication est reçue par un thread de com
        int portListening = ((IPEndPoint)socket.LocalEndPoint).Port;
        if (portListening == 10000)
        {
            Console.WriteLine("ERROR: Thread_com received message instead of serveur_main, IdMessage : " + packetReceived.IdMessage);
            packet = new Packet
            {
                Error = Tools.Errors.BadPort
            };
            return;
        }

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
    /// <param name="socket">Socket <see cref="Socket" />.</param>
    public static void TimerExpiration(ref Packet packet, Socket socket)
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
    /// <param name="socket">Socket <see cref="Socket" />.</param>
    public static void CancelTuilePlacement(Packet packetReceived, ref Packet packet, Socket socket)
    {
        // Vérification que la communication est reçue par un thread de com
        int portListening = ((IPEndPoint)socket.LocalEndPoint).Port;
        if (portListening == 10000)
        {
            Console.WriteLine("ERROR: Thread_com received message instead of serveur_main, IdMessage : " + packetReceived.IdMessage);
            packet = new Packet
            {
                Error = Tools.Errors.BadPort
            };
            return;
        }

        _ = packetReceived;
        // cancel le placement de la tuile qui avais été validé par le joueur
        packet.Error = Tools.Errors.None; // remplacer par Permission si aucune erreur sinon par Unknown
    }

    /// <summary>
    ///     annulation d'un pion
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    /// <param name="socket">Socket <see cref="Socket" />.</param>
    public static void CancelPionPlacement(Packet packetReceived, ref Packet packet, Socket socket)
    {
        // Vérification que la communication est reçue par un thread de com
        int portListening = ((IPEndPoint)socket.LocalEndPoint).Port;
        if (portListening == 10000)
        {
            Console.WriteLine("ERROR: Thread_com received message instead of serveur_main, IdMessage : " + packetReceived.IdMessage);
            packet = new Packet
            {
                Error = Tools.Errors.BadPort
            };
            return;
        }

        _ = packetReceived;
        // cancel le placement du pion qui avais été validé par le joueur
        packet.Error = Tools.Errors.None; // remplacer par Permission si aucune erreur sinon par Unknown
    }
}
