using UnityEngine;
using UnityEngine.UI;

public class RoomSelectionMenu : Miscellaneous
{
	public void HideRoomSelection()
	{
		HidePopUpOptions();
		ChangeMenu("RoomSelectionMenu", "HomeMenu");
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