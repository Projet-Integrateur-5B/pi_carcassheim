using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class Serveur_main : MonoBehaviour
{
    private static List<Thread_communication> _lst_obj_threads_com;
    private static List<Thread> _lst_threads_com;

    void Start()
    {

        _lst_threads_com = new List<Thread>();

        _lst_obj_threads_com = new List<Thread_communication>();

        int fonctionReseauRecept = 0;

        // RESEAU - boucle de réception des communications
        while(fonctionReseauRecept != 0){

            int typeMsg = 0; // Dépendra du type : identification, connexion, etc

            if(typeMsg == 1){
                // Réception d'une identification (connexion login)

                // Création d'un thread temporaire de gestion de la requête 

                // Créer un objet pour chaque threads, comme ça pas de pb d'un attribut utilisé par plusieurs threads
                Thread nouv_thread_connexion = new Thread(new ThreadStart(thread_connexion));
                nouv_thread_connexion.Start();

                // A FAIRE - Fonction de redirection vers thread de connexion

                //(BDD - Vérifie si le login et mdp sont bons)

            }
            else if(typeMsg == 2){
                // Réception d'une connexion à une partie
            }
            else if(typeMsg == 3){ 

                // Réception d'une création de partie (un if dans le while de reception global)

                // TEMP - DEBUG
                int port_partie = 1;
                    
                if(_lst_threads_com.Count == 0 && _lst_obj_threads_com.Count == 0){ // Aucun thread de comm n'existe
                    
                    Thread_communication thread_com = new Thread_communication(port_partie);
                    Thread nouv_thread = new Thread(new ThreadStart(thread_com.lancement_thread_com));

                    _lst_obj_threads_com.Add(thread_com);
                    _lst_threads_com.Add(nouv_thread);

                    nouv_thread.Start();

                    // A FAIRE - Fonction de redirection vers thread de com
                }
                else{
                    bool thread_com_trouve = false;

                    // Parcours des différents threads de communication pour trouver un qui gère < 5 parties
                    foreach(Thread_communication thread_com in _lst_obj_threads_com){
                        lock(thread_com){
                            if(thread_com.get_nb_parties_gerees() < 5){

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

                        _lst_obj_threads_com.Add(thread_com);
                        _lst_threads_com.Add(nouv_thread);

                        nouv_thread.Start();

                        // A FAIRE - Fonction (dans le thread de com) de création d'accueil

                        // A FAIRE - Fonction de redirection vers thread de com

                    }

                }
        
                // Passage de la requête vers le thread de communication lié
            }
                
            
        }
        // RESEAU - fin boucle de réception des communications


        
        Debug.Log("Ceci est un test");


        // Prévenir tous les threads que le serveur ferme 


        // Fermeture de tous les threads
        foreach(Thread thread in _lst_threads_com){
            thread.Join();
        }

    }

    public static void thread_connexion(){

    }
}