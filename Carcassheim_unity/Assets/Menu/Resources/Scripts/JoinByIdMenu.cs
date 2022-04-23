using UnityEngine;
using UnityEngine.UI;
using Assets.System;
using ClassLibrary;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;

public class JoinByIdMenu : Miscellaneous
{
	private Transform idMenu, IMCI; 
	private InputField idCM;
	void Start()
	{
		idMenu = GameObject.Find("SubMenus").transform.Find("JoinByIdMenu").transform;
		IMCI = idMenu.Find("InputField").transform.Find("InputFieldEndEdit").transform;
		idCM = IMCI.GetChild(0).GetComponent<InputField>();
	}

	public void InputFieldEndEdit(InputField inp)
	{
		Debug.Log("Input submitted" + " : " + inp.text);
	}

	public void HideJoinById()
	{
		HidePopUpOptions();
		ChangeMenu("JoinByIdMenu", "RoomSelectionMenu");
		Communication.Instance.isInRoom = 0;
	}

	public void ShowJoinPublicRoom2()
	{
		HidePopUpOptions();
		bool a = string.Equals(RemoveLastSpace(idCM.text), "123");
		InputFieldEndEdit(idCM);
		if (a)
		{
		// a changer en fonction de l'id valide etc. :
		//ChangeMenu("JoinByIdMenu", "PublicRoomMenu");
		Communication.Instance.isInRoom = 1;
		}
	}
}