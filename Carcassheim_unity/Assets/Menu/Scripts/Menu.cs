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

  // Start is called before the first frame update
  void Start() {
    /* optionsMenu.SetActive(false); */
  }

  // Update is called once per frame
  void Update() {

  }

  public void OnPointerEnter(PointerEventData eventData) {
    Color newCol;

    if (ColorUtility.TryParseHtmlString("#6F00FF", out newCol)) {
      HighlightHere.GetComponent < Text > ().color = newCol;
    } else {
      HighlightHere.GetComponent < Text > ().color = Color.blue;
    }
  }

  public void OnPointerExit(PointerEventData eventData) {
    Color newCol;

    if (ColorUtility.TryParseHtmlString("#07062e", out newCol)) {
      HighlightHere.GetComponent < Text > ().color = newCol;
    } else {
      HighlightHere.GetComponent < Text > ().color = Color.black;
    }
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

  public void Quitter() {
    Application.Quit();
    Debug.Log("Quit!");
  }
}