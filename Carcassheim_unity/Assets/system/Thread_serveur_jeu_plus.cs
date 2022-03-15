

using System;
using System.Collections.Generic;
using UnityEngine;


public partial class Thread_serveur_jeu :  MonoBehaviour
{

    public static int tuile_a_tirer(int id, int x, Dictionary<int,int>dico)
    {
        int sum = 0;
        foreach (var item in dico)          //Parcourir dico:
        {
            if (sum+item.Value < x) {
                sum += item.Value;          //chercher l'id correspondant
            }
               
            else {
                //Debug.Log("sum = " + sum);
                //Debug.Log("id = " + id + "   proba= "+item.Value + "  x = "+x);
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
        System.Random MyRand = new System.Random();
        int x = 0, idTuile = 0, booleen = 1, sumDesProbas = 0, sumDesProbas_tmp=0;

        //Recuperer les id des tuiles et leurs probas depuis la bdd

        Dictionary<int,int> map = new Dictionary<int,int>();


        /*
         * 
         * 
         * 
         * Section du test
         * 
        
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

  
         *Fin de section 
         * 
         */


        //Parcourir le dictionnaire resultat pour calculer la somme des probabilités des tuiles:
        foreach ( var item in map)
        {
            sumDesProbas += item.Value;

        }

        //Tirage aléatoire des tuiles
        for (int i = 0;i<nbTuiles;i++)
        {
            x = MyRand.Next(sumDesProbas);
            idTuile= tuile_a_tirer(idTuile, x, map);
            list.Add(idTuile);

        }
   
       
        //Retourner la liste 
        return list;
    }

   /*
    * 
    * void Start()
    {
        int n = 70;
        List<int> list = null;
        list = new List<int>();

        list = Random_sort_tuiles(n);
        foreach (int k in list)

        {

            Debug.Log(k);



        }
    }
   */

}


