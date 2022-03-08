using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class Thread_communication
{
    // Attributs

    private int _id_thread_com;
    private int _numero_port;
    private int _nb_parties_gerees;
    private List<int> _id_parties_gerees;
    

    // Constructeur

    public Thread_communication(int num_port, int id){
        _numero_port = num_port;
        _nb_parties_gerees = 0;
        _id_parties_gerees = new List<int>();
        _id_thread_com = id;
    }
    
    // Getters et setters

    public int Get_nb_parties_gerees(){
        return _nb_parties_gerees;
    }

    public List<int> Get_id_parties_gerees(){
        return _id_parties_gerees;
    }

    // Augmente le nombre de parties gérées de 1
    public void Add_partie_geree(int id_partie_ajoutee){
        _id_parties_gerees.Add(id_partie_ajoutee);
        _nb_parties_gerees++;
    }

    // Méthodes

    public void Lancement_thread_com(){

        Thread.Sleep(2000);

        Debug.Log(string.Format("[{0}] Je suis un thread !", _id_thread_com));
        Debug.Log(string.Format("[{0}] J'officie sur le port numéro {0} !", _id_thread_com, _numero_port));
        Debug.Log(string.Format("[{0}] Je gère actuellement {1} parties!", _id_thread_com, _nb_parties_gerees));
        foreach(int id_ite in _id_parties_gerees){
            Debug.Log(string.Format("[{0}] Je gère la partie d'ID {1}", _id_thread_com, id_ite));
        }


        int debug = 1;

        if(debug != 1) // TEMPORAIRE - A retirer plus tard
        {

        }

 
    }
}