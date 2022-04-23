using Assets.System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomIsCreated : Miscellaneous
{
	public void HideRoomIsCreated()
    {
    	HidePopUpOptions();
		ChangeMenu("RoomIsCreatedMenu", "PublicRoomMenu");
    }

    public void ShowRoom()
    {
		HidePopUpOptions();
		ChangeMenu("RoomIsCreatedMenu", "PublicRoomMenu");
    }
}
