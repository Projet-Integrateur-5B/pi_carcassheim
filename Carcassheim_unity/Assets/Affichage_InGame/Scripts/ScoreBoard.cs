using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text playerScore;
    [SerializeField] private TMP_Text playerRank;

    [SerializeField] private Button validate;
    [SerializeField] DisplaySystem system;

    [SerializeField] private bool end_of_game;
    // Start is called before the first frame update
    void Start()
    {
        playerName.text = "0";
        playerScore.text = "0";
        playerRank.text = "0";

        validate.onClick.AddListener(Quit);

        end_of_game = false;
        gameObject.SetActive(false);
    }

    public void setEndOfGame(PlayerRepre player, int rank)
    {
        playerName.text = player.Name;
        playerScore.text = "Score : " + player.Score.ToString();
        playerRank.text = "Rank : " + rank.ToString();

        end_of_game = true;
        gameObject.SetActive(true);
    }
    public void Quit()
    {
        // Debug.Log("Call system state");
        Application.Quit(0);
    }
}
