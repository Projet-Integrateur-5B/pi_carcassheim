using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    bool gameInPause = false;
    private GameObject PauseMenuUI;
    private GameObject ParentPauseMenuUI;

    // void Start() {
    // }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    GameObject FindPauseMenu()
    {
        Transform[] trs = ParentPauseMenuUI.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.name == "Menu Pause")
            {
                return t.gameObject;
            }
        }
        return null;
    }
    public void setGameInPause(bool value)
    {
        gameInPause = value;
        ParentPauseMenuUI =  GameObject.Find("/Pause");
        PauseMenuUI = FindPauseMenu();
        Debug.Log("pause menu" + PauseMenuUI);
            if (gameInPause)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        
    }

    public bool getGameInPause()
    {
        return gameInPause;
    }

    public void Resume()
    {
        Debug.Log("Resume game");
        ParentPauseMenuUI =  GameObject.Find("/Pause");
        PauseMenuUI = FindPauseMenu();
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

    public void Pause()
    {
        Debug.Log("Menu en pause");
        ParentPauseMenuUI =  GameObject.Find("/Pause");
        PauseMenuUI = FindPauseMenu();
        gameInPause = true;
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }
}
