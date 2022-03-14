using System;
using System.Collections.Generic;

public partial class Thread_serveur_jeu
{

	// Attributs

	// Champs qui paramètrent la partie

	private readonly int _id_partie;

	private List<int> _lst_joueurs; // Contient les ID's de chaque joueur
	private int _id_moderateur; // Identifiant du joueur modérateur

	private string _statut_partie; 

	private bool _privee;
	private int _timer; // En secondes
	private int _timer_max_joueur; // En secondes
	private int _meeples; // Nombre de meeples par joueur

	// Champs nécessaires pour le bon fonctionnement du programme



	// Constructeur
	public Thread_serveur_jeu(int id_partie, int id_joueur_createur)
	{
		_id_partie = id_partie;

		_lst_joueurs = new List<int>();

		_lst_joueurs.Add(id_joueur_createur);
		_id_moderateur = id_joueur_createur;

		_statut_partie = "ACCUEIL";

		// Initialisation des valeurs par défaut
		_privee = true; // Une partie est par défaut privée
		_timer = 3600; // Une heure par défaut
		_timer_max_joueur = 40;
		_meeples = 8;
	}

	// Méthodes

	public void Lancement_thread_serveur_jeu()
    {

		

		// A FAIRE - Fin/retour de la fonction pour libérer l'objet associé dans le thread_com à la fin de la partie et donc de ce thread
    }
}
