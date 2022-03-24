using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Banner : MonoBehaviour
{

    public TimerRepre timerTour = null;
    [SerializeField] private TMP_Text nbPlayerTMP;
    [SerializeField] private TMP_Text nbMeepleTMP;
    [SerializeField] private TMP_Text timerTourTMP;
    [SerializeField] private TMP_Text nbPointsTMP;
    [SerializeField] private DisplaySystem master;

    private PlayerRepre _player = null;
    uint _nb_player = 0;
    // Start is called before the first frame update

    void Start()
    {
        nbPlayerTMP.text = "0";
        nbMeepleTMP.text = "0";
        nbPointsTMP.text = "0";
    }

    void OnEnable()
    {
        if (_player != null)
        {
            _player.OnMeepleUpdate += meepleUpdated;
            _player.OnScoreUpdate += scoreUpdated;
        }
        if (timerTour != null)
        {
            timerTour.OnSecondPassed += tourUpdated;
        }
        master.OnPlayerDisconnected += playerDisconnected;
    }

    void OnDisable()
    {
        if (_player != null)
        {
            _player.OnMeepleUpdate -= meepleUpdated;
            _player.OnScoreUpdate -= scoreUpdated;
        }
        if (timerTour != null)
        {
            timerTour.OnSecondPassed -= tourUpdated;
        }
        master.OnPlayerDisconnected -= playerDisconnected;
    }

    public void playerDisconnected(PlayerRepre player)
    {
        if (_nb_player > 0)
            _nb_player -= 1;
        nbPlayerTMP.text = _nb_player.ToString();
    }

    public void setPlayer(PlayerRepre player)
    {
        if (_player != null)
        {
            _player.OnMeepleUpdate -= meepleUpdated;
            _player.OnScoreUpdate -= scoreUpdated;
        }
        _player = player;
        if (_player != null)
        {
            _player.OnMeepleUpdate += meepleUpdated;
            _player.OnScoreUpdate += scoreUpdated;
        }
    }

    public void setPlayerNumber(uint number)
    {
        _nb_player = number;
        nbPlayerTMP.text = _nb_player.ToString();
    }

    public void setTimerTour(TimerRepre timer)
    {
        timerTour.setTime(timer);
        timerTour.OnSecondPassed += tourUpdated;
        timerTourTMP.text = timer.ToString();
    }

    void scoreUpdated(uint old_score, uint new_score)
    {
        nbPointsTMP.text = new_score.ToString();
    }

    void meepleUpdated(uint meeple)
    {
        nbMeepleTMP.text = meeple.ToString();
    }

    void playerUpdated(uint nb_player)
    {
        _nb_player = nb_player;
        nbPlayerTMP.text = nb_player.ToString();
    }

    void tourUpdated()
    {
        timerTourTMP.text = timerTour.ToString();
    }

}
