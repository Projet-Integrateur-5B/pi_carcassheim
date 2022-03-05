using System;
using System.Threading;
using System.Collections.Generic;
using Thread_communication;
using Thread_connexion;

public static class Serveur_main 
{
    private List<Thread_communication> _lst_threads_com;

    static void Main(string[] args){

        _lst_threads_com = new List<Thread>();

        // RESEAU - boucle de réception des communications

            // Réception d'une identification (connexion login)

                // Création d'un thread temporaire de gestion de la requête 

                // Créer un objet pour chaque threads, comme ça pas de pb d'un attribut utilisé par plusieurs threads
                Thread nouv_thread_connexion = new Thread(new ThreadStart(thread_connexion));
                nouv_thread_connexion.Start();

                // A FAIRE - Fonction de redirection vers thread de connexion

                //(BDD - Vérifie si le login et mdp sont bons)
            

            // Réception d'une connexion à une partie
            
            int port_partie;

            
            // Réception d'une création de partie (un if dans le while de reception global)
            
                if(_lst_threads_com.Count() == 0){ // Aucun thread de comm n'existe
                    
                    Thread_communication thread_com = new Thread_communication(port_partie);
                    Thread nouv_thread = new Thread(new ThreadStart(thread_com.lancement_thread_com));
                    _lst_threads_com.Add(thread_com);
                    nouv_thread.Start();

                    // A FAIRE - Fonction de redirection vers thread de com
                }
                else{
                    bool thread_com_trouve = false;

                    // Parcours des différents threads de communication pour trouver un qui gère < 5 parties
                    foreach(Thread_communication thread_com in _lst_threads_com){
                        lock(thread_com){
                            if(thread_com.get_parties_gerees() < 5){

                                thread_com_trouve = true;
                                thread_com.add_partie_geree();

                                // A FAIRE - Fonction (dans le thread de com) de création d'accueil

                                // A FAIRE - Fonction de redirection vers thread de com
                            }
                        }
                    }

                    // Si aucun des threads n'est libre pour héberger une partie de plus
                    if(thread_com_trouve == false){

                        Thread_communication thread_com = new Thread_communication(port_partie);
                        Thread nouv_thread = new Thread(new ThreadStart(thread_com.lancement_thread_com));
                        _lst_threads_com.Add(thread_com);
                        nouv_thread.Start();

                        // A FAIRE - Fonction (dans le thread de com) de création d'accueil

                        // A FAIRE - Fonction de redirection vers thread de com

                    }

                }
                

                

                // Passage de la requête vers le thread de communication lié
                
            

        // RESEAU - fin boucle de réception des communications


        

        // Prévenir tous les threads que le serveur ferme 


        // Fermeture de tous les threads
        foreach(Thread thread_com in _lst_threads_com){
            thread_com.Join();
        }

    }

    public void thread_connexion(){

    }
}