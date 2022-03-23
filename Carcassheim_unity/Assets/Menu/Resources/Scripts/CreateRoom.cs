using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRoom : Miscellaneous
{
	public void HideCreateRoom()
    {
    	HidePopUpOptions();
		ChangeMenu("CreateRoomMenu", "RoomSelectionMenu");
    }

    public void ShowRoomIsCreated()
    {
		HidePopUpOptions();
		ChangeMenu("CreateRoomMenu", "RoomIsCreatedMenu");	
    }
}
