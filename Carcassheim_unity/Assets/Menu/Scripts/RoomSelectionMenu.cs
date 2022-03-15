using UnityEngine;
using UnityEngine.UI;

public class RoomSelectionMenu : Miscellaneous
{
	public GameObject Pop_up_Options;
	public static bool s_isOpenPanel = false;
	// Start is called before the first frame update
	void Start()
	{
		Pop_up_Options = GameObject.Find("SubMenus").transform.Find("Panel Options").gameObject;
	}

	void Update()
	{
	}

	public void HideRoomSelection()
	{
		s_isOpenPanel = false;
		Pop_up_Options.SetActive(s_isOpenPanel);
		ChangeMenu("RoomSelectionMenu", "HomeMenu");
	}

	public void ShowPopUpOptions()
	{
		s_isOpenPanel = !s_isOpenPanel;
		Pop_up_Options.SetActive(s_isOpenPanel);
	}
}