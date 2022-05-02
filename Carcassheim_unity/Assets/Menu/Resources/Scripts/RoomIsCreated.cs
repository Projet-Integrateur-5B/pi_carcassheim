using Assets.System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///    Room Is Created menu 
/// </summary>
public class RoomIsCreated : Miscellaneous
{
    /// <summary>
    ///    Go to Public Room menu 
    /// </summary>
    public void HideRoomIsCreated()
    {
    	HidePopUpOptions();
		ChangeMenu("RoomIsCreatedMenu", "PublicRoomMenu");
    }

    /// <summary>
    ///    Go to Public Room menu 
    /// </summary>
    public void ShowRoom()
    {
		HidePopUpOptions();
		ChangeMenu("RoomIsCreatedMenu", "PublicRoomMenu");
    }
}
