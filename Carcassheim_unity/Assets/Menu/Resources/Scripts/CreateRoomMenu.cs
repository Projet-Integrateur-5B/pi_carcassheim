using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomMenu : Miscellaneous
{
    private void Start()
    {
        
    }

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

	public void ToggleValueChangedCRM(Toggle curT)
	{
		if (curT.isOn)
			Debug.Log(curT.name);
	}
}