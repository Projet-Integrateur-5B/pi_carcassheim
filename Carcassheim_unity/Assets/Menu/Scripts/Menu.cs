using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Menu: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
  public GameObject mainMenu;
  public GameObject optionsMenu;

  public GameObject HighlightHere;
  public Button button;

  public AudioSource musicSource;
  public Text musicText;

  public AudioSource soundSource;
  public Text soundText;

  public GameObject ConnectState;
  public Button StateButtonPlay;
  public Button StateButtonStat;
  public bool State;

  // Start is called before the first frame update
  void Start() {
    State = false;
    // Null Reference Exceptions WARNING (A résoudre !)
    if (!State) { 
      StateButtonPlay.interactable = !StateButtonPlay.interactable;
      StateButtonStat.interactable = !StateButtonStat.interactable;
      Debug.Log(StateButtonPlay.name);
      Debug.Log(StateButtonStat.name);
    }

    /* optionsMenu.SetActive(false); */
  }

  // Update is called once per frame
  void Update() {

  }

  public void OnPointerEnter(PointerEventData eventData) {
    tryColor(HighlightHere, Color.blue, "#6F00FF");
  }

  public void OnPointerExit(PointerEventData eventData) {
    tryColor(HighlightHere, Color.black, "#07062e");
  }

  /* ---------------------------------------------------------------------------------------------- */
  /* ------------------------------------ MISCELLANEOUS methods ----------------------------------- */
  /* ---------------------------------------------------------------------------------------------- */

  public void tryColor(GameObject change, Color defaultColor, string coloration) {
    Color newCol;

    if (ColorUtility.TryParseHtmlString(coloration, out newCol)) {
      change.GetComponent < Text > ().color = newCol;
    } else {
      change.GetComponent < Text > ().color = defaultColor;
    }
  }

  /* ---------------------------------------------------------------------------------------------- */
  /* ------------------------------------------- ACCUEIL ------------------------------------------ */
  /* ---------------------------------------------------------------------------------------------- */

  public void Connect() {
    tryColor(ConnectState, Color.green, "#90EE90");
    ConnectState.GetComponent < Text > ().text = "Connecte";
    ConnectState.GetComponent < Text > ().transform.position = new Vector3(1275, 575, 0);
    HighlightHere.SetActive(false);
    StateButtonPlay.interactable = !StateButtonPlay.interactable;
    StateButtonStat.interactable = !StateButtonStat.interactable;
    Debug.Log("Connecté");
  }

  public void Jouer() {
    Application.OpenURL("https://www.epicgames.com/store/fr/p/carcassonne");
  }

  public void Statistiques() {
    Application.OpenURL("https://fr.wikipedia.org/wiki/Statistique");
  }

  public void ShowOptions() {
    mainMenu.SetActive(false);
    optionsMenu.SetActive(true);
    HighlightHere.GetComponent < Text > ().color = Color.black;
  }

  public void HideOptions() {
    optionsMenu.SetActive(false);
    mainMenu.SetActive(true);
    HighlightHere.GetComponent < Text > ().color = Color.black;
  }

  public void Quitter() {
    Application.Quit();
    Debug.Log("Quit!");
  }

  /* ---------------------------------------------------------------------------------------------- */
  /* ------------------------------------------- OPTIONS ------------------------------------------ */
  /* ---------------------------------------------------------------------------------------------- */

  public void SwitchMusic() {
    if (musicSource.volume > 0) {
      musicSource.volume = 0;
      musicText.text = "Musique 'OFF'";
    } else {
      {
        musicSource.volume = 100;
        musicText.text = "Musique 'ON'";
      }
    }
  }

  public void SwitchSounds() {
    if (soundSource.volume > 0) {
      soundSource.volume = 0;
      soundText.text = "Son 'OFF'";
    } else {
      {
        soundSource.volume = 100;
        soundText.text = "Son 'ON'";
      }
    }
  }

  public void FullScreen() {
    Screen.fullScreen = !Screen.fullScreen;
    Debug.Log("Windowed");
  }

  public void Help() {
    Application.OpenURL("https://tinyurl.com/SlapDance");
  }

}