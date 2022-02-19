using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionsMenu : MonoBehaviour {

    private ConnectionMenu co;
    private Miscellaneous ms;

    // Start is called before the first frame update
    void Start()
    {
        // SCRIPT :
        ms = gameObject.AddComponent(typeof(Miscellaneous)) as Miscellaneous;
        co = gameObject.AddComponent(typeof(ConnectionMenu)) as ConnectionMenu;
    }

    // Update is called once per frame
    void Update() { }

    public void HideOptions()
    {
        ms.changeMenu(ms.FindMenu("OptionsMenu"), ms.FindMenu("HomeMenu"));
    }

    public void SwitchMusic()
    {
        if (GameObject.Find("MusicController")
                .GetComponent<AudioSource>()
                .volume
            > 0) {
            GameObject.Find("MusicController")
                .GetComponent<AudioSource>()
                .volume
                = 0;
            GameObject.Find("Btn Musique")
                .GetComponent<Button>()
                .GetComponentInChildren<Text>()
                .text
                = "Musique 'OFF'";
        } else {
            GameObject.Find("MusicController")
                .GetComponent<AudioSource>()
                .volume
                = 100;
            GameObject.Find("Btn Musique")
                .GetComponent<Button>()
                .GetComponentInChildren<Text>()
                .text
                = "Musique 'ON'";
        }
    }

    public void SwitchSound()
    {
        if (GameObject.Find("SoundController")
                .GetComponent<AudioSource>()
                .volume
            > 0) {
            GameObject.Find("SoundController")
                .GetComponent<AudioSource>()
                .volume
                = 0;
            GameObject.Find("Btn Son")
                .GetComponent<Button>()
                .GetComponentInChildren<Text>()
                .text
                = "Son 'OFF'";
        } else {

            GameObject.Find("SoundController")
                .GetComponent<AudioSource>()
                .volume
                = 100;
            GameObject.Find("Btn Son")
                .GetComponent<Button>()
                .GetComponentInChildren<Text>()
                .text
                = "Son 'ON'";
        }
    }

    public void FullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        Debug.Log("Windowed");
    }

    public void Help() { Application.OpenURL("https://tinyurl.com/SlapDance"); }
}