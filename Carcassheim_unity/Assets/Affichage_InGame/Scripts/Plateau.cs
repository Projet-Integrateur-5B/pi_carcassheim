using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plateau : MonoBehaviour
{
    public event Action OnBoardExpanded;
    public float BoardRadius { get => _board_radius; private set { OnBoardExpanded?.Invoke(); _board_radius = value; } }
    private float _board_radius;

    // Quand on rajoute une tuile, on met à jour la taille du plateau dans BoardRadius => la valeur du centre de la tuile qui est la plus loin en x ou en y et on expand

    // Faire un dictionnaire pour stocker les données

    public Tuile getTileAt(Position pos)
    {
        // Récupère la tuile présente à une certaine position et la renvoie

        //renvoie null si c'est pas dans le plateau
        return null;
    }

    public void setTilePossibilities(PlayerRepre player, Tuile tile)
    {
        // Dans les tuiles il y a déjà les positions

        // Soit elles sont déjà affichées => display
        // Sinon on fait rien (display sera appelé ailleurs)

        // indicateur : carré + collider dedans
    }

    public void displayTilePossibilities()
    {
        // Affichage sur le plateau des possibilités de placement de tuiles

        // Les positions du plateau possibles sont mises en avant avec la couleur du joueur élu
        // La subrilliance sur la dernière position jouée par le joueur élu est enlevée

        // indicateur = coup actuel

    }

    public void hideTilePossibilities()
    {

    }

    // repère du plateau : faire un repère avec un O -> le centre du repère et donc du plateau

    // u et v les axes x et y

    public void boardCollide(Ray ray)
    {
        // Voir si le joueur a cliqué à un endroit


        // On fait un collider sur les tuiles
            // Voir table -> tablecheck et table colliderHit

        
    }

    public void displayMeeplePossiblities()
    {
        // Affichage sur le plateau des possibilités de placement de meeples

        //

    }

    public void hideMeeplePossibilities()
    {

    }

    public void finalizeTurn()
    {
        // Fin de tour
    }
}