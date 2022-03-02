using System;
using System.Threading;

public class Thread_communication
{
    // Attributs

    private int _numero_port;
    private int _parties_gerees;
    

    // Constructeur

    public Thread_communication(int num_port){
        _numero_port = num_port;
        _parties_gerees = 0;
    }
    
    // Getters et setters

    public int get_parties_gerees(){
        return _parties_gerees;
    }

    // Augmente le nombre de parties gérées de 1
    public int add_partie_geree(){
        _parties_gerees++;
    }

    // Méthodes

    public void lancement_thread_com()
    {

        private List<int> _id_parties_gerees = new List<int>();
        
    }
}