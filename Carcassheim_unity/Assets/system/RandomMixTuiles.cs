using System.Collections.Generic;
using System.Linq;



/* 
 * Cette méthode est à utiliser côté serveur
 * Dans le cas ou', au moment de l'envoie de trois tuiles 
 * a echoué (aucune tiule n'est posable
 * Dans ce cas : il faut appeler cette méthode afin de mélanger la listes des tuiles aavant de 
 * appeler à nouveau la méthode qui tire trois tuiles
 * elle prend en parametres la listeDesTuiles et la renvoie mélangée
 * */
public class RandomMixTuiles 
{
    public List<ulong> mixList(List<ulong> tuilesGame)
    {
        List<ulong> tuilesGame_resultat = new List<ulong>();
        var rnd = new System.Random();
        var randomedList = tuilesGame.OrderBy(item => rnd.Next());
        foreach (var value in randomedList)
        {
            tuilesGame_resultat.Add(value);
        }
        return tuilesGame_resultat;
    }

    
}