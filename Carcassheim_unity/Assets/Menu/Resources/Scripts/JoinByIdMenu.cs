public class JoinByIdMenu : Miscellaneous
{
	public void HideJoinById()
	{
		HidePopUpOptions();
		ChangeMenu("JoinByIdMenu", "RoomSelectionMenu");
	}

	public void ShowJoinPublicRoom2()
	{
		HidePopUpOptions();
		ChangeMenu("JoinByIdMenu", "PublicRoomMenu");	
	}
}