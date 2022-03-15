using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerList : MonoBehaviour
{

    public Transform waiting_players;
    public GameObject player_repre_model;
    public DisplaySystem master;

    private LinkedList<PlayerRepre> m_players;
    private LinkedList<GameObject> m_players_repre;

    // Start is called before the first frame update
    void Start()
    {
        m_players = new LinkedList<PlayerRepre>();
        m_players_repre = new LinkedList<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool addPlayer(PlayerRepre player)
    {
        foreach (PlayerRepre pl in m_players)
        {
            if (pl.Id == player.Id)
            {
                return false;
            }
        }

        GameObject repre = Instantiate<GameObject>(player_repre_model);
        PlayerIcon ic = repre.GetComponent<PlayerIcon>();
        ic.setPlayer(player);
        if (m_players.Count == 0)
        {
            m_players.AddLast(player);
            m_players_repre.AddLast(repre);
            repre.transform.SetParent(transform, false);
            repre.transform.SetAsFirstSibling();
        }
        else
        {
            m_players.AddBefore(m_players.Last, player);
            m_players_repre.AddBefore(m_players_repre.Last, repre);
            repre.transform.SetParent(waiting_players, false);
        }
        return true;
    }

    public PlayerRepre getActPlayer()
    {
        LinkedListNode<PlayerRepre> pl_node = m_players.Last;
        return pl_node.Value;
    }

    public bool nextPlayer(PlayerRepre player)
    {
        if (m_players.Count < 2)
        {
            Debug.Log("No player to cycle trough");
            return false;
        }

        LinkedListNode<PlayerRepre> pl_node = m_players.First;
        LinkedListNode<GameObject> repre_node = m_players_repre.First;
        LinkedListNode<GameObject> prev_repre_node = m_players_repre.Last;
        m_players.RemoveFirst();
        m_players.AddLast(pl_node);
        m_players_repre.RemoveFirst();
        m_players_repre.AddLast(repre_node);

        PlayerRepre pl = pl_node.Value;
        GameObject repre = repre_node.Value;
        GameObject prev_repre = prev_repre_node.Value;
        /*if (pl.Id != player.Id)
        {
            master.askPlayerOrder(m_players);
            //TODO treat correctly the case where the order change
        }*/

        repre.transform.SetParent(transform, false);
        prev_repre.transform.SetParent(waiting_players, false);
        repre.transform.SetAsFirstSibling();
        return true;
    }
}
