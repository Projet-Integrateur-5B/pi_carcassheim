using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerIcon : MonoBehaviour
{
    public Image player_color;
    public TMP_Text player_score;
    public TMP_Text player_meeple;
    public PlayerRepre Player { get => _player; private set => setPlayer(value); }
    private PlayerRepre _player;

    public void Start()
    {
        player_score.text = "0";
        player_meeple.text = "0";
    }

    public void setPlayer(PlayerRepre player)
    {
        if (_player != null)
        {
            _player.OnMeepleUpdate -= meepleUpdated;
            _player.OnScoreUpdate -= scoreUpdated;

        }
        player.OnMeepleUpdate += meepleUpdated;
        player.OnScoreUpdate += scoreUpdated;
        player_color.color = player.color;
    }

    public void OnDestroy()
    {
        if (_player != null)
        {
            _player.OnMeepleUpdate -= meepleUpdated;
            _player.OnScoreUpdate -= scoreUpdated;
        }
    }

    public void meepleUpdated(uint nb_meeple)
    {
        player_meeple.text = nb_meeple.ToString();
    }

    public void scoreUpdated(uint old_score, uint new_score)
    {
        player_score.text = new_score.ToString();
    }

}
