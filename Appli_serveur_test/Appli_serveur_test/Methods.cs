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
        packet.IdPlayer = state.Packet.IdPlayer;

        // Check IdMessage : different action

        switch (state.Packet.IdMessage)
        {
            case Tools.IdMessage.Login:
                packet.Error = Connection(state.Packet);
                break;
            case Tools.IdMessage.Signup:
                packet.Error = Signup(state.Packet);
                break;
            case Tools.IdMessage.Statistics:
                packet = Statistics(state.Packet);
                break;
            case Tools.IdMessage.RoomList:
                packet = RoomList(state.Packet);
                break;
            case Tools.IdMessage.RoomJoin:
                packet = RoomJoin(state.Packet);
                break;
            case Tools.IdMessage.RoomLeave:
                packet.Error = RoomLeave(state.Packet);
                break;
            case Tools.IdMessage.RoomReady:
                packet.Error = RoomReady(state.Packet);
                break;
            case Tools.IdMessage.RoomSettings:
                packet = RoomSettings(state.Packet);
                break;
            case Tools.IdMessage.RoomStart:
                packet = RoomStart(state.Packet);
                break;
            case Tools.IdMessage.TuileDraw:
                packet = Statistics(state.Packet);
                break;
            case Tools.IdMessage.TuilePlacement:
                packet = Statistics(state.Packet);
                break;
            case Tools.IdMessage.PionPlacement:
                packet = Statistics(state.Packet);
                break;
            /*case Tools.IdMessage.CancelPlacement:
                packet = Statistics(state.Packet);
                break;*/
            case Tools.IdMessage.TourValidation:
                packet = Statistics(state.Packet);
                break;
            case Tools.IdMessage.TimerExpiration:
                packet = Statistics(state.Packet);
                break;
            case Tools.IdMessage.LeaveGame:
                packet = Statistics(state.Packet);
                break;
            case Tools.IdMessage.EndGame:
                packet = Statistics(state.Packet);
                break;
            case Tools.IdMessage.Logout: // impossible
                break;
            case Tools.IdMessage.RoomCreate:
                break;
            case Tools.IdMessage.CancelTuilePlacement:
                break;
            case Tools.IdMessage.CancelPionPlacement:
                break;
            case Tools.IdMessage.WarningCheat:
                break;
            case Tools.IdMessage.KickFromGame:
                break;
            case Tools.IdMessage.Default:
            default:
                packet.Error = Tools.Errors.Unknown;
                break;
        }

        return packet;
    }

    public static Tools.Errors Connection(Packet packet)
    {
        // verifié ici si packet.data[0] correspond bien a un pseudo et une adresse mail de la bdd et si packet.Data[1] correspond au bon mdp
        // if a modifié pour return true si les infos sont valide
        if (packet.Data[0] == "pseudo" && packet.Data[1] == "mdp18")
        {
            return Tools.Errors.None;
        }

        return Tools.Errors.Unknown;
    }

    // ne pas toucher cette fonction
    public static Tools.Errors Disconnection(Packet packet)
    {
        if (packet.IdPlayer == 999)
        {
            return Tools.Errors.None;
        }

        return Tools.Errors.Unknown;
    }

    public static Tools.Errors Signup(Packet packet)
    {
        // verifie si les informations d'inscription sont valide ( ne sont pas les mêmes qu'un utilisateur deja inscrit
        // packet.Data[0] = pseudp ; packet.Data[1] = mdp ; packet.Data[2] = mail ; packet.Data[3] = date de naissance
        // if a modifié pour return true si info valide
        if (packet.Data[0] == "pseudo" && packet.Data[1] == "mdp18")
        {
            return Tools.Errors.None;
        }

        return Tools.Errors.Unknown;
    }

    public static Packet Statistics(Packet packet)
    {
        // id joueur dans packet.IdPlayer
        // doit chercher les infos dans la bdd et les mettre dans packet.Data comme indiquer
        var retour = new Packet(packet.Type, packet.IdMessage, true, packet.IdPlayer,
            Array.Empty<string>());
        var list = new List<string>(retour.Data.ToList())
        {
            //remplacer ici les string par les valeurs de la bdd sous forme de string
            "XP",
            "Niveau",
            "Victoires",
            "Defaites",
            "Nbpartie"
        };
        retour.Data = list.ToArray();
        retour.Error = Tools.Errors.None; // remplacer par "false" si erreur
        return retour;
    }

    public static Packet RoomJoin(Packet packet)
    {
        // id room dans packet.IdRoom
        // id joueur dans packet.IdPlayer
        // ajouter le joueur a la partie dans le systeme de jeu
        // doit chercher les infos dans la bdd e la room et les mettre dans packet.Data comme indiquer
        var retour = RoomSettings(packet);
        if (false) // entrer ici si erreur
        {
            retour.Error = Tools.Errors.Unknown; // remplacer par le bon code erreur
        }

        var list = new List<string>(retour.Data.ToList())
        {
            "10000" // nouveau port
        };

        retour.Data = list.ToArray();
        return retour;
    }

    public static Packet RoomList(Packet packet)
    {
        // doit chercher les infos dans la bdd pour les room dispo et les mettre dans packet.Data comme indiquer
        var retour = new Packet(packet.Type, packet.IdMessage, true, packet.IdPlayer,
            Array.Empty<string>());
        var list = new List<string>(retour.Data.ToList());
        for (var i = 0; i < 1; i++) // boucle a faire pour nb room
        {
            list.Add("id room"); // id room
            list.Add("pseudo hote"); // pseudo de l'hote de la room
            list.Add("nb joueur present"); // nb joueur deja present
            list.Add("nb joueur max"); // nb joueur max de la room
        }

        retour.Data = list.ToArray();
        retour.Error = Tools.Errors.None; //remplacer par "false" si erreur
        return retour;
    }

    public static Tools.Errors RoomLeave(Packet packet)
    {
        // supprimer le joueur de la prtie dans le systeme de jeu
        // return true si bien reussit et false si joueur non retirer de la partie ( si erreur en gros)
        if (packet.IdPlayer == 999)
        {
            return Tools.Errors.None;
        }

        return Tools.Errors.Unknown;
    }

    public static Tools.Errors RoomReady(Packet packet)
    {
        // met le joueur prêt dans le jeu ( true si bien passer, false sinon )
        // id room dans packet.IdRoom
        // id joueur dans packet.IdPlayer
        if (packet.IdPlayer == 999)
        {
            return Tools.Errors.None;
        }

        return Tools.Errors.Unknown;
    }

    public static Packet RoomSettings(Packet packet)
    {
        // id room dans packet.IdRoom
        // doit chercher les infos dans la bdd de la room et les mettre dans packet.Data comme indiquer
        var retour = new Packet(packet.Type, packet.IdMessage, true, packet.IdPlayer,
            Array.Empty<string>());
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
        retour.Error = Tools.Errors.None; // remplacer par "false" si erreur
        return retour;
    }

    public static Packet RoomStart(Packet packet)
    {
        // lancement de la partie
        // id room dans packet.IdRoom
        // start le timer general
        // retourne l'ordre des joueurs et l'ordre des tuiles
        var retour = new Packet(packet.Type, packet.IdMessage, true, packet.IdPlayer,
            Array.Empty<string>());
        var list = new List<string>(retour.Data.ToList());
        for (var i = 0; i < 4; i++) // boucle a faire pour nb joueur present dans l'ordre de tirage
        {
            list.Add("pseudo joueur"); // pseudo du joueur X
        }

        retour.Data = list.ToArray();
        retour.Error = Tools.Errors.None; // remplacer par "false" si erreur
        return retour;
    }

    public static Packet TuileDraw(Packet packet)
    {
        // tirage de 3 tuiles pour le joueur
        var retour = new Packet(packet.Type, packet.IdMessage, true, packet.IdPlayer,
            Array.Empty<string>());
        var list = new List<string>(retour.Data.ToList())
        {
            "numero tuile", "numero tuile", "numero tuile"
        };

        retour.Data = list.ToArray();
        retour.Error = Tools.Errors.None; // remplacer par "false" si erreur
        return retour;
    }

    public static Packet TuilePlacement(Packet packet)
    {
        // test si la tuile est bien a un emplacement valide
        var retour = new Packet(packet.Type, packet.IdMessage, true, packet.IdPlayer,
            Array.Empty<string>())
        {
            Error = Tools.Errors.None, // remplacer par "false" si erreur
            //Permission = 1 // 1 si tuile accepter sinon 0
        };
        return retour;
    }

    public static Packet PionPlacement(Packet packet)
    {
        // test si le pion est bien a un emplacement valide
        var retour = new Packet(packet.Type, packet.IdMessage, true, packet.IdPlayer,
            Array.Empty<string>())
        {
            Error = Tools.Errors.None, // remplacer par "false" si erreur
            //Permission = 1 // 1 si pion accepter sinon 0
        };
        return retour;
    }

    public static Packet CancelPlacement(Packet packet)
    {
        // annule le placement de la tuile
        var retour = new Packet(packet.Type, packet.IdMessage, true, packet.IdPlayer,
            packet.Data)
        {
            Error = Tools.Errors.None, // remplacer par "false" si erreur dans l'annulation
        };
        return retour;
    }

    public static Packet TourValidation(Packet packet)
    {
        // valide le tour
        // calculer les points du tour avec les zone qui viennent de se fermé
        var retour = new Packet(packet.Type, packet.IdMessage, true, packet.IdPlayer,
            packet.Data)
        {
            Error = Tools.Errors.None, // remplacer par "false" si erreur dans la validation
        };
        var list =
            new List<string>(retour.Data
                    .ToList()) // a faire seulement si une zone se ferme ( d'apres ce que j'ai compris )
                {
                    "position X", // position pion où la zone a été fermé
                    "position Y"
                };
        retour.Data = list.ToArray();
        return retour;
    }

    public static Packet TimerExpiration(Packet packet)
    {
        // timer du joueur expirer, on passe au prochaine joueur
        var retour = new Packet(packet.Type, packet.IdMessage, true, packet.IdPlayer,
            packet.Data)
        { Error = Tools.Errors.None };
        return retour;
    }

    public static Packet LeaveGame(Packet packet)
    {
        // joueur quitte la partie ( le supprimer de la partie du coup ? )
        var retour = new Packet(packet.Type, packet.IdMessage, true, packet.IdPlayer,
            packet.Data)
        { Error = Tools.Errors.None };
        var list = new List<string>(retour.Data.ToList())
        {
            "pseudo" // pseudo ou ID du joueur qui a leave la game
        };
        retour.Data = list.ToArray();
        return retour;
    }

    public static Packet EndGame(Packet packet)
    {
        // partie finit, on choisis un emplacement X Y où sera afficher les scores
        // donner les score des joueurs ( peut importe l'ordre )
        var retour = new Packet(packet.Type, packet.IdMessage, true, packet.IdPlayer,
            Array.Empty<string>());
        var list = new List<string>(retour.Data.ToList())
        {
            "position X", // position afficher X
            "position Y" // position afficher Y
        };
        for (var i = 0; i < 4; i++) // boucle a faire pour nb joueur present
        {
            list.Add("pseudo joueur"); // pseudo du joueur X
            list.Add("score"); // score du joueur X
        }

        retour.Data = list.ToArray();
        retour.Error = Tools.Errors.None; // remplacer par "false" si erreur
        return retour;
    }
}
