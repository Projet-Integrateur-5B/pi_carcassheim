using UnityEngine;
using UnityEngine.UI;
using Assets.System;
using System;
using ClassLibrary;
using System.Collections.Generic;
using System.Threading;

public class RoomSelectionMenu : Miscellaneous
{
	private Transform roomSelectMenu, panelRooms; 

	//il faut qu'on recoive le nombre de room

	private static int nombreRoom = 5;

	List<List<GameObject>> List_of_Rooms;

	public List<string> listAction;
	public Semaphore s_listAction;

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

		listAction = new List<string>();
		s_listAction = new Semaphore(1, 1);

		/* Commuication Async */
		Communication.Instance.StartListening(OnPacketReceived);
	}

	public void HideRoomSelection()
	{
		HidePopUpOptions();
		ChangeMenu("RoomSelectionMenu", "HomeMenu");
		/* Stop la reception dans cette class */
		Communication.Instance.StopListening(OnPacketReceived);
	}

	public void ShowJoinById()
	{
		HidePopUpOptions();
		ChangeMenu("RoomSelectionMenu", "JoinByIdMenu");
		/* Stop la reception dans cette class */
		Communication.Instance.StopListening(OnPacketReceived);
	}

	public void ShowJoinPublicRoom()
	{
		HidePopUpOptions();
		ChangeMenu("RoomSelectionMenu", "PublicRoomMenu");
		/* Stop la reception dans cette class */
		Communication.Instance.StopListening(OnPacketReceived);
	}

	public void ShowCreateRoom()
	{
		HidePopUpOptions();
		ChangeMenu("RoomSelectionMenu", "CreateRoomMenu");
		/* Stop la reception dans cette class */
		Communication.Instance.StopListening(OnPacketReceived);
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
		Packet packet = new Packet();
		packet.IdMessage = Tools.IdMessage.RoomList;
		packet.IdPlayer = Communication.Instance.idClient;
		packet.Data = Array.Empty<string>();

		Communication.Instance.SetRoom(0);
		Communication.Instance.SendAsync(packet);
	}

	public void OnPacketReceived(object sender, Packet packet)
	{
		if (packet.IdMessage == Tools.IdMessage.RoomList)
		{
			if (packet.Error == Tools.Errors.None)
			{
				s_listAction.WaitOne();
				listAction.AddRange(packet.Data);
				s_listAction.Release();
			}
		}
	}

	private void Update()
	{
		s_listAction.WaitOne();
		int taille = listAction.Count;
		s_listAction.Release();

		if (taille > 0)
		{
			// A modifier quand il y aura une liste
			int compteur = 0;

			s_listAction.WaitOne();
			for (int i = 2; i < taille; i += 5)
			{
				SetIDRoom(compteur, listAction[i]);
				SetHostsRoom(compteur, listAction[i + 1]);
				SetPlayersRoom(compteur, listAction[i + 2]);
				SetMaxPlayersRoom(compteur, listAction[i + 3]);
				SetEndgameRoom(compteur, listAction[i + 4]);
				compteur++;
			}

			listAction.Clear();
			s_listAction.Release();
		}
	}
}