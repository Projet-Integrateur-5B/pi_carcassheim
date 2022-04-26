using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.system;
using TMPro;
public class backLocal : CarcasheimBack
{
    private const string XML_PATH = "";
    private Dictionary<ulong, Tuile> dicoTuile;
    private Plateau _plateau;
    private List<PlayerInitParam> players = new List<PlayerInitParam>(); 
    private List<int> players_score = new List<int>();

    private int index_player = 0; // joueur en jeu
    private int nb_player = 0; // à remplir via field

    [SerializeField] private TMP_Text error_msg;

    private WinCondition my_wincond = WinCondition.WinByTile;
    [SerializeField] private GameObject fen_point;
    [SerializeField] private GameObject fen_tile;
    [SerializeField] private GameObject fen_time;

    int win_time_sec = 0;
    int win_time_min = 10;
    
    int win_tile_nb;
    int win_point_nb;
    
    private int nb_meeple = 10;
    
    [SerializeField] private DisplaySystem system_display;

    void Awake()
    {
        dicoTuile = LireXML2.Read(XML_PATH);
        _plateau = new Plateau(dicoTuile);
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
            players.Add(new PlayerInitParam(i, nb_meeple, "Joueur "+i.ToString()));
        }
    }

    void gameStart()
    {
        if (validate_start())
        {
            generatePlayers();
            system_display.gameBegin();
        }
        else
        {
            error_msg.text = "Paramètres invalides";
        }
    }


    override public void sendTile(TurnPlayParam play)
    {
        Debug.Log("Am i looking likethis");/*
        if (num_turn + 1 >= nb_turn)
        {
            Debug.Log("Merde");
            system_display.setNextState(DisplaySystemState.endOfGame);
        }
        else
        {
            Debug.Log("Score + turn");
            system_display.setNextState(DisplaySystemState.scoreChange);
            system_display.setNextState(DisplaySystemState.turnStart);
        }*/
        _plateau.PoserTuile((ulong)play.id_tile, new Position(play.tile_pos.X, play.tile_pos.Y, play.tile_pos.Rotation));
        _plateau.PoserPion((ulong)index_player, (ulong)play.id_tile, (ulong)play.slot_pos);

        //player_score[index_player] += CompteurPoint.CompterZoneFerme(play.id_tile, play.slot_pos);
        /*
        switch (my_wincond)
        {
            case WinCondition.WinByTime:
                break;
            case WinCondition.WinByPoint:
                if (player_score[index_player] >= win_point_nb)
                    ;
                break;
            case WinConditon.WinByTile:
                if (_plateau.GetTuiles.Length >= win_tile_nb)
                    ;
                break;
        }*/
    }

    override public void getTile(out TurnPlayParam play)
    {
        play = new TurnPlayParam(-1, null, -1, -1);
    }

    override public void askMeeplesInit(List<MeepleInitParam> meeples)
    {
    }

    override public int askTilesInit(List<TileInitParam> tiles)
    {
        int tot = 0;
        /**
        foreach (TileInitParam param in tiles)
        {
            Tuile tl = dicoTuile[(ulong)param.id_tile];
            var positionPossible = _plateau.PositionsPlacementPossible((ulong)param.id_tile);

            if (positionPossible != null && positionPossible.Length > 0)
            {
                param.tile_flags = true;
                tot++;
            }
        }
        **/
        return tot;
    }

    override public void askPlayersInit(List<PlayerInitParam> players)
    {
        players.AddRange(this.players);
    }

    override public void getTilePossibilities(int tile_id, List<PositionRepre> positions)
    {

        Position[] temp = _plateau.PositionsPlacementPossible(dicoTuile[(ulong)tile_id]);

        foreach (var item in temp)
        {
            positions.Add(new PositionRepre(item.X, item.Y, item.ROT));
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
        return 0;
    }

    override public int getMyPlayer()
    {
        return 0;
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
        win_cond = win_cond;
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
