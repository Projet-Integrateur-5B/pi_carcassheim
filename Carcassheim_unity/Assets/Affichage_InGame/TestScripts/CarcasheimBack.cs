using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarcasheimBack : MonoBehaviour
{

    public int tile_number;

    public List<int> players;
    public int player_index;

    // Start is called before the first frame update
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
        }
    }

    // Update is called once per frame
    void Update()
    {

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
                tile_ids.Add(Random.Range(1, 100));
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

    public void askPlayerOrder(List<int> player_ids)
    {
        player_ids.AddRange(players);
    }

    public int getNextPlayer()
    {
        player_index = (player_index + 1) % players.Count;
        return players[player_index];
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



}
