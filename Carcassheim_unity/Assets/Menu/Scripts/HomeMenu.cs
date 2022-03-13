using UnityEngine;
using UnityEngine.UI;

public class HomeMenu : Miscellaneous
{
	void Start()
	{
		Color newCol;
		if (GetState() == false && FindMenu("HomeMenu").activeSelf == true)
		{
			GameObject.Find("ShowRoomSelection").GetComponent<Button>().interactable = GetState();
			GameObject.Find("ShowStat").GetComponent<Button>().interactable = GetState();
			ColorUtility.TryParseHtmlString("#808080", out newCol);
			GameObject.Find("ShowRoomSelection").GetComponent<Button>().GetComponentInChildren<Text>().color = newCol;
			GameObject.Find("ShowStat").GetComponent<Button>().GetComponentInChildren<Text>().color = newCol;
		}
	}

	void Update()
	{
	}

	public void ShowConnection()
	{
		ChangeMenu("HomeMenu", "ConnectionMenu");
	}

	public void ShowRoomSelection()
	{
		ChangeMenu("HomeMenu", "RoomSelectionMenu");
	//RandomIntColor(GameObject.Find("Etat de connexion"));
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

	public void QuitGame()
	{
		// A LA FIN : quand tout fonctionnera 
		// RemoveAllListeners(); (bouton -> "free")
		Application.Quit();
		Debug.Log("Quit!");
	}
}