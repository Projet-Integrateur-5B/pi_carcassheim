namespace Server;

using Assets;

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
    public static Packet GetFromDatabase(Packet packetReceived)
    {
        // Initialize the packet to default.
        var packet = new Packet
        {

            // TODO : get what the client asked from the database or whatever
            IdMessage = packetReceived.IdMessage,
            Error = packetReceived.Error,
            IdPlayer = packetReceived.IdPlayer,
            IdRoom = packetReceived.IdRoom
        };

        // Check IdMessage : different action

        switch (packetReceived.IdMessage)
        {
            case Tools.IdMessage.Login:
                packet.Error = Login(packetReceived);
                break;
            case Tools.IdMessage.Signup:
                packet.Error = Signup(packetReceived);
                break;
            case Tools.IdMessage.Statistics:
                Statistics(packetReceived, ref packet);
                break;
            case Tools.IdMessage.RoomJoin:
                RoomJoin(packetReceived, ref packet);
                break;
            case Tools.IdMessage.RoomList:
                RoomList(packetReceived, ref packet);
                break;
            case Tools.IdMessage.RoomLeave:
                packet.Error = RoomLeave(packetReceived);
                break;
            case Tools.IdMessage.RoomReady:
                packet.Error = RoomReady(packetReceived);
                break;
            case Tools.IdMessage.RoomSettingsGet:
                packet.Data = RoomSettingsGet(packetReceived);
                break;
            case Tools.IdMessage.RoomSettingsSet:
                packet.Data = RoomSettingsSet(packetReceived);
                break;
            case Tools.IdMessage.RoomStart:
                RoomStart(packetReceived, ref packet);
                break;
            case Tools.IdMessage.TuileDraw:
                TuileDraw(packetReceived, ref packet);
                break;
            case Tools.IdMessage.TuilePlacement:
                packet.Error = TuilePlacement(packetReceived);
                break;
            case Tools.IdMessage.PionPlacement:
                packet.Error = PionPlacement(packetReceived);
                break;
            case Tools.IdMessage.TourValidation:
                TourValidation(packetReceived, ref packet);
                break;
            case Tools.IdMessage.TimerExpiration:
                TimerExpiration(ref packet);
                break;
            case Tools.IdMessage.LeaveGame:
                LeaveGame(packetReceived, ref packet);
                break;
            case Tools.IdMessage.EndGame:
                EndGame(packetReceived, ref packet);
                break;
            case Tools.IdMessage.Logout: // impossible
                packet.Error = Logout(packetReceived);
                break;
            case Tools.IdMessage.RoomCreate:
                RoomCreate(packetReceived, ref packet);
                break;
            case Tools.IdMessage.CancelTuilePlacement:
                CancelTuilePlacement(packetReceived, ref packet);
                break;
            case Tools.IdMessage.CancelPionPlacement:
                CancelPionPlacement(packetReceived, ref packet);
                break;
            case Tools.IdMessage.WarningCheat:
                WarningCheat(ref packet);
                break;
            case Tools.IdMessage.KickFromGame:
                KickFromGame(ref packet);
                break;
            case Tools.IdMessage.Default:
            default:
                packet.Error = Tools.Errors.Unknown;
                break;
        }

        return packet;
    }

    /// <summary>
    ///     connection au serveur du jeu
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    /// <returns></returns>
    public static Tools.Errors Login(Packet packetReceived)
    {
        // verifié ici si packetReceived.data[0] correspond bien a un pseudo et une adresse mail de la bdd et si packetReceived.Data[1] correspond au bon mdp
        // if a modifié pour return true si les infos sont valide
        if (packetReceived.Data[0] == "pseudo" && packetReceived.Data[1] == "mdp18")
        {
            return Tools.Errors.Success;
        }

        return Tools.Errors.Unknown;
    }

    /// <summary>
    ///     inscription au jeu
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    /// <returns></returns>
    public static Tools.Errors Signup(Packet packetReceived)
    {
        // verifie si les informations d'inscription sont valide ( ne sont pas les mêmes qu'un utilisateur deja inscrit
        // packetReceived.Data[0] = pseudp ; packetReceived.Data[1] = mdp ; packetReceived.Data[2] = mail ; packetReceived.Data[3] = date de naissance
        // if a modifié pour return true si info valide
        if (packetReceived.Data[0] == "pseudo" && packetReceived.Data[1] == "mdp18")
        {
            return Tools.Errors.Success;
        }

        return Tools.Errors.Unknown;
    }

    /// <summary>
    ///     envoye des statistiques d'un joueur
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void Statistics(Packet packetReceived, ref Packet packet)
    {
        _ = packetReceived;
        // id joueur dans packetReceived.IdPlayer
        // doit chercher les infos dans la bdd et les mettre dans packet.Data comme indiquer

        var list = new List<string>(packet.Data.ToList())
        {
            //remplacer ici les string par les valeurs de la bdd sous forme de string
            "XP",
            "Niveau",
            "Victoires",
            "Defaites",
            "Nbpartie"
        };
        packet.Data = list.ToArray();
        packet.Error = Tools.Errors.Success; // remplacer par Unknown si erreur
    }

    /// <summary>
    ///     joueur rejoignant une room
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void RoomJoin(Packet packetReceived, ref Packet packet)
    {
        // id room dans packetReceived.IdRoom
        // id joueur dans packetReceived.IdPlayer
        // ajouter le joueur a la partie dans le systeme de jeu
        // doit chercher les infos dans la bdd e la room et les mettre dans packetReceived.Data comme indiquer
        if (false) // entrer ici si erreur
        {
            packet.Error = Tools.Errors.Unknown; // remplacer par le bon code erreur
        }
        else
        {
            var list = new List<string>(packet.Data.ToList())
            {
                "10001" // nouveau port
            };
            packet.Data = list.ToArray();
            packet.Data = packet.Data.Concat(RoomSettingsGet(packetReceived)).ToArray();
            packet.Error = Tools.Errors.Success;
        }
    }

    /// <summary>
    ///     liste des rooms dispo
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void RoomList(Packet packetReceived, ref Packet packet)
    {
        _ = packetReceived;
        // doit chercher les infos dans la bdd pour les room dispo et les mettre dans packet.Data comme indiquer
        var list = new List<string>(packet.Data.ToList());
        for (var i = 0; i < 1; i++) // boucle a faire pour nb room
        {
            list.Add("id room"); // id room
            list.Add("pseudo hote"); // pseudo de l'hote de la room
            list.Add("nb joueur present"); // nb joueur deja present
            list.Add("nb joueur max"); // nb joueur max de la room
        }

        packet.Data = list.ToArray();
        packet.Error = Tools.Errors.Success; //remplacer par Unknown si erreur
    }

    /// <summary>
    ///     joueur quitant une room
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    public static Tools.Errors RoomLeave(Packet packetReceived)
    {
        // supprimer le joueur de la prtie dans le systeme de jeu
        // return true si bien reussit et false si joueur non retirer de la partie ( si erreur en gros)
        if (packetReceived.IdPlayer == 999)
        {
            return Tools.Errors.Success;
        }

        return Tools.Errors.Unknown;
    }

    /// <summary>
    ///     joueur qui se met prêt
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    public static Tools.Errors RoomReady(Packet packetReceived)
    {
        // met le joueur prêt dans le jeu ( true si bien passer, false sinon )
        // id room dans packet.IdRoom
        // id joueur dans packet.IdPlayer
        if (packetReceived.IdPlayer == 999)
        {
            return Tools.Errors.Success;
        }

        return Tools.Errors.Unknown;
    }

    /// <summary>
    ///     recup parametre d'une room
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    public static string[] RoomSettingsGet(Packet packetReceived)
    {
        _ = packetReceived;
        // id room dans packetReceived.IdRoom
        // doit chercher les infos dans la bdd de la room et les mettre dans packet.Data comme indiquer
        var retour = new Packet();
        var list = new List<string>(retour.Data.ToList())
        {
            "nb joueur max",
            "partie privé ou public",
            "mode de la partie",
            "nb tuile",
            "nb pion",
            "timer partie",
            "timer par joueur",
            "nb score max" // parametre de la room a donner ici ( mettre a -1 si non remplit)
        };
        for (var i = 0; i < 4; i++) // boucle a faire pour nb joueur present
        {
            list.Add("pseudo joueur"); // pseudo du joueur X
        }

        retour.Data = list.ToArray();
        return retour.Data;
    }

    /// <summary>
    ///     modif parametre d'une room
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    public static string[] RoomSettingsSet(Packet packetReceived)
    {
        _ = packetReceived;
        // info a modif dans packetReceived.Data
        // id room dans packetReceived.IdRoom
        // doit chercher les infos dans la bdd de la room et les mettre dans packet.Data comme indiquer
        var retour = new Packet();
        var list = new List<string>(retour.Data.ToList())
        {
            "nb joueur max",
            "partie privé ou public",
            "mode de la partie",
            "nb tuile",
            "nb pion",
            "timer partie",
            "timer par joueur",
            "nb score max" // parametre de la room a donner ici ( mettre a -1 si non remplit)
        };
        for (var i = 0; i < 4; i++) // boucle a faire pour nb joueur present
        {
            list.Add("pseudo joueur"); // pseudo du joueur X
        }

        retour.Data = list.ToArray();
        return retour.Data;
    }

    /// <summary>
    ///     lancement d'une room
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void RoomStart(Packet packetReceived, ref Packet packet)
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
        packet.Error = Tools.Errors.Success; // remplacer par Unknown si erreur
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
        packet.Error = Tools.Errors.Success; // remplacer par Unknown si erreur
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
    ///     joueur qui quitte une room
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void LeaveGame(Packet packetReceived, ref Packet packet)
    {
        _ = packetReceived;
        // joueur quitte la partie ( le supprimer de la partie du coup ? )
        var list = new List<string>(packet.Data.ToList())
        {
            "pseudo" // pseudo ou ID du joueur qui a leave la game
        };
        packet.Data = list.ToArray();
        packet.Error = Tools.Errors.None;
    }

    /// <summary>
    ///     fin de la partie
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void EndGame(Packet packetReceived, ref Packet packet)
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
        packet.Error = Tools.Errors.Success; // remplacer par Unknown si erreur
    }

    /// <summary>
    ///     deconnection d'un joueur
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    public static Tools.Errors Logout(Packet packetReceived)
    {
        _ = packetReceived;
        return Tools.Errors.None;
    }

    /// <summary>
    ///     creation d'une room
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    /// <param name="packet">Instance of <see cref="Packet" /> to send.</param>
    public static void RoomCreate(Packet packetReceived, ref Packet packet)
    {
        _ = packetReceived;
        // creation d'une nouvelle room par un joueur
        var list = new List<string>(packet.Data.ToList())
        {
            "10001", // nouveau numero port
            "IdRoom" // id de la room
        };
        packet.Data = list.ToArray();
        packet.Error = Tools.Errors.Success; // remplacer par Unknown si erreur
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

    /// <summary>
    ///     warning envoyer quand suspision de triche
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    public static void WarningCheat(ref Packet packet)
    {
        _ = packet;
        // previens au joueur que c'est un vilain tricheur
        packet.Error = Tools.Errors.Success;
    }

    /// <summary>
    ///     deconnecte un joueur afk d'une partie et du serveur
    /// </summary>
    /// <param name="packetReceived">Instance of <see cref="Packet" /> to received.</param>
    public static void KickFromGame(ref Packet packet)
    {
        _ = packet;
        // kick le joueur parce qu'il est afk depuis trop longtemps
        packet.Error = Tools.Errors.Success;
    }
}
