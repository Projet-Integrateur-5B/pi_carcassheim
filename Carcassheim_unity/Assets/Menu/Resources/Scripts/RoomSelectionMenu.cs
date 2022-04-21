using UnityEngine;
using UnityEngine.UI;
using Assets.System;
using System;

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

		LoadRoomInfo();
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

	private void LoadRoomInfo()
    {
		//text_ID,text_Hosts, text_Players, text_MaxPlayers, text_Endgame
		string[] values = Array.Empty<string>();
		Packet packet = Communication.Instance.CommunicationWithResult(Tools.IdMessage.RoomList, values);

		if (packet.Error != Tools.Errors.None)
			return;

		/* A modifier quand il y aura une liste*/
		//int taille_liste = res.Data.Length;
		int taille_liste = 1;

		for (int i = 0; i < taille_liste; i += 5)
        {
			text_ID.text = packet.Data[i];
			text_Hosts.text = packet.Data[i+1];
			text_Players.text = packet.Data[i+2];
			text_MaxPlayers.text = packet.Data[i+3];
			text_Endgame.text = packet.Data[i+4];
		}
	}
}