using Assets.System;

public class JoinByIdMenu : Miscellaneous
{
	public void HideJoinById()
	{
		HidePopUpOptions();
		ChangeMenu("JoinByIdMenu", "RoomSelectionMenu");
		Communication.Instance.isInRoom = 0;
	}

	public void ShowJoinPublicRoom2()
	{
		HidePopUpOptions();
		ChangeMenu("JoinByIdMenu", "PublicRoomMenu");
		Communication.Instance.isInRoom = 1;
	}
}