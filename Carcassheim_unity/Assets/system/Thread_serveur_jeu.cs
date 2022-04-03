using System;
using System.Collections.Generic;


public partial class Thread_serveur_jeu
{

	// Attributs

	// Champs qui paramètrent la partie

	private readonly int _id_partie;

	private Dictionary<int, int> _dico_joueur_score; // Contient les ID's de chaque joueur
	private int _id_moderateur; // Identifiant du joueur modérateur

	private string _statut_partie;

	private int _mode; // 0 -> Classique | 1 -> Time-attack | 2 -> Score

	private int _nb_tuiles;
	private int _score_max;

	private bool _privee;
	private int _timer; // En secondes
	private int _timer_max_joueur; // En secondes
	private int _meeples; // Nombre de meeples par joueur


	// Champs nécessaires pour le bon fonctionnement du programme

	// A RAJOUTER : tableau d'étape du tour actuel -> placement tuile, placement pion, pour savoir si ces deux derniers ont 
	// été validés (et sont donc légaux).
	// -> On enregistre le placement de la tuile et du pion, une fois vérifié, quelque part. On ne met à jour la structure de donnée
	// qu'à la validation du tour.



	// Constructeur
	public Thread_serveur_jeu(int id_partie, int id_joueur_createur)
	{
		_id_partie = id_partie;

		_dico_joueur_score = new Dictionary<int, int>();

		_dico_joueur_score.Add(id_joueur_createur,0);
		_id_moderateur = id_joueur_createur;

		_statut_partie = "ACCUEIL";

		// Initialisation des valeurs par défaut
		_mode = 0;
		_nb_tuiles = 60;
		_score_max = -1;
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
