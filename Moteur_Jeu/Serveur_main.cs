using System;
using System.Threading;
using System.Collections.Generic;
using Thread_communication;

public static class Serveur_main 
{
    private List<Thread> _lst_threads_com;

    static void Main(string[] args){

        _lst_threads_com = new List<Thread>();

        // RESEAU - boucle de réception des communications

            // Réception d'une identification (connexion login)

            // Création d'un thread temporaire de gestion de la requête 
                //(BDD - Vérifie si le login et mdp sont bons)

            // Réception d'une connexion à une partie
            // Réception d'une création de partie
            int port_partie;

            
                // SI il n'existe aucun thread de communication
                if(_lst_threads_com.Count() == 0){
                    
                    Thread_communication manipulateur_threads = new Thread_communication(port_partie);
                    Thread nouv_thread = new Thread(new ThreadStart(manipulateur_threads.lancement_thread_com));
                    _lst_threads_com.Add(nouv_thread);
                    nouv_thread.Start();
                }

                // Parcours des différents threads de communication pour trouver un qui gère < 5 parties
                foreach(Thread threa_com in _lst_threads_com){
                    //if()
                }

                // Passage de la requête vers le thread de communication lié
                
            

        // RESEAU - fin boucle de réception des communications


        

        // Prévenir tous les threads que le serveur ferme 


        // Fermeture de tous les threads
        foreach(Thread threa_com in _lst_threads_com){
            threa_com.Join();
        }

    }
}