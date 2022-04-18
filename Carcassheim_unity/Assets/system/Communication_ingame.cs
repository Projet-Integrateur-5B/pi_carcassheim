using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Communication_ingame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // ==== DANS LE MENU ====

        // Boucle d'�coute du serveur

        // Type de r�ceptions : mise � jour des param�tres de partie (changement par le g�rant, ou qqun rejoint)

        // Type d'envois :
        //          - mise � jour des param�tres de la partie (si le joueur est le g�rant, � v�rifier c�t� client ET serv)
        //                  -> si modification envoy�e de la part d'un non g�rant, on r�pond un WarningCheat avec le serveur


        // ==== DANS LA GAME ====

        // Boucle d'�coute du serveur

        // Type de r�ceptions :
        //          - mise � jour affichage (coup d'un autre joueur, m�me non valid�)
        //          - r�ception d'un WarningCheat
        //          - indication de la part du serveur que c'est au client de jouer
        //                  -> d�but de la phase d'interactions entres placements du joueur et le serveur
        //                  -> se base sur des fonctions d'attente personnalis�es, o� le script attend que le joueur place ses tuiles/pions


        // Description de la phase d'interaction  :
        //      - R�ception des 3 tuiles 
        //      - V�rification qu'une des 3 est posable, si ce n'est pas le cas on pr�vient le serveur et on demande d'autres tuiles
        //      - Affichage de la tuile ainsi choisie par le client (la premi�re � �tre posable)
        //          -> MaJ graphique (tuile choisie)
        //      - (Attente d'une action du joueur : pose d'une tuile)
        //      - Envoie la pose de tuile au serveur et observe sa r�ponse (si le coup est ill�gal par exemple)
        //          -> MaJ graphique (tuile pos�e)
        //      - (Attente d'une action du joueur : pose d'un pion)
        //      - Envoie la pose de pion au serveur et observe sa r�ponse (si le coup est ill�gal par exemple)
        //          -> MaJ graphique (pion pos�)
        //      - (Attente d'une action du joueur : validation de son tour)
        //      - Envoie la validation du tour au serveur et observe sa r�ponse (si le coup est ill�gal par exemple)
        //          -> MaJ graphique (tour termin�)



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
