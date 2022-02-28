using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RoomSelectionMenu : Miscellaneous
{
	public GameObject Pop_up_Options;
	public static bool IsOpenPanel = false;

	// Start is called before the first frame update
	void Start()
	{
		Pop_up_Options = FindMenu("Panel Options");
	}

	public void HideRoomSelection()
	{
		IsOpenPanel = false;
		Pop_up_Options.SetActive(IsOpenPanel);
		ChangeMenu(FindMenu("RoomSelectionMenu"), FindMenu("HomeMenu"));
	}

	public void ShowPopUpOptions()
	{
		IsOpenPanel = !IsOpenPanel;
		Pop_up_Options.SetActive(IsOpenPanel);
	}
}
