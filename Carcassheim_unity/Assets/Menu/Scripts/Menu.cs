using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject ConnectionMenu;

    public GameObject HighlightHere;
    public Button button;

    public AudioSource musicSource;
    public Text musicText;

    public AudioSource soundSource;
    public Text soundText;

    public GameObject ConnectState;
    public GameObject StateButtonConnect;
    public GameObject StateButtonPlay;
    public GameObject StateButtonStat;
    public static bool State = false;
    public bool Connected;
    public GameObject InputFieldLog;
    public GameObject InputFieldPwd;
    public GameObject Instructions;

    // Start is called before the first frame update
    void
    Start ()
    {
        StateButtonPlay.GetComponent<Button> ().interactable = State;
        StateButtonStat.GetComponent<Button> ().interactable = State;

        Color newCol;

        if (!State && mainMenu.activeSelf)
            {
                ColorUtility.TryParseHtmlString ("#808080", out newCol);
                GameObject.Find ("Btn Jouer")
                    .GetComponent<Button> ()
                    .GetComponentInChildren<Text> ()
                    .color
                    = newCol;
                GameObject.Find ("Btn Statistiques")
                    .GetComponent<Button> ()
                    .GetComponentInChildren<Text> ()
                    .color
                    = newCol;
            }

        /* optionsMenu.SetActive(false); */
    }

    // Update is called once per frame
    void
    Update ()
    {
        /* Debug.Log (StateButtonPlay.GetComponent<Button> ().interactable); */
    }

    public void
    OnPointerEnter (PointerEventData eventData)
    {

        string tmpBtn = button.GetComponent<Button> ().name;
        bool tmpBool = StrCompare (tmpBtn, "Btn Jouer")
                       || StrCompare (tmpBtn, "Btn Statistiques");

        if (!State && tmpBool)
            {
                tryColor (HighlightHere, Color.grey, "#808080");
            }
        else
            {
                tryColor (HighlightHere, Color.blue, "#1e90ff");
            }
        Debug.Log (button.GetComponent<Button> ().name);
    }

    public void
    OnPointerExit (PointerEventData eventData)
    {
        string tmpBtn = button.GetComponent<Button> ().name;
        bool tmpBool = StrCompare (tmpBtn, "Btn Jouer")
                       || StrCompare (tmpBtn, "Btn Statistiques");
        if (State || !tmpBool)
            {
                tryColor (HighlightHere, Color.white, "#f4fefe");
            }
    }

    /* -----------------------------------------------------------------------*/
    /* ------------------------- MISCELLANEOUS methods ---------------------- */
    /* -----------------------------------------------------------------------*/

    public void
    tryColor (GameObject change, Color defaultColor, string coloration)
    {
        Color newCol;

        if (ColorUtility.TryParseHtmlString (coloration, out newCol))
            {
                change.GetComponent<Text> ().color = newCol;
            }
        else
            {
                change.GetComponent<Text> ().color = defaultColor;
            }
    }

    public void
    backButton (GameObject close, GameObject goTo)
    {
        close.SetActive (false);
        goTo.SetActive (true);
        tryColor (HighlightHere, Color.white, "#f4fefe");
    }

    public bool
    StrCompare (string str1, string str2)
    {
        return (str2.Equals (str1));
    }

    public void
    randomIntColor (GameObject GO)
    {
        Color randomColor = new Color (Random.Range (0f, 1f), // Red
                                       Random.Range (0f, 1f), // Green
                                       Random.Range (0f, 1f), // Blue
                                       1 // Alpha (transparency)
        );

        int r = Random.Range (40, 70);
        GO.GetComponent<Text> ().color = randomColor;
        GO.GetComponent<Text> ().fontSize = r;
    }

    /* -----------------------------------------------------------------------*/
    /* -------------------------------- ACCUEIL ----------------------------- */
    /* -----------------------------------------------------------------------*/

    public void
    Jouer ()
    {
        randomIntColor (ConnectState);
    }

    public void
    Statistiques ()
    {
        randomIntColor (ConnectState);
    }

    public void
    ShowOptions ()
    {
        backButton (mainMenu, optionsMenu);
    }

    public void
    ShowConnection ()
    {
        backButton (mainMenu, ConnectionMenu);
    }

    public void
    Quitter ()
    {
        Application.Quit ();
        Debug.Log ("Quit!");
    }

    /* -----------------------------------------------------------------------*/
    /* ------------------------------- OPTIONS ------------------------------ */
    /* -----------------------------------------------------------------------*/

    public void
    HideOptions ()
    {
        backButton (optionsMenu, mainMenu);
    }

    public void
    SwitchMusic ()
    {
        if (musicSource.volume > 0)
            {
                musicSource.volume = 0;
                musicText.text = "Musique 'OFF'";
            }
        else
            {
                {
                    musicSource.volume = 100;
                    musicText.text = "Musique 'ON'";
                }
            }
    }

    public void
    SwitchSounds ()
    {
        if (soundSource.volume > 0)
            {
                soundSource.volume = 0;
                soundText.text = "Son 'OFF'";
            }
        else
            {
                {
                    soundSource.volume = 100;
                    soundText.text = "Son 'ON'";
                }
            }
    }

    public void
    FullScreen ()
    {
        Screen.fullScreen = !Screen.fullScreen;
        Debug.Log ("Windowed");
    }

    public void
    Help ()
    {
        Application.OpenURL ("https://tinyurl.com/SlapDance");
    }

    /* -----------------------------------------------------------------------*/
    /* ----------------------------- CONNEXION ------------------------------ */
    /* -----------------------------------------------------------------------*/

    public void
    HideConnection ()
    {
        backButton (ConnectionMenu, mainMenu);
    }

    public void
    Connect ()
    {
        bool a
            = StrCompare (InputFieldLog.GetComponent<InputField> ().text, "Hello");
        bool b
            = StrCompare (InputFieldPwd.GetComponent<InputField> ().text, "World");
        Connected = a && b;

        if (Connected)
            {
                Color newCol;
                State = true;
                tryColor (Instructions, Color.white, "f4fefe");
                Instructions.GetComponent<Text> ().text = "Connectez vous";
                tryColor (ConnectState, Color.green, "#90EE90");
                ConnectState.GetComponent<Text> ().text = "Connecte";
                ConnectState.GetComponent<Text> ().transform.position
                    = new Vector3 (1275, 575, 0);
                StateButtonConnect.SetActive (false);
                StateButtonPlay.GetComponent<Button> ().interactable = true;
                StateButtonStat.GetComponent<Button> ().interactable = true;
                HideConnection ();
                ColorUtility.TryParseHtmlString ("#f4fefe", out newCol);
                GameObject.Find ("Btn Jouer")
                    .GetComponent<Button> ()
                    .GetComponentInChildren<Text> ()
                    .color
                    = newCol;
                GameObject.Find ("Btn Statistiques")
                    .GetComponent<Button> ()
                    .GetComponentInChildren<Text> ()
                    .color
                    = newCol;
                Debug.Log ("Connecté");
            }
        else
            {
                /* tryColor (Instructions, Color.red, "#FFA500"); */
                randomIntColor (Instructions);
                Instructions.GetComponent<Text> ().text
                    = "Ressaissiez votre login et votre mot de passe !";
            }
    }
    /* -----------------------------------------------------------------------*/
    /* ----------------------------- CREER COMPTE --------------------------- */
    /* -----------------------------------------------------------------------*/

    public void
    CreateAccount ()
    {
        tryColor (Instructions, Color.red, "#fcdc12");
        Instructions.GetComponent<Text> ().text = "IL FAUT ENCORE LE FAIRE !";
    }
}
