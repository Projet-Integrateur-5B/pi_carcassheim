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
	}

	public void ShowRoomParameters(){
		//Application.OpenURL("https://tinyurl.com/SlapDance");
		HidePopUpOptions();
		ChangeMenu("PublicRoomMenu", "RoomParametersMenu");
	}

	public void Ready(){
		Application.OpenURL("https://tinyurl.com/Kakyoin-and-Polnareff");
	}

}