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

	public void ShowConnection()
	{
		ChangeMenu("HomeMenu", "ConnectionMenu");
		Communication.Instance.isInRoom = 0;
	}

	public void ShowRoomSelection()
	{
		ChangeMenu("HomeMenu", "RoomSelectionMenu");
		var test = this.GetComponent<RoomSelectionMenu>();
		test.LoadRoomInfo();
		/* SceneManager.LoadScene("InGame"); */
		Communication.Instance.isInRoom = 0;
	}

	public void ShowOptions()
	{
		ChangeMenu("HomeMenu", "OptionsMenu");
		Communication.Instance.isInRoom = 0;
	}

	public void ShowStat()
	{
		ChangeMenu("HomeMenu", "StatMenu");
		Communication.Instance.isInRoom = 0;
	}

	public void QuitGame() // A LA FIN : quand tout fonctionnera : RemoveAllListeners(); (bouton -> "free")
	{

		//TODO il manque le changement de la view
		Communication.Instance.LancementDeconnexion();
		Application.Quit();
	}
}