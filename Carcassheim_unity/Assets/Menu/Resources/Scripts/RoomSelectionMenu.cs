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
			GameObject.Find("WheelPlayer").GetComponent<UnityEngine.Video.VideoPlayer>().Stop(); 
			GameObject.Find("WheelPlayer").GetComponent<UnityEngine.Video.VideoPlayer>().isLooping = false;
			}
		else {
			GameObject.Find("WheelPlayer").GetComponent<UnityEngine.Video.VideoPlayer>().Play();
			GameObject.Find("WheelPlayer").GetComponent<UnityEngine.Video.VideoPlayer>().isLooping = true;
		}
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

	public void ShowCreateRoom()
	{
		HidePopUpOptions();
		ChangeMenu("RoomSelectionMenu", "CreateRoomMenu");	
	}
}