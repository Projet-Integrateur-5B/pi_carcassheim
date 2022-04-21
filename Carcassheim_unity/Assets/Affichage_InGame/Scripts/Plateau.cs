using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plateau : MonoBehaviour
{
    private class PositionComparer : IEqualityComparer<Position>
    {
        public bool Equals(Position pos0, Position pos1)
        {
            return pos0.X == pos1.X && pos0.Y == pos1.Y;
        }

        public int GetHashCode(Position pos)
        {
            int x = pos.X, y = pos.Y;
            return ((x + y) * (x + y + 1) / 2) + y;
        }
    }
    [SerializeField] TileIndicator tile_indic_model; // ton prefab
    public event Action OnBoardExpanded;
    public float BoardRadius { get => _board_radius; private set { OnBoardExpanded?.Invoke(); _board_radius = value; } }
    private float _board_radius;

    public bool TilePossibilitiesShown { get => _tilePossibilitiesShown; set { _tilePossibilitiesShown = value; } }
    private bool _tilePossibilitiesShown;

    // Dictionnaire pour stocker les données des tuiles posées sur le plateau
    private Dictionary<Position, Tuile> tiles_on_board;
    
    [SerializeField] private List<TileIndicator> act_tile_indicator;

    public Vector3 BoardCollidePos { get => _board_collide_pos; set { _board_collide_pos = value; } }
    private Vector3 _board_collide_pos;

    [SerializeField] private Transform rep_O, rep_u, rep_v;

    // TODO : Quand on rajoute une tuile, on met à jour la taille du plateau dans BoardRadius => la valeur du centre de la tuile qui est la plus loin en x ou en y et on expand

    void Awake()
    {
        tiles_on_board = new Dictionary<Position, Tuile>(new PositionComparer());
    }

    
    // Récupère la tuile présente à une certaine position et la renvoie
    public Tuile getTileAt(Position pos)
    {
        // Parcours du dictionnaire pour récupérer la tuile dont les coordonnées correspondent
        Tuile res = null;
        bool tile_found = pos != null && tiles_on_board.TryGetValue(pos, out res);

        //renvoie null si la position donnée n'est pas dans le plateau actuel
        return tile_found ? res : null;
    }

    public bool setTileAt(Position pos, Tuile tile)
    {
        Tuile act_tile = null;
        bool tile_found;
        if (pos == null)
        {
            tile_found = tile.Pos != null && tiles_on_board.TryGetValue(tile.Pos, out act_tile);
            if (tile_found && act_tile == tile)
            {
                tile.Pos = null;
                return true;
            }
        }
        else
        {
            tile_found = tiles_on_board.TryGetValue(pos, out act_tile);
            if (!tile_found)
            {
                tile_found = tile.Pos != null && tiles_on_board.TryGetValue(tile.Pos, out act_tile);
                if (tile_found)
                {
                    if (act_tile == tile)
                    {
                        if (tile.Pos.X != pos.X || tile.Pos.Y != pos.Y)
                        {
                            tiles_on_board.Remove(tile.Pos);
                            tiles_on_board.Add(pos, tile);
                        }
                        tile.Pos = pos;
                        return true;
                    }
                }
                else
                {
                    tiles_on_board.Add(pos, tile);
                    tile.Pos = pos;
                    return true;
                }
            }
        }
        return false;
    }

    private void addTile(PlayerRepre player, Position pos)
    {
        TileIndicator new_tileInd = Instantiate<TileIndicator>(tile_indic_model);
        new_tileInd.setAttributes(player, pos);
        new_tileInd.transform.position = rep_O.position + (rep_u.position - rep_O.position) * pos.X + (rep_v.position - rep_O.position) * pos.Y;
        new_tileInd.display();
        act_tile_indicator.Add(new_tileInd);
    }

    void cleanTilePossibilities(PlayerRepre player)
    {
        for (int i = 0; i < act_tile_indicator.Count; i++)
        {
            if (act_tile_indicator[i].state != TileIndicatorState.LastTile || act_tile_indicator[i].player ==  player)
            {
                Destroy(act_tile_indicator[i].gameObject);
                if (act_tile_indicator.Count > 1)
                {
                    act_tile_indicator[i] = act_tile_indicator[act_tile_indicator.Count - 1];
                    i--;
                }
                act_tile_indicator.RemoveAt(act_tile_indicator.Count - 1);
            }
        }
    }

    public void setTilePossibilities(PlayerRepre player, Tuile tile)
    {
        // Si les possibilités de l'ancienne tuile sont affichées, on les hide => pas de paramètre à ces fonctions, elles utilisent la liste qui est globale à la classe
        if(_tilePossibilitiesShown)
            hideTilePossibilities();
        
        cleanTilePossibilities(player);
        
        //Remplissage de la liste de tile indicateurs avec la liste des positions donnée par la tuile
        foreach (Position tilePos in tile.possibilitiesPosition)
        {
            addTile(player, tilePos);
        }
    }

    public void displayTilePossibilities()
    {
        // Les positions du plateau possibles sont mises en avant avec la couleur du joueur élu
        // La subrilliance sur la dernière position jouée par le joueur élu est enlevée

        _tilePossibilitiesShown = true;

        // Affichage sur le plateau des possibilités de placement de tuiles
        foreach (TileIndicator tile in act_tile_indicator)
        {
            if (tile.state == TileIndicatorState.TilePossibilitie)
                tile.display();
        }


    }

    public void hideTilePossibilities()
    {
        _tilePossibilitiesShown = false;

        foreach (TileIndicator tile in act_tile_indicator)
        {
            if (tile.state == TileIndicatorState.TilePossibilitie)
                tile.hide();
        }
    }

    public bool boardCollide(Ray ray)
    {
        RaycastHit hit;

        // Voir si le joueur a cliqué sur une tuile du plateau
        if(_tilePossibilitiesShown)
        {
            Physics.Raycast(ray, out hit);
            _board_collide_pos = hit.collider.gameObject.GetComponent<Collider>().transform.position;
            return true;
        }
        else
        {
            Debug.Log("Shouldn't have been an input");
        }

        return true;  
    }

    public void finalizeTurn(Position pos, Tuile tile)
    {
        // Fin de tour
        // Ajouter la position finale de la tuile au dictionnaire contenant les tuiles présentes sur le tableau
        setTileAt(pos, tile);
    }
}