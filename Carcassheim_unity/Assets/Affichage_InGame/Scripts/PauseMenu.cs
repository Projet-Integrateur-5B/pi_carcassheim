using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool gameInPause = false;

    [SerializeField] private GameObject PauseMenuUI;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameInPause)
            {
                Resume();
            }else {
                Pause();
            }
        }
    }

    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameInPause = false;
    }

    public void LoadOptions()
    {
        Debug.Log("Load Options");
        Time.timeScale = 1f;
    }

    public void Quit()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameInPause = true;
    }
}
