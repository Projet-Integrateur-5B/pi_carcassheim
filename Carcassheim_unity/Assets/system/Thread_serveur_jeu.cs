using System;
using System.Collections.Generic;
using System.Timers;


public partial class Thread_serveur_jeu
{

	// Attributs

	// Champs qui paramètrent la partie

	private readonly int _id_partie;
	private int _nbJoueur;


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
		_nbJoueur = 1;

	}
	public void ajoutJoueur(int idJoueur)
    {
		Joueur j = new Joueur(idJoueur);
		_nbJoueur++;
		_dico_joueur_score.Add(idJoueur , 0);

	}

	// Méthodes
	/*
	private static System.Timers.Timer aTimer;
	private static void SetTimer()
	{
		// Create a timer with a two second interval.
		aTimer = new System.Timers.Timer(2000);
		// Hook up the Elapsed event for the timer. 
		aTimer.Elapsed += OnTimedEvent;
		aTimer.AutoReset = true;
		aTimer.Enabled = true;
	}
	*/
	public void Lancement_thread_serveur_jeu()//int?
    {
		_statut_partie = "EN_COURS";

		bool tour = true;
		bool partie = true;
		bool choisirTuile = true;
		bool timerNonExpire = true;
		int idTuile = 0; 
		//Lecture du fichier xml / Récupération des infos des tuiles et des terrains / Création des objets
		String file = "Assets/system/infos.xml";
		LireXml l = new LireXml(file);

		//Initialisation du plateau + poser 1ere tuile
		Plateau p = new Plateau();
		p.Poser1ereTuile(0);//a changer, la tuile va être aléatoirement décidée (contenant riviere!)
		
		//Génération des tuiles:
		List<int> listeTuiles = null;
		listeTuiles = new List<int>();
		listeTuiles = Random_sort_tuiles(_nb_tuiles);

		List<int> troisTuiles = null;
		troisTuiles = new List<int>();

		//Envoie infos au client:
		//..Fct res

		while (partie) {
			
			choisirTuile = true;
			//<-- Res tour du joueur x:
			//..
			while (tour) {
                //Phase tirage de 3 tuiles:
                while (choisirTuile)
                {
					//Tirage de 3 tuiles de la fin du tableau
					troisTuiles = tirageTroisTuiles(listeTuiles);
					//ecoute client FCT RES
						//if client repond avec confirmation que l'une des tuile est posable
							//listeTuiles = suppTuileChoisie(idTuileChoisie);
							//bool = false
						//else continue
				}
				timerNonExpire = true;
				//Timer:
				//..

				//Phase actions
				//While(actions)
				//Attendre une action:
				//while(timerNonExpirer)
				//Action recue:
				//si tentative de poser une tuile :
				//verification positionsInternes
				//si tentative de poser un pion Et/Ou abbé
				//verfication position pion en slot
				//verification position abbé
				//si demande de validation:
				//verificationFinal:
				//si triche:
				//j._tricheJoueur++;
				//if(j._tricheJoueur==2)
				//disconnectJoueur(j);
				//else 
				//avertissement(j)
				//si fermeture: Palteau.ZoneFermee(idTuile,idSlot)
				//comptageDesPoints(idTuile)
				//verification pion/abbé & tuile
				//Si timer expiré 
				//j._noteJoueu--;
				//if (j._noteJoueu==0)
				//disconnectJoueur(j._id);
				//passage du tour:

			}


		}
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


	public static List<int> tirageTroisTuiles(List<int> tuiles)
	{
		List<int> list = new List<int>();
		for (int i = tuiles.Count-1;i>= tuiles.Count-4;i++)
			list.Add(tuiles[i]);
		return list;
	}
	public static List<int> suppTuileChoisie(List<int> tuiles, int idTuile)
	{
		for (int i = tuiles.Count - 1; i>0 && tuiles[i] != idTuile; i++)
			tuiles.Remove(tuiles[i]);

		return tuiles;
	}

	public void avertissement(int id_joueur) 
	{
	
	}

	public void disconnectJoueur(int id_joueur)
	{

	}


}
