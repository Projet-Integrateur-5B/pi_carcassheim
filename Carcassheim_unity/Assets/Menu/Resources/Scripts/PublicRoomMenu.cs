using Assets.System;
using ClassLibrary;
using UnityEngine;
using UnityEngine.UI;

public class PublicRoomMenu : Miscellaneous
{
	private Color readyState;
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
		ColorUtility.TryParseHtmlString("#90EE90", out readyState);
		Text preparer = Miscellaneous.FindObject(absolute_parent, "preparation").GetComponent<Text>();
		//Application.OpenURL("https://tinyurl.com/Kakyoin-and-Polnareff");
		Packet packet = new Packet();
		packet.IdMessage = Tools.IdMessage.StartGame;
		packet.IdPlayer = Communication.Instance.idClient;
		packet.Data = new[] {Communication.Instance.idRoom.ToString()};
		preparer.color = readyState;
        preparer.text = "PRET A JOUER !";
		Communication.Instance.SetIsInRoom(1);
		Communication.Instance.SendAsync(packet);
	}

}