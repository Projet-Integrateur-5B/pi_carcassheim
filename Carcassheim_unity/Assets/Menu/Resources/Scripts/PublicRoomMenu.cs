using Assets.System;
using UnityEngine;
using UnityEngine.UI;

public class PublicRoomMenu : Miscellaneous
{
	void Start()
	{
	}

	public void HideRoom()
	{
		HidePopUpOptions();
		ChangeMenu("PublicRoomMenu", "RoomSelectionMenu");
		Communication.Instance.isInRoom = 0;
	}

	public void ShowRoomParameters(){
		//Application.OpenURL("https://tinyurl.com/SlapDance");
		HidePopUpOptions();
		ChangeMenu("PublicRoomMenu", "RoomParametersMenu");
		Communication.Instance.isInRoom = 1;
	}

	public void Ready(){
		//Application.OpenURL("https://tinyurl.com/Kakyoin-and-Polnareff");
	}

}