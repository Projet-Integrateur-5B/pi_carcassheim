using System;
using System.Collections.Generic;

public class Thread_connexion
{

	// Attributs

	private string _login;
    private int _id_partie;
    private static List<Thread_communication> _lst_obj_threads_com;

    // Constructeur
    public Thread_connexion(string login, int id_partie_cherchee, List<Thread_communication> lst_obj_threads_com)
	{
		_login = login;
        _id_partie = id_partie_cherchee;
        _lst_obj_threads_com = lst_obj_threads_com;
	}

	// Méthodes
	public void Lancement_thread_connexion()
    {

        bool partie_trouvee = false;

        // Parcours des threads de communication pour trouver celui qui gère la partie cherchée
        foreach (Thread_communication thread_com_iterateur in _lst_obj_threads_com)
        {
            lock (thread_com_iterateur)
            {
                List<int> lst_id_parties_gerees = thread_com_iterateur.Get_id_parties_gerees();
                if (lst_id_parties_gerees.Contains(_id_partie))
                {

                    // Partie trouvée
                    partie_trouvee = true;


                    // Passe la main au thread de communication lié
                    // RESEAU : répond au client en l'informant qu'il doit à présent communiquer avec le thread de com
                    //  (en lui filant le port)
                    //      PUIS : modifier un des attributs du thread com en question pour lui indiquer qu'il gère un nvx joueur ?



                    break; // Sortie du foreach
                   
                }
            }
        }

        if (!partie_trouvee) // Si malgré tout la partie ne semble pas exister
        {

            // RESEAU - Indique au client que la partie cherchée n'existe pas ou plus.

        }
    }
}
