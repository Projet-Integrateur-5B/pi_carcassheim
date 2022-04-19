

using System;
using System.Collections.Generic;
using UnityEngine;


public partial class Thread_serveur_jeu : MonoBehaviour
{

    public static ulong tuile_a_tirer(ulong id, int x, Dictionary<ulong, ulong> dico)
    {
        ulong sum = 0;
        foreach (var item in dico)          //Parcourir dico:
        {
            if ((int)(sum + item.Value) < x)
            {
                sum += item.Value;          //chercher l'id correspondant
                                            //Tant que sum+la proba de tuile actuele > id de tuile --> avance
            }

            else
            {
                id = item.Key;              //id retrouvé
                break;
            }

        }

        return id;
    }

    public static List<ulong> Random_sort_tuiles(int nbTuiles)
    {
        List<ulong> list = null;
        list = new List<ulong>();
        System.Random MyRand = new System.Random();
        int x = 0;
        ulong idTuile = 0, sumDesProbas = 0;

        //Recuperer les id des tuiles et leurs probas depuis la bdd

        //Dictionary<int, int> map = new Dictionary<int, int>();
        //La section suivante est à remplacer par une methode de l'équipe BDD qui retourne 
        //un dico des ids de tuile avec leurs probas
        /*************************/
        Dictionary<ulong, ulong> map = new Dictionary<ulong, ulong>()
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
        /*************************/
        // a remplacer par : 
        //RemplirTuiles(map);
        //Parcourir le dictionnaire resultat pour calculer la somme des probabilités des tuiles:
        foreach (var item in map)
        {
            sumDesProbas += item.Value;

        }
        int tmp = (int)(sumDesProbas - sumDesProbas %1.0);
        //Tirage aléatoire des tuiles
        for (int i = 0; i < nbTuiles; i++)
        {
            x = MyRand.Next(tmp);
            idTuile = tuile_a_tirer(idTuile, x, map);
            list.Add(idTuile);

        }
        //Retourner la liste 
        return list;

    }
}


