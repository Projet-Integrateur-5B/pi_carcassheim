using System;
using System.Threading;
using System.Collections.Generic;

public class Thread_communication
{
    // Attributs

    private int _numero_port;
    private int _nb_parties_gerees;
    private List<int> _id_parties_gerees;
    

    // Constructeur

    public Thread_communication(int num_port){
        _numero_port = num_port;
        _nb_parties_gerees = 0;
        _id_parties_gerees = new List<int>();
    }
    
    // Getters et setters

    public int get_nb_parties_gerees(){
        return _nb_parties_gerees;
    }

    public List<int> get_id_parties_gerees(){
        return _id_parties_gerees;
    }

    // Augmente le nombre de parties gérées de 1
    public void add_partie_geree(){
        _nb_parties_gerees++;
    }

    // Méthodes

    public void lancement_thread_com(){
        
    }
}