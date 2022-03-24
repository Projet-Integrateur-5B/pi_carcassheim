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
		//bool tour = true;
		//bool partie = true;
        //while (partie) {
			//LireXml?
			//Initialisation du plateau

			//boucle_Tour:
			//while (tour) { 
			//Phase tirage de 3 tuiles
			//while(bool)
			//Tirage de 3 tuiles de la fin du tableau
			//ecoute client
				//if client repond avec confirmation que l'une des tuile est posable
				//bool = false
				
			//else continue

			//idTuile=idTuileChoisie
			//Timer

			//Phase actions
			//While(1)
			//Attendre une action:
				//Action recue:
					//si tentative de poser une tuile :
						//verification positionsInternes
					//si tentative de poser un pion Et/Ou abbé
						//verfication position pion en slot
						//verification position abbé
					//si demande de validation:
						//si fermeture: Palteau.ZoneFermee(idTuile,idSlot)
							//comptageDesPoints(idTuile)
						//verification pion/abbé & tuile
		//passage du tour:

		//}

		
		//}
		// A FAIRE - Fin/retour de la fonction pour libérer l'objet associé dans le thread_com à la fin de la partie et donc de ce thread
		//return 0;	//cas ou' tout s'est bien passé et la partie est finie
    }
    public void comptageDesPoints(int[] idSlots)
    {
	//Règles d'évaluation:
		// route : 1 point par tuile
		// ville : Chaque tuile dans une ville complétée rapporte 2 points.
			// De plus, chaque blason dans une ville complétée rapporte
			// 2 points de plus.
		// abbaye : Une abbaye est complétée lorsqu’elle est complètement entourée de tuiles.
			// Lors de l’évaluation, une abbaye complétée rapporte 1 point par tuile la
			// complétant(incluant celle de l’abbaye).

	}
}
