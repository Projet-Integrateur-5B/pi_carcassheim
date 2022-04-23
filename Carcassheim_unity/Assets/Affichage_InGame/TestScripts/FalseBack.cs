using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FalseBack : CarcasheimBack
{
    public int tile_number;

    public List<int> players_id;
    public List<string> players_names;
    public int player_index;
    public bool first = true;

    // Start is called before the first frame update
    string randomStrong(int nb_char)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[nb_char];

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[Random.Range(0, nb_char - 1)];
        }

        return new string(stringChars);
    }

    void Start()
    {
        tile_number = Random.Range(29, 119);
        int L = Random.Range(2, 6);
        for (int j = 0; j < L; j++)
        {
            int id;
            do
            {
                id = Random.Range(0, 500);
            } while (players_id.Contains(id));
            players_id.Add(id);
            players_names.Add(randomStrong(Random.Range(8, 20)));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    override public void sendTile(int tile_id, PositionRepre tile_pos, int id_meeple, int slot_pos)
    {

    }

    override public void getTile(out int tile_id, out PositionRepre pos, out int id_meeple, out int slot_pos)
    {
        tile_id = -1;
        pos = null;
        id_meeple = -1;
        slot_pos = -1;
    }

    override public void askMeeplesInit(List<MeepleInitParam> meeples)
    {
        do
        {
            for (int i = 0; i < 4; i++)
            {
                if (Random.Range(0f, 1f) < 0.3f)
                {
                    meeples.Add(new MeepleInitParam(i, Random.Range(1, 10)));
                }
            }
        } while (meeples.Count <= 0);
    }

    override public int askTilesInit(List<TileInitParam> tiles)
    {
        int total = Random.Range(1, 10);
        int true_total = 0;
        do
        {
            for (int i = 0; i < total; i++)
            {
                int id = Random.Range(0, 6);
                bool tile_flag = (i == total - 1) || (true_total < 4 && Random.Range(0f, 1f) < 0.4);
                tiles.Add(new TileInitParam(id, tile_flag));
                if (tile_flag)
                {
                    true_total++;
                }
            }
        } while (true_total < 0);
        return true_total;
    }

    override public void askPlayersInit(List<PlayerInitParam> players)
    {
        for (int j = 0; j < players_id.Count; j++)
        {
            int nb_meeple = Random.Range(3, 20);
            players.Add(new PlayerInitParam(players_id[j], nb_meeple, players_names[j]));
        }
    }

    override public void getTilePossibilities(int tile_id, List<PositionRepre> positions)
    {
        int new_pos, new_r;

        do
        {
            for (int i = 0; i < 4; i++)
            {
                if ((new_pos = Random.Range(0, 10)) < 3)
                {
                    new_r = Random.Range(0, 4);
                    for (int r = 0; r < new_r; r++)
                        positions.Add(new PositionRepre(new_pos, new_pos, r));
                }
            }
        } while (positions.Count == 0);
    }

    override public void askPlayerOrder(List<int> player_ids)
    {
        player_ids.AddRange(players_id);
    }

    override public void askScores(List<PlayerScore> players_scores)
    {

    }

    override public int askIdTileInitial()
    {
        return 0;
    }

    override public int getNextPlayer()
    {
        if (!first)
        {
            player_index = (player_index + 1) % players_id.Count;
            return players_id[player_index];
        }
        else
        {
            first = false;
            return players_id[player_index];
        }
    }

    override public int getMyPlayer()
    {
        return players_id[Random.Range(0, players_id.Count)];
    }

    override public void askTimerTour(out int min, out int sec)
    {
        min = 1;
        sec = 0;
    }

    override public void sendAction(DisplaySystemAction action)
    {
        return;
    }


    override public void askWinCondition(ref WinCondition win_cond, List<int> parameters)
    {
        var r = Random.Range(0, 3);
        switch (r)
        {
            case 0:
                win_cond = WinCondition.WinByPoint;
                parameters.Add(Random.Range(100, 1000));
                break;
            case 1:
                win_cond = WinCondition.WinByTime;
                parameters.Add(Random.Range(0, 10));
                parameters.Add(Random.Range(30, 50));
                break;
            case 2:
                win_cond = WinCondition.WinByTile;
                parameters.Add(Random.Range(20, 500));
                break;
        }
    }
}