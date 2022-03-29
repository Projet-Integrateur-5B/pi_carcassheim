namespace Server;

using Assets;

public partial class Server
{
    // vérifie si les informations de connecction sont valide, si oui return true sinon false
    public static bool Connection(Packet packet) //fonction a modifier pour la connection
    {
        if (packet.Data[1] == "mdp18" && packet.Data[0] == "pseudo")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // deconnecte le joueur du serveur, true si deconnexion reussit , false sinon
    public static bool Disconnection(Packet packet) // fonction pour deconnecter le joueur
    {
        if (packet.IdPlayer == 999)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // vérifie si l'inscription est valide et si oui return true sinon false
    public static bool Signup(Packet packet) //fonction a modifier pour l'inscription
    {
        if (packet.IdPlayer == 999)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // rentre les stats du joueur dans packet.Data ( string ) ou dans une nouvelle classe ( a crée et modifier alors )
    // id du joueur présent dans packet.IdPlayer
    // Status = true si aucune erreur, sinon false
    public static Packet Statistics(Packet packet) //fonction a modifier pour stat d'un joueur
    {
        Array.Clear(packet.Data);
        var retour = "joueur 1 : 23/12 ";
        var list = new List<string>(packet.Data.ToList())
        {
            retour
        };
        packet.Data = list.ToArray();

        packet.Status = true;
        return packet;
    }

    // return la liste des rooms dispo dans un string ( ou une classe a crée )
    public static Packet RoomJoin(Packet packet)
    {
        Array.Clear(packet.Data);
        var retour = "r1 : 2 joueur";
        var list = new List<string>(packet.Data.ToList())
        {
            retour
        };
        packet.Data = list.ToArray();
        packet.Status = true;
        return packet;
    }

    // met dans Data les info de la room ( ou une classe a crée )
    // modifie les info de la room cote server
    // Status = true si tout c'est bien passer sinon false
    public static Packet RoomList(Packet packet)
    {
        Array.Clear(packet.Data);
        var retour = " room 1";
        var list = new List<string>(packet.Data.ToList())
        {
            retour
        };
        packet.Data = list.ToArray();
        packet.Status = true;
        return packet;
    }

    // return true si room bien quitter, false si erreur
    // modifier donnee room cote serveur
    public static bool RoomLeave(Packet packet)
    {
        if (packet.IdPlayer == 999)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // met le joueur prêt ( true si bien passer, false sinon )
    public static bool RoomReady(Packet packet)
    {
        if (packet.IdPlayer == 999)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // modifie les parametres de la room
    // status true si parametre bien modifier, false si erreur
    public static Packet RoomSettings(Packet packet)
    {
        Array.Clear(packet.Data);
        var retour = " nb personne : 1";
        var list = new List<string>(packet.Data.ToList())
        {
            retour
        };
        packet.Data = list.ToArray();
        packet.Status = true;
        return packet;
    }

    // envoye les changement de la room ( joueur join ou modification parametre ou joueur pret )
    public static Packet RoomEdit(Packet packet)
    {
        packet.Status = true;
        Array.Clear(packet.Data);
        var retour = "joueur arriver";
        var list = new List<string>(packet.Data.ToList())
        {
            retour
        };
        packet.Data = list.ToArray();
        return packet;
    }
}
