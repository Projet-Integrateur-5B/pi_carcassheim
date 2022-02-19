using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConnectionMenu : MonoBehaviour {
    private HomeMenu home;
    private Miscellaneous ms;

    private static bool State = false;

    // Start is called before the first frame update
    void Start()
    {
        // SCRIPT :
        ms = gameObject.AddComponent(typeof(Miscellaneous)) as Miscellaneous;
        home = gameObject.AddComponent(typeof(HomeMenu)) as HomeMenu;
    }

    // Update is called once per frame
    void Update() { }

    public bool getState() { return State; }

    public void HideConnection()
    {
        ms.changeMenu(ms.FindMenu("ConnectionMenu"), ms.FindMenu("HomeMenu"));
    }

    public void Connect()
    {
        bool a = ms.StrCompare(GameObject.Find("InputField Email/Login")
                                   .GetComponent<InputField>()
                                   .text,
            "Hello");
        bool b = ms.StrCompare(GameObject.Find("InputField Password")
                                   .GetComponent<InputField>()
                                   .text,
            "World");
        State = a && b;

        if (State) {
            Color newCol;
            ms.tryColor(GameObject.Find("Instructions"), Color.white, "f4fefe");
            GameObject.Find("Instructions").GetComponent<Text>().text
                = "Connectez vous";
            HideConnection();
            ms.tryColor(
                GameObject.Find("Etat de connexion"), Color.green, "#90EE90");
            GameObject.Find("Etat de connexion").GetComponent<Text>().text
                = "Connecte";
            GameObject.Find("Etat de connexion").transform.position
                = new Vector3(1250, 475, 0);
            GameObject.Find("Btn Connexion").SetActive(false);
            GameObject.Find("Btn Jouer").GetComponent<Button>().interactable
                = true;
            GameObject.Find("Btn Statistiques")
                .GetComponent<Button>()
                .interactable
                = true;

            ColorUtility.TryParseHtmlString("#f4fefe", out newCol);
            GameObject.Find("Btn Jouer")
                .GetComponent<Button>()
                .GetComponentInChildren<Text>()
                .color
                = newCol;
            GameObject.Find("Btn Statistiques")
                .GetComponent<Button>()
                .GetComponentInChildren<Text>()
                .color
                = newCol;
            Debug.Log("Connecté");
        } else {
            ms.randomIntColor(GameObject.Find("Instructions"));
            GameObject.Find("Instructions").GetComponent<Text>().text
                = "Ressaissiez votre login et votre mot de passe !";
        }
    }

    public void CreateAccount()
    {
        ms.tryColor(GameObject.Find("Instructions"), Color.red, "#fcdc12");
        GameObject.Find("Instructions").GetComponent<Text>().text
            = "IL FAUT ENCORE LE FAIRE !";
    }
}