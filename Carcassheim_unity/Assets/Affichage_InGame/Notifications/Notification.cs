using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Notification : MonoBehaviour
{
    [SerializeField] private bool notification = false;
    [SerializeField] private string message;
    [SerializeField] private TMP_Text messageBox;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!notification)
            gameObject.SetActive(false);
        messageBox.text = message;
    }

    // Update is called once per frame
    void Update()
    {
        if (notification)
        {
            gameObject.SetActive(true);
        }
        
    }

    string getMessageFromServer()
    {
        return "";
    }

    void setMessage(string m)
    {
        message = m;
        messageBox.text = message;
    }
}
