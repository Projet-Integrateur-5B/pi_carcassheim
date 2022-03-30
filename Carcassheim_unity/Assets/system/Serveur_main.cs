using System;
using System.Threading;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Serveur_main : MonoBehaviour
{
    private static List<Thread_communication> _lst_obj_threads_com;
    private static List<Thread> _lst_threads_com;
    private static List<int> _lst_port_dispo;

    private static int _compteur_id_thread_com;

    void Start()
    {
        

        _lst_threads_com = new List<Thread>();

        _lst_obj_threads_com = new List<Thread_communication>();

        _lst_port_dispo = new List<int>();
        _lst_port_dispo.Add(1); // DEBUG
        _lst_port_dispo.Add(2); // DEBUG

        _compteur_id_thread_com = 0;

        int fonctionReseauRecept = 0;

        // RESEAU - boucle de réception des communications
        while(fonctionReseauRecept != 0){

            int typeMsg = 0; // Dépendra du type : identification, connexion, etc

            if(typeMsg == 1){ // Réception d'une identification (connexion login)

                string login = "Test_DEBUG";
                string mdp = "Test_DEBUG";

                string client = "Temporaire"; // RESEAU - Prévoir un objet "client"


                // Création d'un thread temporaire de gestion de la requête 
                Thread_identification thread_identification = new Thread_identification(login,mdp,client);

                // Créer un objet pour chaque threads, comme ça pas de pb d'un attribut utilisé par plusieurs threads
                Thread nouv_thread_identification = new Thread(new ThreadStart(thread_identification.Lancement_thread_identification));
                nouv_thread_identification.Start();


            }
            else if(typeMsg == 2){ // Réception d'une connexion à une partie

                string login = "Test_DEBUG_connexion";
                int id_partie_cherchee = 1; // DEBUG

                // Création d'un thread temporaire de gestion de la requête de connexion
                Thread_connexion thread_connexion = new Thread_connexion(login, id_partie_cherchee, _lst_obj_threads_com);

                // Créer un objet pour chaque threads, comme ça pas de pb d'un attribut utilisé par plusieurs threads
                Thread nouv_thread_connexion = new Thread(new ThreadStart(thread_connexion.Lancement_thread_connexion));
                nouv_thread_connexion.Start();


            }
            else if(typeMsg == 3){ // Réception d'une création de partie (un if dans le while de reception global) -> A DEPLACER dans thread com (faire un appel de fonction)

 
                    
                if (_lst_threads_com.Count == 0 && _lst_obj_threads_com.Count == 0) { // Aucun thread de comm n'existe

                    int port_nouv_thread_com = Creation_thread_com();

                    if (port_nouv_thread_com != -1) { // Seulement si un nouveau thread de com a pu être créé

                        // RESEAU - Fonction de redirection du client vers le bon thread de com

                    }

                    
                }
                else{
                    bool thread_com_trouve = false;

                    // Parcours des différents threads de communication pour trouver un qui gère < 5 parties
                    foreach(Thread_communication thread_com_iterateur in _lst_obj_threads_com)
                    {
                        lock (thread_com_iterateur.Get_lock_nb_parties_gerees())
                        {
                            if (thread_com_iterateur.Get_nb_parties_gerees() < 5) {

                                thread_com_trouve = true;


                                // A FAIRE - Fonction (dans le thread de com) de création d'accueil ET DE PARTIE QUOI
                                // Exemple:
                                /*
                                int nouvel_id = 2; // BDD - Rajouter requête pour récupérer le prochain id dispo
                                thread_com_iterateur.add_partie_geree();
                                */


                                // RESEAU - Fonction de redirection du client vers le bon thread de com (pour qu'il lui dise qu'il veut créer une partie)

                                break; // Sort du foreach
                            }
                        }
                    }

                    // Si aucun des threads n'est libre pour héberger une partie de plus
                    if(thread_com_trouve == false) 
                    {

                        int port_nouv_thread_com = Creation_thread_com();

                        if (port_nouv_thread_com != -1) { // Seulement si un nouveau thread de com a pu être créé     

                            // A FAIRE - Fonction (dans le thread de com) de création d'accueil

                            // RESEAU - Fonction de redirection du client vers le bon thread de com (pour qu'il lui dise qu'il veut créer une partie)
                        }


                    }

                }
        
                // Passage de la requête vers le thread de communication lié
            }
                  
        }
        // RESEAU - fin boucle de réception des communications


        // Test threads 
        Debug.Log("Ceci est un test");

        // Création d'un thread de com de port 1
        Creation_thread_com();

        // Création d'un thread de com de port 2
        Creation_thread_com();

        // On indique au premier qu'il gère 1 partie et au second 2
        _lst_obj_threads_com[0].Add_partie_geree(1);

        _lst_obj_threads_com[1].Add_partie_geree(2);
        _lst_obj_threads_com[1].Add_partie_geree(3);

      
        // Prévenir tous les threads que le serveur ferme 

    

        // Fermeture de tous les threads
        foreach(Thread thread in _lst_threads_com){
            thread.Join();
        }


        // Test de communication réseau
        Socket? socket = null;
        var original = new Packet();

        var error_value = Client.Connection(ref socket);
        switch (error_value)
        {
            case Errors.None:
                break;
            case Errors.ConfigFile:
                // TODO : handle case : config file is bad or issue while extracting the data
               Debug.Log("Errors.ConfigFile");
                break;
            case Errors.Socket:
                // TODO : handle case : connection could not be established
               Debug.Log("Errors.Socket");
                break;
            case Errors.ToBeDetermined:
                break;
            default:
                // TODO : handle case : default
               Debug.Log("Errors.Unknown");
                break;
        }

        string[] test = { "pseudo", "mdp18" };
        error_value = socket.Communication(ref original, IdMessage.Default, test);
        switch (error_value)
        {
            case Errors.None:
                break;
            case Errors.Format: // == Errors.Receive ?
                // TODO : handle case : wrong format
                Debug.Log("Errors.Format");
                break;
            case Errors.Socket:
                // TODO : handle case : connection error
                Debug.Log("Errors.Socket");
                break;
            case Errors.Data:
                // TODO : handle case : error while getting the packet ready
                Debug.Log("Errors.Data");
                break;
            case Errors.Receive:
                // TODO : handle case : error while receiving an answer
                Debug.Log("Errors.Receive");
                break;
            default:
                // TODO : handle case : default
                Debug.Log("Errors.Unknown");
                break;
        }

        error_value = Client.Disconnection(socket);
        switch (error_value)
        {
            case Errors.None:
                break;
            case Errors.Socket:
                // TODO : handle case : connection could not be closed
               Debug.Log("Errors.Socket");
                break;
            default:
                // TODO : handle case : default
               Debug.Log("Errors.Unknown");
                break;
        }




    }

    private int Creation_thread_com(){

        if(_lst_port_dispo.Count != 0)
        {
            int port_choisi = _lst_port_dispo[_lst_port_dispo.Count - 1];

            Thread_communication thread_com = new Thread_communication(port_choisi, _compteur_id_thread_com);
            _compteur_id_thread_com++;
            Thread nouv_thread = new Thread(new ThreadStart(thread_com.Lancement_thread_com));

            _lst_obj_threads_com.Add(thread_com);
            _lst_threads_com.Add(nouv_thread);

            _lst_port_dispo.RemoveAt(_lst_port_dispo.Count - 1);

            nouv_thread.Start();

            return port_choisi;
        }
        else
        {
            // RESEAU - Communiquer au client qu'il est impossible de créer une nouvelle partie, maximum atteint




            return -1;
        }

        

    }
}