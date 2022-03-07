using System;
using System.Threading;

public class Thread_connexion
{
    // Attributs

    private string _login;
    private string _mdp;

    // Constructeur

    public Thread_connexion(string login, string mdp){
        _login = login;
        _mdp = mdp;
    }
    
    // Getters et setters


    // Méthodes

    public void Lancement_thread_connexion()
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