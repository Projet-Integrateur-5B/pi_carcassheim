

using System;
using System.Collections;
using System.Collections.Generic;


public partial class Thread_serveur_jeu
{

    public static int tuile_a_tirer(int id, int x, Dictionary<int,int>dico)
    {
        int sum = 0;
        foreach (var item in dico)          //Parcourir dico:
        {
            if (sum < x) {
                sum += item.Value;          //chercher l'id correspondant
            }
               
            else {
                id = item.Key;              //id trouvé
                break;
            }

        }

        return id;
    }

    public static  List<int> Random_sort_tuiles(int nbTuiles)
    {
        List<int> list = null;
        list = new List<int>();
        Random MyRand = new Random();
        int x = 0, idTuile = 0, booleen = 1, sumDesProbas = 0, sumDesProbas_tmp=0;

        //Recuperer les id des tuiles et leurs probas depuis la bdd

        //Dictionary<int,int> map = new Dictionary<int,int>();




        /*
         * 
         * 
         * 
         * Section du test
         * 
         */


        Dictionary<int, int> map = new Dictionary<int, int>()
        {
            { 0, 2},
            { 1, 4},
            { 2, 6},
            { 3, 8},
            { 4, 9},
            { 5, 10},
            { 6, 11},
            { 7, 13},
            { 8, 14}
        };

   

        /*
         * 
         * 
         *Fin de section 
         * 
         */


        //Parcourir le dictionnaire resultat pour calculer la somme des probabilités des tuiles:
        foreach ( var item in map)
        {
            sumDesProbas += item.Value;

        }

        //Test mode du jeu depuis partie en question:

        //Si mode avec un nombre de tuiles limité :
        //n est le nombre de tuiles:
        //recupere la proba de chaque tuile en proba
        //faire pour chaque tuile : ajouter la tuile (proba*n) fois dans la liste 
        for (int i = 0;i<nbTuiles;i++)
        {
            x = MyRand.Next(sumDesProbas);
            idTuile= tuile_a_tirer(idTuile, x, map);
            list.Add(idTuile);

        }
        //Sinon, on tire 60 en attendant
        //60 est le nb de tuile de demarage 
        // recupere la proba de chaque tuile en proba
        //faire pour chaque tuile : ajouter la tuile (proba*60) fois dans la liste 
        
        
        /* Y a plus besoin

        //Melanger la liste 
        int arrayLength = list.Count;
        list.ToArray();

        for (int i = arrayLength - 1; i > 1; --i)
        {
            // tirage au sort d'un index entre 0 et la valeur courante de "i"
            int randomIndex = MyRand.Next(i);
            // intervertion des éléments situés aux index "i" et "randomIndex"
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        */

        //Retourner la liste 
        return list;
    }

    void Start()
    {
        int n = 70;
        List<int> list = null;
        list = new List<int>();

        Random_sort_tuiles(n);
    }


}


