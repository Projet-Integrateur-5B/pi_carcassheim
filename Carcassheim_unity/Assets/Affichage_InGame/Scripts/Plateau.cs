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

    public bool TilePossibilitiesShown { get => _tilePossibilitiesShown; set { _tilePossibilitiesShown = value; } }
    private bool _tilePossibilitiesShown;

    public bool MeeplePossibilitiesShown { get => _meeplePossibilitiesShown; set { _meeplePossibilitiesShown = value; } }
    private bool _meeplePossibilitiesShown;

    private Dictionary<int, TileOnBoard> tiles_on_board;
    //private Collider tile_collider_model;
    
    private List<TileIndicator> act_tile_indicator;

    public Vector3 BoardCollidePos { get => _board_collide_pos; set { _board_collide_pos = value; } }
    private Vector3 _board_collide_pos;

    [SerializeField] private Transform rep_O, rep_u, rep_v;


    //public TileOnBoard meeple_type { get; set; } = MeepleType.DefaultMeeple;

    //private Dictionary<string, TilesOnBoard> tiles_on_board;

    // = new Dictionary<string, Tuile, int, int>();

    // Quand on rajoute une tuile, on met à jour la taille du plateau dans BoardRadius => la valeur du centre de la tuile qui est la plus loin en x ou en y et on expand

    // Faire un dictionnaire pour stocker les données des tuiles
    // Chaque rang du dictionnaire aura une clé liée à une tuile, une position x et une position y

    
    // Récupère la tuile présente à une certaine position et la renvoie
    public Tuile getTileAt(Position pos)
    {
        // Parcours du dictionnaire pour récupérer la tuile dont les coordonnées correspondent
        foreach(var value in tiles_on_board.Values)
        {
            if(value.Pos == pos)
                return value.Tile;
        }
 
        //renvoie null si la position donnée n'est pas dans le plateau actuel
        return null;
    }

    public void setTilePossibilities(PlayerRepre player, Tuile tile)
    {
        RaycastHit hit;
        
        // Si les possibilités de l'ancienne tuile sont affichées, on les hide => pas de paramètre à ces fonctions, elles utilisent la liste qui est globale à la classe
        if(_tilePossibilitiesShown)
            hideTilePossibilities();
        
        //if (Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << BoardLayer)))
        //    consumed = board.boardCollide(hit.transform);

        //Remplir la liste de tile indicateurs
        // Dans les tuiles il y a déjà les positions
        //act_tile_indicator = tile.possibilitiesPosition;

        // Les tile indicateurs contiennent : un carré de la taille d'une tuile et de la couleur du joueur, un collider


        // Soit les possibilités d'une autre tuile sont déjà affichées => dans ce cas là on appelle display directement pour remplacer ces anciennes tuiles
        if(_tilePossibilitiesShown)
            displayTilePossibilities();

        // Rajouter cette tuile ainsi

        // Sinon on fait rien (display sera appelé ailleurs)

    }

    public void displayTilePossibilities()
    {
        _tilePossibilitiesShown = true;
        // Affichage sur le plateau des possibilités de placement de tuiles

/*         ColliderStat collider = Instantiate<ColliderStat>(tile_collider_model, tile_zone.transform);
        collider.transform.position = tile.transform.position;
        collider.Index = act_tile_count;
        tile_mapping.Add(tile, collider); */

        //tl.Id, tl.possibilitiesPosition;

        // Les positions du plateau possibles sont mises en avant avec la couleur du joueur élu
        // La subrilliance sur la dernière position jouée par le joueur élu est enlevée

        // indicateur = coup actuel

    }

    public void hideTilePossibilities()
    {
        _tilePossibilitiesShown = false;

    }

    // repère du plateau : faire un repère avec un O -> le centre du repère et donc du plateau

    // u et v les axes x et y

    public bool boardCollide(Ray ray)
    {
        RaycastHit hit;
        // Voir si le joueur a cliqué sur une tuile du plateau
        if(_tilePossibilitiesShown)
        {
            Physics.Raycast(ray, out hit);
            _board_collide_pos = hit.collider.gameObject.GetComponent<Collider>().transform.position;
            //board_collide_pos = display_system.setSelectedTile(hit.GetComponent<ColliderStat>().Index);
            return true;
        }
        else if(_meeplePossibilitiesShown)
        {
            /* if (hit.parent != meeple_zone.transform)
            {
                Debug.Log("Parent of hit is " + hit.parent.name + " instead of " + meeple_zone.name);
                break;
            }
            display_system.setSelectedMeeple(hit.GetComponent<MeepleColliderStat>().Index); */
            return true;
        }
        else
        {
            Debug.Log("Shouldn't have been an input in " + act_table_state.ToString());
        }

        return true;  
    }

    public void displayMeeplePossiblities()
    {
        // Affichage sur le plateau des possibilités de placement de meeples
    }

    public void hideMeeplePossibilities()
    {

    }

    public void finalizeTurn(Position pos, Tuile tile)
    {
        // Fin de tour
        // Ajouter la position finale de la tuile au dictionnaire contenant les tuiles présentes sur le tableau
        TileOnBoard tb = new TileOnBoard(tiles_on_board.Count, pos, tile);
        tiles_on_board.Add(tb.Id, tb);
    }
}