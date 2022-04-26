using System;
using UnityEngine;
using UnityEngine.UI;
using Assets.System;

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
		ChangeMenu("HomeMenu", "HomeMenu"); // FAIT RIEN
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

		//TODO il manque le changement de la view
		Communication.Instance.LancementDeconnexion();
		Application.Quit();
	}
}