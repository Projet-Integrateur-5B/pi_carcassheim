using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.system;
using System;
using TMPro;
public class backLocal : CarcasheimBack
{
    private const string XML_PATH = "config_back.xml";
    private Dictionary<ulong, Tuile> dicoTuile;
    
    private Plateau _plateau;
    private List<PlayerInitParam> players = new List<PlayerInitParam>(); 
    private List<int> players_score = new List<int>();

    private int index_player = 0; // joueur en jeu
    private int nb_player = 3; // à remplir via field

    [SerializeField] private TMP_Text error_msg;

    private WinCondition my_wincond = WinCondition.WinByTile;
    [SerializeField] private GameObject fen_point;
    [SerializeField] private GameObject fen_tile;
    [SerializeField] private GameObject fen_time;

    int win_time_sec = 0;
    int win_time_min = 10;

    long time_start_of_game = 0;
    
    int win_tile_nb = 70;
    int nb_tile_drawn = 0;
    int win_point_nb;
    
    private int nb_meeple = 10;

    private int compteur_de_tour = 0;
    private int last_generated_tile_tour = -1;

    private TurnPlayParam act_turn_play;

    private List<Position> possibilities_act_turn = new List<Position>();
    private List<ulong> tile_drawn = new List<ulong>(); 
    
    [SerializeField] private DisplaySystem system_display;

    void Start()
    {
        dicoTuile = LireXML2.Read(XML_PATH);
        _plateau = new Plateau(dicoTuile);
        gameStart();
    }

    public void pressed()
    {

    }

    bool validate_start()
    {
        bool valid = true;
        switch (my_wincond)
        {
            case WinCondition.WinByPoint:
            valid = win_point_nb > 0;
            break;
            case WinCondition.WinByTile:
            valid = win_time_min > 0 && win_time_sec >= 0;
            break;
            case WinCondition.WinByTime:
            valid = win_point_nb > 0;
            break;
        }
        valid = valid && nb_player >= 0;

        return valid;
    }

    void generatePlayers()
    {
        for (int i = 0; i < nb_player; i++)
        {
            players.Add(new PlayerInitParam(i, nb_meeple, "Joueur "+(i+1).ToString()));
        }
    }

    public void gameStart()
    {
        if (validate_start())
        {
            _plateau.PoserTuile((ulong) askIdTileInitial(), 0, 0, 0);

            switch (my_wincond)
            {
                case WinCondition.WinByTime:
                    time_start_of_game = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    break;
            }

            generatePlayers();
            system_display.gameBegin();
        }
        else
        {
            error_msg.text = "Paramètres invalides";
        }
    }

    void newTurn()
    {
        system_display.setNextState(DisplaySystemState.turnStart);
        index_player = (index_player + 1)%players.Count;
        compteur_de_tour += 1;
    }


    override public void sendTile(TurnPlayParam play)
    {
        Debug.Log("Am i looking likethis "+play.id_tile+" "+play.tile_pos+" "+play.id_meeple+" "+play.slot_pos);
        bool tuile_valide = false;
        bool meeple_valide = false;
        ulong player_act = (ulong) players[index_player].id_player;
        if (play.id_tile != -1 && _plateau.PlacementLegal((ulong) play.id_tile, play.tile_pos.X, play.tile_pos.Y, play.tile_pos.Rotation))
        {
            _plateau.PoserTuile((ulong)play.id_tile, play.tile_pos.X, play.tile_pos.Y, play.tile_pos.Rotation);
            tuile_valide = true;
        }
        if (play.id_meeple != -1 && tuile_valide && _plateau.PionPosable((ulong) play.id_meeple, (ulong) play.slot_pos, player_act))
        {
            _plateau.PoserPion(player_act, (ulong)play.id_tile, (ulong)play.slot_pos);
            meeple_valide = true;
        }

        //TODO regarder avancement score pour tout les joueurs int  += CompteurPoint.CompterZoneFerme(play.id_tile, play.slot_pos);
        
        bool end = false;
        bool score_changed = false;
        switch (my_wincond)
        {
            case WinCondition.WinByTime:

                break;
            case WinCondition.WinByPoint:
                for (int i = 0; i < players.Count; i++)
                {
                    end = end || (players_score[index_player] >= win_point_nb);
                }
                break;
            case WinCondition.WinByTile:
                end = _plateau.GetTuiles.Length >= win_tile_nb;
                break;
        }
        act_turn_play = new TurnPlayParam(tuile_valide ? play.id_tile : -1, tuile_valide ? play.tile_pos : null, meeple_valide ? play.id_meeple : -1, meeple_valide ? play.slot_pos : -1);
        if (end)
        {
            system_display.setNextState(DisplaySystemState.endOfGame);
        }
        else if (score_changed)
        {
            system_display.setNextState(DisplaySystemState.scoreChange);
            newTurn();
        }
        else
        {
            newTurn();
        } 

    }

    override public void getTile(out TurnPlayParam play)
    {
        play = act_turn_play;
    }

    override public void askMeeplesInit(List<MeepleInitParam> meeples)
    {
    }

    private void generateTile()
    {
        if (last_generated_tile_tour >= compteur_de_tour)
        {
            Debug.Log("Shouldn't generate new tile");
        }
        else
        {
            possibilities_act_turn.Clear();
            tile_drawn.Clear();
            do{
                int index = UnityEngine.Random.Range(0, dicoTuile.Count);
                tile_drawn.Add((ulong) index);
                possibilities_act_turn.AddRange(_plateau.PositionPlacementPossible(tile_drawn[tile_drawn.Count - 1]));
                nb_tile_drawn += 1;
            }while(possibilities_act_turn.Count <= 0 && nb_tile_drawn < win_tile_nb);
            last_generated_tile_tour = compteur_de_tour;
        }
    }
    override public int askTilesInit(List<TileInitParam> tiles)
    {
        int tot = 0;
        generateTile();

        for (int i = 0; i<tile_drawn.Count; i++)
        {
            tiles.Add(new TileInitParam((int) tile_drawn[i], i + 1 == tile_drawn.Count));
        }
        return 1;
    }

    override public void askPlayersInit(List<PlayerInitParam> players)
    {
        players.AddRange(this.players);
    }

    override public void getTilePossibilities(int tile_id, List<PositionRepre> positions)
    {
        foreach (Position pos in possibilities_act_turn)
        {
            Debug.Log("POSITION POSSIBLE : "+pos.ToString());
            positions.Add(new PositionRepre(pos.X, pos.Y, pos.ROT));
        }
    }

    override public void askPlayerOrder(List<int> player_ids)
    {

    }

    override public void askScores(List<PlayerScoreParam> players_scores)
    {
    }

    override public int askIdTileInitial()
    {
        return 20;
    }

    override public int getNextPlayer()
    {
        return players[index_player].id_player;
    }

    override public int getMyPlayer()
    {
        return -1;
    }

    override public void askTimerTour(out int min, out int sec)
    {
        min = 1;
        sec = 0;
    }

    override public void sendAction(DisplaySystemAction action)
    {
    }

    override public void askWinCondition(ref WinCondition win_cond, List<int> parameters)
    {
        win_cond = my_wincond;
        switch (win_cond)
        {
            case WinCondition.WinByTime:
                parameters.Add(win_time_min);
                parameters.Add(win_time_sec);
                break;
            case WinCondition.WinByPoint:
                parameters.Add(win_point_nb);
                break;
            case WinCondition.WinByTile:
                parameters.Add(win_tile_nb);
                break;
        }
    }

    override public void askFinalScore(List<PlayerScoreParam> playerScores)
    {
        //TODO Pareil que askScore
    }
}
