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

    // Update is called once per frame
    void Update()
    {
        if (end_of_game)
        {
            gameObject.SetActive(true);
            
        }
    }


    public void setEndOfGame(PlayerRepre player)
    {
        playerName.text = player.Name;
        playerScore.text = "Score : " + player.Score.ToString();
        playerRank.text = "Rank : 0";

        end_of_game = true;
    }
    public void Quit()
    {
        Debug.Log("Call system state");
        Application.Quit(0);
    }
}
