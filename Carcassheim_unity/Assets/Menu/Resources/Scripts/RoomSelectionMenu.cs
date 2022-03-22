using UnityEngine;
using UnityEngine.UI;

public class RoomSelectionMenu : Miscellaneous
{
	public void HideRoomSelection()
	{
		HidePopUpOptions();
		ChangeMenu("RoomSelectionMenu", "HomeMenu");
	}

	public void ShowPopUpOptions()
	{
		// PATCH probleme de rotation (activation loop ok)
		if(GetPanelOpen()){
			print("rjlrjlr");	
			GameObject.Find("WheelPlayer").GetComponent<UnityEngine.Video.VideoPlayer>().isLooping = false; }
		else {GameObject.Find("WheelPlayer").GetComponent<UnityEngine.Video.VideoPlayer>().isLooping = true;
		print("dhfkfke");}
		SetPanelOpen(!GetPanelOpen());
		Pop_up_Options.SetActive(GetPanelOpen());
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
}