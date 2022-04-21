using UnityEngine;
using UnityEngine.UI;

public class RoomSelectionMenu : Miscellaneous
{
	private Transform roomSelectMenu, panelRooms; 
	private Text text_ID, text_Hosts, text_Endgame, text_Players, text_MaxPlayers;

	void Start()
	{
		roomSelectMenu = GameObject.Find("SubMenus").transform.Find("RoomSelectionMenu").transform;
		panelRooms = roomSelectMenu.Find("Canvas").transform.Find("ListOfRoom").transform.Find("PanelRooms").transform;
		text_ID = panelRooms.Find("ID_Test").GetComponent<Text>();
		text_Hosts = panelRooms.Find("Hosts_Test").GetComponent<Text>();
		text_Endgame = panelRooms.Find("Endgame_Test").GetComponent<Text>();
		text_Players = panelRooms.Find("Players_Test").GetComponent<Text>();
		text_MaxPlayers = panelRooms.Find("Max players_Test").GetComponent<Text>();
	}

	public void HideRoomSelection()
	{
		HidePopUpOptions();
		ChangeMenu("RoomSelectionMenu", "HomeMenu");
	}

	public void ShowJoinById()
	{
		HidePopUpOptions();
		ChangeMenu("RoomSelectionMenu", "JoinByIdMenu");
	}

	public void ShowJoinPublicRoom()
	{
		HidePopUpOptions();
		ChangeMenu("RoomSelectionMenu", "PublicRoomMenu");	
	}

	public void ShowCreateRoom()
	{
		HidePopUpOptions();
		ChangeMenu("RoomSelectionMenu", "CreateRoomMenu");	
	}

	public string GetIDRoom()
    {
		return text_ID.text;
    }

	public string GetHostsRoom()
	{
		return text_Hosts.text;
	}

	public string GetEndgameRoom()
	{
		return text_Endgame.text;
	}

	public string GetPlayersRoom()
	{
		return text_Players.text;
	}

	public string GetMaxPlayersRoom()
	{
		return text_MaxPlayers.text;
	}


	public void SetIDRoom(string texte)
	{
		this.text_ID.text = texte;
	}

	public void SetHostsRoom(string texte)
	{
		this.text_Hosts.text = texte;
	}

	public void SetEndgameRoom(string texte)
	{
		this.text_Endgame.text = texte;
	}

	public void SetPlayersRoom(string texte)
	{
		this.text_Players.text = texte;
	}

	public void SetMaxPlayersRoom(string texte)
	{
		this.text_MaxPlayers.text = texte;
	}
}