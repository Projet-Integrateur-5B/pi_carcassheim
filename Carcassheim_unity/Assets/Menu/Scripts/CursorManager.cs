using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour,
                             IPointerEnterHandler,
                             IPointerExitHandler,
                             IPointerClickHandler {

    private HomeMenu home;
    private OptionsMenu option;
    private ConnectionMenu co;
    private Miscellaneous ms;

    private Texture2D cursorTexture;
    private CursorMode cursorMode = CursorMode.Auto;
    private Vector2 cursorHotspot = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        // Cursor Texture :
        cursorTexture = Resources.Load("basic_01 BLUE") as Texture2D;
        Cursor.SetCursor(cursorTexture, cursorHotspot, cursorMode);

        // SCRIPT :
        home = gameObject.AddComponent(typeof(HomeMenu)) as HomeMenu;
        option = gameObject.AddComponent(typeof(OptionsMenu)) as OptionsMenu;
        co = gameObject.AddComponent(typeof(ConnectionMenu)) as ConnectionMenu;
        ms = gameObject.AddComponent(typeof(Miscellaneous)) as Miscellaneous;
    }

    // Update is called once per frame
    void Update() { }

    public void OnPointerClick(PointerEventData eventData)
    {

        ms.tryColorText(GameObject.Find(name)
                            .GetComponent<Button>()
                            .GetComponentInChildren<Text>(),
            Color.white, "#f4fefe");
        GameObject.Find("SoundController").GetComponent<AudioSource>().Play();

        switch (name) {
        // HomeMenu :
        case "Btn Connexion":
            home.ShowConnection();
            break;
        case "Btn Jouer":
            home.Jouer();
            break;
        case "Btn Options":
            home.ShowOptions();
            break;
        case "Btn Statistiques":
            home.Statistiques();
            break;
        case "Btn Quitter le jeu":
            home.Quitter();
            break;

        // OptionsMenu :
        case "Btn Retour Opt":
            option.HideOptions();
            break;
        case "Btn Son":
            option.SwitchSound();
            break;
        case "Btn Musique":
            option.SwitchMusic();
            break;
        case "Btn Fenêtré":
            option.FullScreen();
            break;
        case "Btn Aide":
            option.Help();
            break;

        // ConnectionMenu :
        case "Btn Retour Co":
            co.HideConnection();
            break;
        case "Btn Se Connecter":
            co.Connect();
            break;
        case "Btn Creer un compte":
            co.CreateAccount();
            break;

        default:
            return;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        bool tmpBool = ms.StrCompare(name, "Btn Jouer")
            || ms.StrCompare(name, "Btn Statistiques");

        if (!co.getState() && tmpBool) {
            ms.tryColorText(GameObject.Find(name)
                                .GetComponent<Button>()
                                .GetComponentInChildren<Text>(),
                Color.grey, "#808080");
        } else {
            ms.tryColorText(GameObject.Find(name)
                                .GetComponent<Button>()
                                .GetComponentInChildren<Text>(),
                Color.blue, "#1e90ff");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        bool tmpBool = ms.StrCompare(name, "Btn Jouer")
            || ms.StrCompare(name, "Btn Statistiques");
        if (co.getState() || !tmpBool) {
            ms.tryColorText(GameObject.Find(name)
                                .GetComponent<Button>()
                                .GetComponentInChildren<Text>(),
                Color.white, "#f4fefe");
        }
    }
}