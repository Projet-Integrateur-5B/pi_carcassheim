using UnityEngine;
using UnityEngine.UI;

public class PublicRoomMenu : Miscellaneous
{
	void Start()
	{
		Transform tmpGO = GameObject.Find("ShowRoomParameters").transform;
		tmpGO.Find("WheelPlayer").GetComponent<UnityEngine.Video.VideoPlayer>().isLooping = true;
		Debug.Log(tmpGO);
	}

	public void HideRoom()
	{
		HidePopUpOptions();
		ChangeMenu("PublicRoomMenu", "RoomSelectionMenu");
	}

	public void ShowRoomParameters(){
		Application.OpenURL("https://tinyurl.com/SlapDance");
	}

	public void Ready(){
		Application.OpenURL("https://tinyurl.com/Kakyoin-and-Polnareff");
	}

}