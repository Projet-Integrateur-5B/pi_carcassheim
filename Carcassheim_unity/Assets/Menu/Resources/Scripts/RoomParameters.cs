using Assets.System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomParameters : Miscellaneous
{
	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void HideRoomParameters()
	{
		HidePopUpOptions();
		ChangeMenu("RoomParametersMenu", "PublicRoomMenu");
		Communication.Instance.isInRoom = 1;
	}

	public void ToggleValueChangedRPM(Toggle curT)
	{
		if (curT.isOn)
			Debug.Log(curT.name);
	}
}