using System;

public partial class Thread_serveur_jeu
{

	// Attributs

	private int _id_partie;

	// Constructeur
	public Thread_serveur_jeu(int id_partie)
	{
		_id_partie = id_partie;

		// IDEES :
		// Initialiser avec mêmes attributs que accueil partie ? Seulement un attribut en plus "en accueil" ? 
	}

	// Méthodes

	public void Lancement_thread_serveur_jeu()
    {

		

		// A FAIRE - Fin/retour de la fonction pour libérer l'objet associé dans le thread_com à la fin de la partie et donc de ce thread
    }
}
