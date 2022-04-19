using System;
using System.Collections.Generic;
using System.Timers;


public partial class Thread_serveur_jeu
{

	// Attributs

	// Champs qui paramètrent la partie

	private readonly int _id_partie;
	private int _nbJoueur;
	private List <Joueur> _joueurs;

	private Dictionary<int, int> _dico_joueur_score; // Contient les ID's de chaque joueur
	private int _id_moderateur; // Identifiant du joueur modérateur

	private string _statut_partie;

	private int _mode; // 0 -> Classique | 1 -> Time-attack | 2 -> Score

	private int _nb_tuiles;
	private int _score_max;
	private bool _partieEnCours;
	private bool _privee;
	private int _timer; // En secondes
	private int _timer_max_joueur; // En secondes
	private int _meeples; // Nombre de meeples par joueur


	// Champs nécessaires pour le bon fonctionnement du programme



	// Constructeur
	public Thread_serveur_jeu(int id_partie, int id_joueur_createur)
	{
		_id_partie = id_partie;
		_joueurs= new List<Joueur> ();
		_dico_joueur_score = new Dictionary<int, int>();
		Joueur J1 = new Joueur(id_joueur_createur); //joueur_createur
		_joueurs.Add(J1);
		_dico_joueur_score.Add(id_joueur_createur,0);
		_id_moderateur = id_joueur_createur;
		_statut_partie = "ACCUEIL";
		_partieEnCours = false;
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
	public void changeModeToclassic(int nbTuiles)
    {
		_mode = 0;
		_nb_tuiles = nbTuiles;
    }
	public void changeModeToTimeAttack(int timer)
	{
		_mode = 1;
		_timer= timer;
	}
	public void changeModeToScore(int score)
	{
		_mode = 2;
		_score_max=score;
	}
	
	
	public void ajoutJoueur(int idJoueur)
    {
		Joueur j = new Joueur(idJoueur);
		_joueurs.Add(j);
		_nbJoueur++;
		_dico_joueur_score.Add(idJoueur , 0);
	}

	public void suppJoueur(int idJoueur)
	{
		_joueurs.Remove(_joueurs[idJoueur]);
		_dico_joueur_score.Remove(idJoueur);
		_nbJoueur--;
	}

	
	public void Lancement_thread_serveur_jeu()//int?
    {
		_statut_partie = "EN_COURS";
		_partieEnCours = true;

		//Lecture du fichier xml / Récupération des infos des tuiles et des terrains / Création des objets
		String file = "Assets/system/infos.xml";
		//LireXml l = new LireXml(file); //A REMETTRE

		//Initialisation du plateau + poser tuiles de riviere
		Plateau p = new Plateau();
		//p.Poser1ereTuile(0);//a changer, la rivière va être aléatoirement géneré 
		
		//Génération des tuiles:
		List<ulong> listeTuiles = null;
		listeTuiles = new List<ulong>();
		listeTuiles = Random_sort_tuiles(_nb_tuiles);

		

        //Envoie infos au client:
        //..Fct res

        switch (_mode)
        {
			case 0: 
				moteurModeClassique(p,listeTuiles);
				break;
			case 1:
				moteurModeTimeAttack(p,listeTuiles);
				break;
			case 2:
				moteurModeScore(p,listeTuiles);
				break;
        }

		// A FAIRE - Fin/retour de la fonction pour libérer l'objet associé dans le thread_com à la fin de la partie et donc de ce thread
		//return 0;	//cas ou' tout s'est bien passé et la partie est finie
    }

	public void moteurModeClassique(Plateau p, List<ulong> listeTuiles)
	{
		List<ulong> troisTuiles = null;
		troisTuiles = new List<ulong>();
		bool tour = true;
		bool choisirTuile = true;
		bool timerNonExpire = true;
		int idTuile = 0;
		Timer aTimer;
		aTimer = new System.Timers.Timer();
		initTimerTour(aTimer);


		while (_partieEnCours)
		{

			choisirTuile = true;
			//<-- Res tour du joueur x:
			//..

			//Passage du tour au joueur suivant 
			//...
			tour = true;
			while (tour)
			{
				
				//Phase1 : tirage de 3 tuiles:
				while (choisirTuile)
				{
					//Tirage de 3 tuiles de la fin du tableau
					troisTuiles = tirageTroisTuiles(listeTuiles);
					//Res(troisTuiles)<-
					//ecoute client FCT RES->
					//if client repond avec confirmation que l'une des tuile est posable
						//listeTuiles = suppTuileChoisie(idTuileChoisie);
						choisirTuile = false;
					//else continue
				}
				timerNonExpire = true;


				//Phase 2 : actions
				//While(timerNonExpire)///!!
				//timer:

				//informer client du tour:
				//Res<--
				aTimer.Enabled = true;

				//Attendre une action:
				//while(timerNonExpire)
				//Action recue: -->Res
				//si tentative de poser une tuile :
				//verification positionsInternes
				//si tentative de poser un pion Et/Ou abbé
				//verfication position pion en slot
				//verification position abbé
				//si demande de validation:
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
				//j._noteJoueur--;
				//if (j._noteJoueu==0)
				//disconnectJoueur(j._id);

				//passage du tour:
				tour = false;
			}
		}
	}
	public void moteurModeScore(Plateau p, List<ulong> listeTuiles)
	{
		bool tour = true;
		bool partie = true;
		bool choisirTuile = true;
		bool timerNonExpire = true;
		int idTuile = 0;
		List<ulong> troisTuiles = null;
		troisTuiles = new List<ulong>();
		Timer aTimer;
		aTimer = new System.Timers.Timer();
		initTimerTour(aTimer);

	}
	public void moteurModeTimeAttack(Plateau p, List<ulong> listeTuiles)
	{
		bool tour = true;
		bool partie = true;
		bool choisirTuile = true;
		bool timerNonExpire = true;
		int idTuile = 0;
		List<ulong> troisTuiles = null;
		troisTuiles = new List<ulong>();
		Timer aTimer;
		aTimer = new System.Timers.Timer();
		initTimerTour(aTimer);

	}

	public void initTimerTour( Timer aTimer)
		{
			aTimer.Interval = _timer_max_joueur;
			// désactiver l'auto renouvelement  
			aTimer.AutoReset = false;
		}


	public void initTimerPartie(Timer aGameTimer)
	{
		aGameTimer.Interval = _timer;
		// désactiver l'auto renouvelement  
		aGameTimer.AutoReset = false;
        // Hook up the Elapsed event for the timer. 
        aGameTimer.Elapsed += finDeTimer;
	}

	public void finDeTimer(Object source, System.Timers.ElapsedEventArgs e)
    {
		endGame();
	}
	public void endGame()
	{
		_partieEnCours = false;
	}
	
	public static List<ulong> tirageTroisTuiles(List<ulong> tuiles)
	{
		List<ulong> list = new List<ulong>();
		for (int i = tuiles.Count - 3 ;i< tuiles.Count; i++)
			list.Add(tuiles[i]);
		return list;
	}
	public static List<ulong> suppTuileChoisie(List<ulong> tuiles, ulong idTuile)
	{
		int i = 0;
		for (i = tuiles.Count - 1; i >= 0 && tuiles[i] != idTuile; i--) ;
		tuiles.Remove(tuiles[i]);

		return tuiles;
	}

	public void avertissement(int id_joueur) 
	{
		//FCT Res <-Client
	}

	public void disconnectJoueur(int id_joueur)
	{
		_dico_joueur_score.Remove(id_joueur);
		_nbJoueur--;
		// FCT Res <-Client
	}


}
