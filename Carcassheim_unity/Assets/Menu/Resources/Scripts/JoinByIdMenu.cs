using UnityEngine;
using UnityEngine.UI;
using Assets.System;
using ClassLibrary;
using System.Collections.Generic;
using System.Threading;

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
		/* Commuication Async */
		Communication.Instance.StartListening(OnPacketReceived);

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
		packet.IdMessage = Tools.IdMessage.PlayerJoin;
		packet.IdPlayer = Communication.Instance.idClient;
		packet.Data = new string[] { RemoveLastSpace(idCM.text) };

		Communication.Instance.SendAsync(packet);
	}

	public void OnPacketReceived(object sender, Packet packet)
	{

		bool res = false;
		if (packet.IdMessage == Tools.IdMessage.PlayerJoin)
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