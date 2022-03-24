using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    [SerializeField] private GameObject test_spawn;
    [SerializeField] private candle_manager candle;
    // Start is called before the first frame update
    void Start()
    {
        if (test_spawn != null)
            Instantiate(test_spawn);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
