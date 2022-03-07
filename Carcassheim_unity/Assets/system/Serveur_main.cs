using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class Serveur_main : MonoBehaviour
{
    private static List<Thread_communication> _lst_obj_threads_com;
    private static List<Thread> _lst_threads_com;

    private static int _compteur_id_thread_com;

    void Start()
    {

        _lst_threads_com = new List<Thread>();

        _lst_obj_threads_com = new List<Thread_communication>();

        _compteur_id_thread_com = 0;

        int fonctionReseauRecept = 0;

        // RESEAU - boucle de réception des communications
        while(fonctionReseauRecept != 0){

            int typeMsg = 0; // Dépendra du type : identification, connexion, etc

            if(typeMsg == 1){ // Réception d'une identification (connexion login)

                string login = "Test_DEBUG";
                string mdp = "Test_DEBUG";


                // Création d'un thread temporaire de gestion de la requête 
                Thread_connexion thread_connexion = new Thread_connexion(login,mdp);

                // Créer un objet pour chaque threads, comme ça pas de pb d'un attribut utilisé par plusieurs threads
                Thread nouv_thread_connexion = new Thread(new ThreadStart(thread_connexion.Lancement_thread_connexion));
                nouv_thread_connexion.Start();

                // A FAIRE - Fonction de redirection vers thread de connexion

                

            }
            else if(typeMsg == 2){ // Réception d'une connexion à une partie
                
            }
            else if(typeMsg == 3){ // Réception d'une création de partie (un if dans le while de reception global)

                // TEMP - DEBUG
                int port_partie = 1;
                    
                if(_lst_threads_com.Count == 0 && _lst_obj_threads_com.Count == 0){ // Aucun thread de comm n'existe
                    
                    Creation_thread_com(port_partie);

                    // A FAIRE - Fonction de redirection vers thread de com
                }
                else{
                    bool thread_com_trouve = false;

                    // Parcours des différents threads de communication pour trouver un qui gère < 5 parties
                    foreach(Thread_communication thread_com_iterateur in _lst_obj_threads_com){
                        lock(thread_com_iterateur){
                            if(thread_com_iterateur.Get_nb_parties_gerees() < 5){

                                thread_com_trouve = true;
                        
                                // A FAIRE - Fonction (dans le thread de com) de création d'accueil ET DE PARTIE QUOI
                                // Exemple:
                                /*
                                int nouvel_id = 2; // BDD - Rajouter requête pour récupérer le prochain id dispo
                                thread_com_iterateur.add_partie_geree();
                                */

                                // A FAIRE - Fonction de redirection vers thread de com
                            }
                        }
                    }

                    // Si aucun des threads n'est libre pour héberger une partie de plus
                    if(thread_com_trouve == false){

                        Creation_thread_com(port_partie);

                        // A FAIRE - Fonction (dans le thread de com) de création d'accueil

                        // A FAIRE - Fonction de redirection vers thread de com

                    }

                }
        
                // Passage de la requête vers le thread de communication lié
            }
                  
        }
        // RESEAU - fin boucle de réception des communications


        // Test threads 
        Debug.Log("Ceci est un test");

        // Création d'un thread de com de port 1
        Creation_thread_com(1);

        // Création d'un thread de com de port 2
        Creation_thread_com(2);

        // On indique au premier qu'il gère 1 partie et au second 2
        _lst_obj_threads_com[0].Add_partie_geree(1);

        _lst_obj_threads_com[1].Add_partie_geree(2);
        _lst_obj_threads_com[1].Add_partie_geree(3);




        // Prévenir tous les threads que le serveur ferme 

        // Fermeture de tous les threads
        foreach(Thread thread in _lst_threads_com){
            thread.Join();
        }

    }

    private void Creation_thread_com(int port_partie){

        Thread_communication thread_com = new Thread_communication(port_partie,_compteur_id_thread_com);
        _compteur_id_thread_com++;
        Thread nouv_thread = new Thread(new ThreadStart(thread_com.Lancement_thread_com));

        _lst_obj_threads_com.Add(thread_com);
        _lst_threads_com.Add(nouv_thread);

        nouv_thread.Start();

    }
}