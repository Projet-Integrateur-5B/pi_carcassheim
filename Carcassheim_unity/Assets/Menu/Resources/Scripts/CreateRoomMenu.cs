using Assets.System;
using ClassLibrary;
using system;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomMenu : Miscellaneous
{
	public List<bool> listAction;
	public Semaphore s_listAction;
	private void Start()
    {
		this.gameObject.AddComponent<Communication_inRoom>();

		listAction = new List<bool>();
		s_listAction = new Semaphore(1, 1);

		OnMenuChange += OnStart;
	}

	public void OnStart(string pageName)
	{
		switch (pageName)
		{
			case "CreateRoomMenu":
				/* Commuication Async */
				Communication.Instance.StartListening(OnPacketReceived);
				break;

			default:
				/* Ce n'est pas la bonne page */
				/* Stop la reception dans cette class */
				Communication.Instance.StopListening(OnPacketReceived);
				break;
		}
	}

	public void HideCreateRoom()
	{
		HidePopUpOptions();
		ChangeMenu("CreateRoomMenu", "RoomSelectionMenu");
	}

	public void ShowRoomIsCreated()
	{
		HidePopUpOptions();

		Packet packet = new Packet();
		packet.IdMessage = Tools.IdMessage.RoomCreate;
		packet.IdPlayer = Communication.Instance.idClient;
		packet.Data = Array.Empty<string>();

		Communication.Instance.SendAsync(packet);
	}

	public void ToggleValueChangedCRM(Toggle curT)
	{
		if (curT.isOn)
			Debug.Log(curT.name);
	}

	public void OnPacketReceived(object sender, Packet packet)
	{
		bool res = false;
		if (packet.IdMessage == Tools.IdMessage.RoomCreate)
		{
			if (packet.Error == Tools.Errors.None)
			{
				res = true;
				Communication.Instance.SetRoom(int.Parse(packet.Data[0]));
				Communication.Instance.SetPort(int.Parse(packet.Data[1]));
				Communication.Instance.SetIsInRoom(1);
			}

			s_listAction.WaitOne();
			listAction.Add(res);
			s_listAction.Release();
		}
	}

	private void Update()
	{
		s_listAction.WaitOne();
		int taille = listAction.Count;
		s_listAction.Release();

		bool res = false;
		if (taille > 0)
		{
			for (int i = 0; i < taille; i++)
			{
				s_listAction.WaitOne();
				res = (listAction[i]);
				s_listAction.Release();
			}

			if (res)
			{
				ChangeMenu("CreateRoomMenu", "RoomIsCreatedMenu");
			}

			s_listAction.WaitOne();
			listAction.Clear();
			s_listAction.Release();
		}
	}
}