using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WinCondition
{
    WinByTime,
    WinByPoint,
    WinByTile
};
public class CarcasheimBack : MonoBehaviour
{

    public int tile_number;

    public List<int> players;
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
            } while (players.Contains(id));
            players.Add(id);
            players_names.Add(randomStrong(Random.Range(8, 20)));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void sendTile(int tile_id, Position tile_pos, int id_meeple, int slot_pos)
    {

    }

    public void getTile(out int tile_id, out Position pos, out int id_meeple, out int slot_pos)
    {
        tile_id = -1;
        pos = null;
        id_meeple = -1;
        slot_pos = -1;
    }

    public void askMeeples(IList<MeepleType> meeples, IList<int> meeple_number)
    {
        do
        {
            foreach (MeepleType mt in System.Enum.GetValues(typeof(MeepleType)))
            {
                if (Random.Range(0f, 1f) < 0.3f)
                {
                    meeples.Add(mt);
                    meeple_number.Add(Random.Range(1, 24));
                }
            }
        } while (meeples.Count <= 0);
    }

    public int askTiles(IList<int> tile_ids, IList<bool> tile_flags)
    {
        int total = Random.Range(1, 10);
        int true_total = 0;
        do
        {
            for (int i = 0; i < total; i++)
            {
                tile_ids.Add(Random.Range(0, 6));
                bool tile_flag = (i == total - 1) || (true_total < 4 && Random.Range(0f, 1f) < 0.4);
                tile_flags.Add(tile_flag);
                if (tile_flag)
                {
                    true_total++;
                }
            }
        } while (true_total < 0);
        return true_total;
    }

    public void askPlayers(List<int> player_ids, List<string> players_name)
    {
        player_ids.AddRange(players);
        players_name.AddRange(players_names);
    }

    public void getTilePossibilities(int tile_id, List<Position> positions)
    {
        for (int r = 0; r < 4; r++)
            positions.Add(new Position(0, 0, r));
    }

    public void askPlayers(List<int> player_ids)
    {
        player_ids.AddRange(players);
    }

    public void askPlayerOrder(List<int> player_ids)
    {
        player_ids.AddRange(players);
    }

    public int getNextPlayer()
    {
        if (!first)
        {
            player_index = (player_index + 1) % players.Count;
            return players[player_index];
        }
        else
        {
            first = false;
            return players[player_index];
        }
    }

    public int getMyPlayer()
    {
        return players[Random.Range(0, players.Count)];
    }

    public void askTimerTour(out int min, out int sec)
    {
        min = 1;
        sec = 0;
    }

    public void sendAction(DisplaySystemAction action)
    {
        return;
    }


    public void askWinCondition(ref WinCondition win_cond, List<int> parameters)
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
