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