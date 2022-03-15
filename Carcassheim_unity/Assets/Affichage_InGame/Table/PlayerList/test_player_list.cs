using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_player_list : MonoBehaviour
{
    public PlayerList pl_list;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlayerRepre pl = new PlayerRepre();
            pl_list.addPlayer(pl);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            pl_list.nextPlayer(null);
        }
        else if (Input.GetKeyDown("m"))
        {
            PlayerRepre pl = pl_list.getActPlayer();
            pl.NbMeeple += 1;
        }
        else if (Input.GetKeyDown("s"))
        {
            PlayerRepre pl = pl_list.getActPlayer();
            pl.Score += 10;
        }
    }
}
