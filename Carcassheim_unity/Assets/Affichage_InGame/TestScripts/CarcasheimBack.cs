using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum WinCondition
{
    WinByTime,
    WinByPoint,
    WinByTile
};

public struct PlayerInitParam
{
    public PlayerInitParam(int id_player, int nb_meeple, string player_name)
    {
        this.id_player = id_player;
        this.nb_meeple = nb_meeple;
        this.player_name = player_name;
    }

    public int id_player;
    public int nb_meeple;
    public string player_name;
};

public struct MeepleInitParam
{
    public MeepleInitParam(int id_meeple, int nb_meeple)
    {
        this.id_meeple = id_meeple;
        this.nb_meeple = nb_meeple;
    }
    public int id_meeple;
    public int nb_meeple;
};

public struct TileInitParam
{
    public TileInitParam(int id_tile, bool tile_flags)
    {
        this.id_tile = id_tile;
        this.tile_flags = tile_flags;
    }
    public int id_tile;
    public bool tile_flags;
};

public struct Zone
{
    public Zone(PositionRepre pos, int id_tuile, int id_slot)
    {
        this.pos = pos;
        this.id_tuile = id_tuile;
        this.id_slot = id_slot;
    }
    public PositionRepre pos;
    public int id_tuile;
    public int id_slot;
};

public struct PlayerScoreParam
{
    public PlayerScoreParam(int id_player, int points_gagnes, Zone[] zone)
    {
        this.id_player = id_player;
        this.points_gagnes = points_gagnes;
        this.zone = zone;
    }
    public int id_player;
    public int points_gagnes;
    public Zone[] zone;
};

public struct TurnPlayParam
{
    public TurnPlayParam(int id_tile, PositionRepre tile_pos, int id_meeple, int slot_pos)
    {
        this.id_tile = id_tile;
        this.tile_pos = tile_pos;
        this.id_meeple = id_meeple;
        this.slot_pos = slot_pos;
    }

    public int id_tile;
    public PositionRepre tile_pos;
    public int id_meeple;
    public int slot_pos;
}

public abstract class CarcasheimBack : MonoBehaviour
{
    public abstract void sendTile(TurnPlayParam play);

    public abstract void getTile(out TurnPlayParam play);

    public abstract void askMeeplesInit(List<MeepleInitParam> meeples);

    public abstract int askTilesInit(List<TileInitParam> tiles);

    public abstract void askPlayersInit(List<PlayerInitParam> players);

    public abstract void getTilePossibilities(int tile_id, List<PositionRepre> positions);

    public abstract void askPlayerOrder(List<int> player_ids);

    public abstract int getNextPlayer();

    public abstract int getMyPlayer();

    public abstract void askScores(List<PlayerScoreParam> players_scores);

    public abstract void askTimerTour(out int min, out int sec);

    public abstract int askIdTileInitial();

    public abstract void sendAction(DisplaySystemAction action);

    public abstract void askWinCondition(ref WinCondition win_cond, List<int> parameters);

    public abstract void askFinalScore(List<PlayerScoreParam> playerScores);
}