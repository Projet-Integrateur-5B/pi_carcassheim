using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AccountMenu : MonoBehaviour {
    private static ConnectionMenu co;
    private static Miscellaneous ms;

    // Start is called before the first frame update
    void Start()
    {
        // SCRIPT :
        ms = gameObject.AddComponent(typeof(Miscellaneous)) as Miscellaneous;
        co = gameObject.AddComponent(typeof(ConnectionMenu)) as ConnectionMenu;
    }

    // Update is called once per frame
    void Update() { }
}