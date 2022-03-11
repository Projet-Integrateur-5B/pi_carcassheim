using System;
using System.Threading;

public class Thread_identification
{
    // Attributs

    private string _login;
    private string _mdp;
    private string _client; //TEMPORAIRE   // RESEAU - Définir objet "client"

    // Constructeur

    public Thread_identification(string login, string mdp, string client)
    {
        _login = login;
        _mdp = mdp;
        _client = client;
    }
    
    // Getters et setters


    // Méthodes

    public void Lancement_thread_identification()
    {
        bool identifiants_valides = false;

        // BDD - Requête BDD pour tester la validité


        if (!identifiants_valides){ // Identification échouée 

            // RESEAU - Communique avec le client pour lui dire que la connexion est refusée
        }
        else{ // Identification réussite

            // RESEAU - Communique avec le client pour lui dire que la connexion est acceptée

            // BDD - Récupère les informations du joueur

            // RESEAU - Envoie les informations du joueur au client

                
        }
    }
}