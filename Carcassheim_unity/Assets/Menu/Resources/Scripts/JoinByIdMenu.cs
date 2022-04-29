using UnityEngine;
using UnityEngine.UI;
using Assets.System;
using ClassLibrary;
using System.Collections.Generic;
using System.Threading;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class JoinByIdMenu : Miscellaneous
{
	private Transform idMenu, IMCI; 
	private InputField idCM;

	public List<bool> listAction;
	public Semaphore s_listAction;

	void Start()
	{
		idMenu = GameObject.Find("SubMenus").transform.Find("JoinByIdMenu").transform;
		IMCI = idMenu.Find("InputField").transform.Find("InputFieldEndEdit").transform;
		idCM = IMCI.GetChild(0).GetComponent<InputField>();

		listAction = new List<bool>();
		s_listAction = new Semaphore(1, 1);

		OnMenuChange += OnStart;
	}

	public void OnStart(string pageName)
	{
		switch (pageName)
		{
			case "JoinByIdMenu":
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

	public void InputFieldEndEdit(InputField inp)
	{
		Debug.Log("Input submitted" + " : " + inp.text);
	}

	public void HideJoinById()
	{
		HidePopUpOptions();
		ChangeMenu("JoinByIdMenu", "RoomSelectionMenu");
	}

	public void ShowJoinPublicRoom2()
	{
		HidePopUpOptions();
		InputFieldEndEdit(idCM);

		Packet packet = new Packet();
		packet.IdMessage = Tools.IdMessage.AskPort;
		packet.IdPlayer = Communication.Instance.idClient;
		packet.IdRoom = int.Parse(RemoveLastSpace(idCM.text));
		packet.Data = Array.Empty<string>();

		Communication.Instance.SetRoom(int.Parse(idCM.text));
		Communication.Instance.SetIsInRoom(0);
		Communication.Instance.SendAsync(packet);
	}

	public void OnPacketReceived(object sender, Packet packet)
	{

		bool res = false;
		if (packet.IdMessage == Tools.IdMessage.AskPort)
		{
			if (packet.Error == Tools.Errors.None)
			{
				res = true;
				Communication.Instance.SetPort(int.Parse(packet.Data[0]));
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
				ChangeMenu("JoinByIdMenu", "PublicRoomMenu");
			}

			s_listAction.WaitOne();
			listAction.Clear();
			s_listAction.Release();
		}
	}
}