using System;
using System.Threading;
using System.Collections.Generic;

// L'accueil dans lequel les joueurs se rejoignent et paramètrent la partie
public class  Accueil_partie
{

    // Attributs

    private int _id_partie;

    private List<int> _lst_joueurs; // Contient les ID's de chaque joueur
    private int _id_moderateur; // Identifiant du joueur modérateur

    private string _statut_partie;
    private int _privee; 
    private int _timer;
    private int _timer_max_joueur;
    private int _meeples; // Nombre de meeples par joueur

    // Constructeurs 

    public Accueil_partie(int id_joueur_createur)
    {

        _lst_joueurs = new List<int>();

        // BDD - Parcours de la liste des parties actuelles pour récupérer un ID non utilisé
        //_id_partie = ???

        _lst_joueurs.Add(id_joueur_createur);
        _id_moderateur = id_joueur_createur;

        _statut_partie = "ACCUEIL";

        // Initialisation des valeurs par défaut
        _privee = 1; // Une partie est par défaut privée
        _timer = 3600; // Une heure par défaut
        _timer_max_joueur = 40;
        _meeples = 8;


    }

    // Getters et setters

    // Méthodes


} 