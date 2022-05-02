using System;
using UnityEngine;
using UnityEngine.UI;
using Assets.System;
using UnityEngine.SceneManagement;
using System.Collections;

public class HomeMenu : Miscellaneous
{
    private Transform HCB; // Home Container Buttons
    private Color btnInactivColor;
    void Start()
    {
        // INITIALISATION
        HCB = GameObject.Find("SubMenus").transform.Find("HomeMenu").transform.Find("Buttons").transform;
        HCB.Find("ShowRoomSelection").GetComponent<Button>().interactable = GetState();
        HCB.Find("ShowStat").GetComponent<Button>().interactable = GetState();
        ColorUtility.TryParseHtmlString("#808080", out btnInactivColor);
        HCB.Find("ShowRoomSelection").GetComponent<Button>().GetComponentInChildren<Text>().color = btnInactivColor;
        HCB.Find("ShowStat").GetComponent<Button>().GetComponentInChildren<Text>().color = btnInactivColor;
    }

    public void ShowSolo()
    {
        StartCoroutine(LoadLocal());
        gameObject.SetActive(false);
    }

    public void ShowConnection()
    {
        ChangeMenu("HomeMenu", "ConnectionMenu");
    }

    public void ShowRoomSelection()
    {
        ChangeMenu("HomeMenu", "RoomSelectionMenu");
        /* SceneManager.LoadScene("InGame"); */
    }

    public void ShowOptions()
    {
        ChangeMenu("HomeMenu", "OptionsMenu");
    }

    public void ShowStat()
    {
        ChangeMenu("HomeMenu", "StatMenu");
    }

    public void QuitGame() // A LA FIN : quand tout fonctionnera : RemoveAllListeners(); (bouton -> "free")
    {
        Communication.Instance.LancementDeconnexion();
        ChangeMenu("HomeMenu", "ConnectionMenu");
        Application.Quit();
    }


    IEnumerator LoadLocal()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("InGame_LOCAL");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}