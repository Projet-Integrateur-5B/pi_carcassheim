using UnityEngine;
using UnityEngine.UI;
using Assets.System;
using System;
using ClassLibrary;
using System.Collections.Generic;

public class RoomSelectionMenu : Miscellaneous
{
	private Transform roomSelectMenu, panelRooms; 

	//il faut qu'on recoive le nombre de room

	private static int nombreRoom = 5;

	List<List<GameObject>> List_of_Rooms;

	void Start()
	{
		roomSelectMenu = GameObject.Find("SubMenus").transform.Find("RoomSelectionMenu").transform;
		panelRooms = roomSelectMenu.Find("Canvas").transform.Find("ListOfRoom").transform.Find("PanelRooms").transform;

		List_of_Rooms = new List<List<GameObject>>();
		for(int i = 0; i < nombreRoom; i++)
        {
			List_of_Rooms.Add(new List<GameObject>() { new GameObject("ID_Test " + i), new GameObject("Hosts_Test " + i), new GameObject("Endgame_Test " + i), new GameObject("Players_Test " + i), new GameObject("Max players_Test " + i) });
		}

		int temp = 0;

		foreach (List<GameObject> room_list in List_of_Rooms)
		{
			foreach (GameObject item in room_list)
			{
				//Debug.Log(item.name);
				item.transform.parent = panelRooms;
				item.AddComponent<Text>();
				item.GetComponent<Text>().text = "" + temp;
				item.GetComponent<Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
				item.GetComponent<Text>().fontSize = 49;
				item.transform.localScale = new Vector3(1, 1, 1);
				item.GetComponent<Text>().alignment = TextAnchor.UpperCenter;
				item.GetComponent<Text>().color = Color.black;
				temp++;
			}
		}
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

	public string GetIDRoom(int index)
    {
		return this.List_of_Rooms[index][0].GetComponent<Text>().text;
    }

	public string GetHostsRoom(int index)
	{
		return this.List_of_Rooms[index][1].GetComponent<Text>().text;
	}

	public string GetEndgameRoom(int index)
	{
		return this.List_of_Rooms[index][2].GetComponent<Text>().text;
	}

	public string GetPlayersRoom(int index)
	{
		return this.List_of_Rooms[index][3].GetComponent<Text>().text;
	}

	public string GetMaxPlayersRoom(int index)
	{
		return this.List_of_Rooms[index][4].GetComponent<Text>().text;
	}


	public void SetIDRoom(int index, string texte)
	{
		this.List_of_Rooms[index][0].GetComponent<Text>().text = texte;
	}

	public void SetHostsRoom(int index, string texte)
	{
		this.List_of_Rooms[index][1].GetComponent<Text>().text = texte;
	}

	public void SetEndgameRoom(int index, string texte)
	{
		this.List_of_Rooms[index][2].GetComponent<Text>().text = texte;
	}

	public void SetPlayersRoom(int index, string texte)
	{
		this.List_of_Rooms[index][3].GetComponent<Text>().text = texte;
	}

    public void SetMaxPlayersRoom(int index, string texte) {
		this.List_of_Rooms[index][4].GetComponent<Text>().text = texte;
	}

	public void LoadRoomInfo()
    {
		//text_ID,text_Hosts, text_Players, text_MaxPlayers, text_Endgame
		string[] values = Array.Empty<string>();
		Packet packet = Communication.Instance.CommunicationWithResult(Tools.IdMessage.RoomList, values);

		if (packet.Error != Tools.Errors.None)
			return;

		// A modifier quand il y aura une liste
		int taille_liste = packet.Data.Length;
		int compteur = 0;
		for (int i = 2; i < taille_liste; i += 5)
        {
			SetIDRoom(compteur, packet.Data[i]);
			SetHostsRoom(compteur, packet.Data[i+1]);
			SetPlayersRoom(compteur, packet.Data[i+2]);
			SetMaxPlayersRoom(compteur, packet.Data[i+3]);
			SetEndgameRoom(compteur, packet.Data[i+4]);
			compteur++;
		}
	}
}