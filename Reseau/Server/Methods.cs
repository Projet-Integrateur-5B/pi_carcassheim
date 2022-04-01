namespace Server;

using Assets;

public partial class Server
{
    public static Packet GetFromDatabase(IAsyncResult ar)
    {
        var packet = new Packet();

        var state = (StateObject?)ar.AsyncState;
        if (state is null)
        {
            // TODO : state is null
            return packet;
        }

        // TODO : get what the client asked from the database or whatever
        switch (state.Packet.IdMessage)
        {
            case IdMessage.Connection:
                packet.Status = Connection(packet);
                Array.Clear(packet.Data);
                break;
            case IdMessage.Signup:
                packet.Status = Signup(packet);
                Array.Clear(packet.Data);
                break;
            case IdMessage.Statistics:
                packet = Statistics(packet);
                break;
            case IdMessage.RoomList:
                packet = RoomList(packet);
                break;
            case IdMessage.RoomJoin:
                packet = RoomJoin(packet);
                break;
            case IdMessage.RoomLeave:
                packet.Status = RoomLeave(packet);
                Array.Clear(packet.Data);
                break;
            case IdMessage.RoomReady:
                packet.Status = RoomReady(packet);
                Array.Clear(packet.Data);
                break;
            case IdMessage.RoomSettings:
                packet = RoomSettings(packet);
                break;
            case IdMessage.RoomStart:
                packet = RoomStart(packet);
                break;
            case IdMessage.Disconnection: // impossible
                Array.Clear(packet.Data);
                break;
            case IdMessage.Default:
            default:
                packet.Status = false;
                break;
        }
        return packet;
    }

    public static bool Connection(Packet packet)
    {
        // verifié ici si packet.data[0] correspond bien a un pseudo et une adresse mail de la bdd et si packet.Data[1] correspond au bon mdp
        // if a modifié pour return true si les infos sont valide
        if (packet.Data[0] == "pseudo" && packet.Data[1] == "mdp18")
        {
            return true;
        }

        return false;
    }

    // ne pas toucher cette fonction
    public static bool Disconnection(Packet packet)
    {
        if (packet.IdPlayer == 999)
        {
            return true;
        }

        return false;
    }

    public static bool Signup(Packet packet)
    {
        // verifie si les informations d'inscription sont valide ( ne sont pas les mêmes qu'un utilisateur deja inscrit
        // packet.Data[0] = pseudp ; packet.Data[1] = mdp ; packet.Data[2] = mail ; packet.Data[3] = date de naissance
        // if a modifié pour return true si info valide
        if (packet.Data[0] == "pseudo" && packet.Data[1] == "mdp18")
        {
            return true;
        }

        return false;
    }

    public static Packet Statistics(Packet packet)
    {
        // id joueur dans packet.IdPlayer
        // doit chercher les infos dans la bdd et les mettre dans packet.Data comme indiquer
        var retour = new Packet(packet.Type, packet.IdRoom, packet.IdMessage, true, packet.IdPlayer,
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
        retour.Status = true; // remplacer par false si erreur
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
            retour.Status = false;
        }

        return retour;
    }

    public static Packet RoomList(Packet packet)
    {
        // doit chercher les infos dans la bdd pour les room dispo et les mettre dans packet.Data comme indiquer
        var retour = new Packet(packet.Type, packet.IdRoom, packet.IdMessage, true, packet.IdPlayer,
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
        retour.Status = true; //remplacer par false si erreur
        return retour;
    }

    public static bool RoomLeave(Packet packet)
    {
        // supprimer le joueur de la prtie dans le systeme de jeu
        // return true si bien reussit et false si joueur non retirer de la partie ( si erreur en gros)
        if (packet.IdPlayer == 999)
        {
            return true;
        }

        return false;
    }

    public static bool RoomReady(Packet packet)
    {
        // met le joueur prêt dans le jeu ( true si bien passer, false sinon )
        // id room dans packet.IdRoom
        // id joueur dans packet.IdPlayer
        if (packet.IdPlayer == 999)
        {
            return true;
        }

        return false;
    }

    public static Packet RoomSettings(Packet packet)
    {
        // id room dans packet.IdRoom
        // doit chercher les infos dans la bdd de la room et les mettre dans packet.Data comme indiquer
        var retour = new Packet(packet.Type, packet.IdRoom, packet.IdMessage, true, packet.IdPlayer,
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
        for (var i = 0; i < 1; i++) // boucle a faire pour nb joueur present
        {
            list.Add("pseudo joueur"); // pseudo du joueur X
        }

        retour.Data = list.ToArray();
        retour.Status = true; // remplacer par false si erreur
        return retour;
    }

    public static Packet RoomStart(Packet packet)
    {
        // lancement de la partie
        // id room dans packet.IdRoom
        packet.Status = true; // mettre sur false si erreur dans lancement
        return packet;
    }
}
