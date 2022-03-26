namespace Fonction;

using Assets;
public class FonctionServer
{
    // vérifie si les informations de connecction sont valide, si oui return true sinon false
    public static bool IsConnection(Packet packet) //fonction a modifier pour la connection
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

    // deconnecte le joueur du serveur, true si deconnexion reussit , false sinon
    public static bool Deconnexion(Packet packet) // fonction pour deconnecter le joueur
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

    // vérifie si l'inscription est valide et si oui retur, true sinon false
    public static bool Inscription(Packet packet) //fonction a modifier pour l'inscription
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
    public static Packet Statistique(Packet packet) //fonction a modifier pour stat d'un joueur
    {
        packet.Data = "joueur 1 : 23/12 ";
        packet.Status = true;
        return packet;
    }

    // return la liste des rooms dispo dans un string ( ou une classe a crée )
    public static string ListeRoom(Packet packet)
    {
        var room = "r1 : 2 joueur";
        return room;
    }

    // met dans Data les info de la room ( ou une classe a crée )
    // modifie les info de la room cote server
    // Status = true si tout c'est bien passer sinon false
    public static Packet JoinRoom(Packet packet)
    {
        packet.Data = "joueur 1";
        packet.Status = true;
        return packet;
    }

    // return true si room bien quitter, false si erreur
    // modifier donnee room cote serveur
    public static bool LeaveRoom(Packet packet)
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
    public static bool ReadyRoom(Packet packet)
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
    public static Packet ParametreRoom(Packet packet)
    {
        packet.Status = true;
        return packet;
    }

    // envoye les changement de la room ( joueur join ou modification parametre ou joueur pret )
    public static string ChangementRoom(Packet packet)
    {
        var data = "joueur arriver";
        return data;
    }

}

