using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Communication_ingame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // ==== DANS LE MENU ====

        // Boucle d'écoute du serveur

        // Type de réceptions : mise à jour des paramètres de partie (changement par le gérant, ou qqun rejoint)

        // Type d'envois :
        //          - mise à jour des paramètres de la partie (si le joueur est le gérant, à vérifier côté client ET serv)
        //                  -> si modification envoyée de la part d'un non gérant, on répond un WarningCheat avec le serveur


        // ==== DANS LA GAME ====

        // Boucle d'écoute du serveur

        // Type de réceptions :
        //          - mise à jour affichage (coup d'un autre joueur, même non validé)
        //          - réception d'un WarningCheat
        //          - indication de la part du serveur que c'est au client de jouer
        //                  -> début de la phase d'interactions entres placements du joueur et le serveur
        //                  -> se base sur des fonctions d'attente personnalisées, où le script attend que le joueur place ses tuiles/pions


        // Description de la phase d'interaction  :
        //      - Réception des 3 tuiles 
        //      - Vérification qu'une des 3 est posable, si ce n'est pas le cas on prévient le serveur et on demande d'autres tuiles
        //      - Affichage de la tuile ainsi choisie par le client (la première à être posable)
        //          -> MaJ graphique (tuile choisie)
        //      - (Attente d'une action du joueur : pose d'une tuile)
        //      - Envoie la pose de tuile au serveur et observe sa réponse (si le coup est illégal par exemple)
        //          -> MaJ graphique (tuile posée)
        //      - (Attente d'une action du joueur : pose d'un pion)
        //      - Envoie la pose de pion au serveur et observe sa réponse (si le coup est illégal par exemple)
        //          -> MaJ graphique (pion posé)
        //      - (Attente d'une action du joueur : validation de son tour)
        //      - Envoie la validation du tour au serveur et observe sa réponse (si le coup est illégal par exemple)
        //          -> MaJ graphique (tour terminé)



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
