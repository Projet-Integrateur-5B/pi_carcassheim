using Assets.System;
using ClassLibrary;
using UnityEngine;
using UnityEngine.UI;

public class PublicRoomMenu : Miscellaneous
{
	void Start()
	{
		
	}

	public void HideRoom()
	{
		Packet packet = new Packet();
		packet.IdMessage = Tools.IdMessage.PlayerLeave;
		packet.IdPlayer = Communication.Instance.idClient;
		packet.Data = new[] { Communication.Instance.idRoom.ToString() };

		Communication.Instance.SetIsInRoom(1);
		Communication.Instance.SendAsync(packet);

		HidePopUpOptions();
		ChangeMenu("PublicRoomMenu", "RoomSelectionMenu");
	}

	public void ShowRoomParameters(){
		//Application.OpenURL("https://tinyurl.com/SlapDance");
		HidePopUpOptions();
		ChangeMenu("PublicRoomMenu", "RoomParametersMenu");
	}

	public void Ready(){
		//Application.OpenURL("https://tinyurl.com/Kakyoin-and-Polnareff");
		Packet packet = new Packet();
		packet.IdMessage = Tools.IdMessage.StartGame;
		packet.IdPlayer = Communication.Instance.idClient;
		packet.Data = new[] {Communication.Instance.idRoom.ToString()};

		Communication.Instance.SetIsInRoom(1);
		Communication.Instance.SendAsync(packet);
	}

}