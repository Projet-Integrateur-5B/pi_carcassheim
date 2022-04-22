using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayClientInterface : CarcasheimBack
{
    public override int askIdTileInitial()
    {
        throw new System.NotImplementedException();
    }

    public override void askMeeplesInit(List<MeepleInitParam> meeples)
    {
        throw new System.NotImplementedException();
    }

    public override void askPlayerOrder(List<int> player_ids)
    {
        throw new System.NotImplementedException();
    }

    public override void askPlayersInit(List<PlayerInitParam> players)
    {
        throw new System.NotImplementedException();
    }

    public override void askScores(List<PlayerScore> players_scores)
    {
        throw new System.NotImplementedException();
    }

    public override int askTilesInit(List<TileInitParam> tiles)
    {
        throw new System.NotImplementedException();
    }

    public override void askTimerTour(out int min, out int sec)
    {
        throw new System.NotImplementedException();
    }

    public override void askWinCondition(ref WinCondition win_cond, List<int> parameters)
    {
        throw new System.NotImplementedException();
    }

    public override int getMyPlayer()
    {
        throw new System.NotImplementedException();
    }

    public override int getNextPlayer()
    {
        throw new System.NotImplementedException();
    }

    public override void getTile(out int tile_id, out PositionRepre pos, out int id_meeple, out int slot_pos)
    {
        throw new System.NotImplementedException();
    }

    public override void getTilePossibilities(int tile_id, List<PositionRepre> positions)
    {
        throw new System.NotImplementedException();
    }

    public override void sendAction(DisplaySystemAction action)
    {
        throw new System.NotImplementedException();
    }

    public override void sendTile(int tile_id, PositionRepre tile_pos, int id_meeple, int slot_pos)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
