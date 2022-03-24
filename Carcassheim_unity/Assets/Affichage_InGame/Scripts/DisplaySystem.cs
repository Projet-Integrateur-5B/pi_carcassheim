using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaySystem : MonoBehaviour
{

    public event System.Action<PlayerRepre> OnPlayerDisconnected;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void askPlayerOrder(LinkedList<PlayerRepre> players)
    {
        players.Clear();
        Debug.Log("Should call back");

    }
}
