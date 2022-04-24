using Assets.System;
using ClassLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PublicRoomMenu : Miscellaneous
{
	private Color readyState;

	public List<string> listAction;
	public Semaphore s_listAction;
	void Start()
	{
		listAction = new List<string>();
		s_listAction = new Semaphore(1, 1);

		OnMenuChange += OnStart;
	}

	public void OnStart(string pageName)
	{
		switch (pageName)
		{
			case "PublicRoomMenu":
				/* Commuication Async */
				Communication.Instance.StartListening(OnPacketReceived);
				break;

			default:
				/* Ce n'est pas la bonne page */
				/* Stop la reception dans cette class */
				Communication.Instance.StopListening(OnPacketReceived);
				break;
		}
	}

	public void HideRoom()
	{
		Packet packet = new Packet();
		packet.IdMessage = Tools.IdMessage.PlayerLeave;
		packet.IdPlayer = Communication.Instance.idClient;
		packet.Data = new[] { Communication.Instance.idRoom.ToString() };

		Communication.Instance.SetIsInRoom(1);
		Communication.Instance.SendAsync(packet);

		HidePopUpOptions();
		ChangeMenu("PublicRoomMenu", "RoomSelectionMenu");
	}

	public void ShowRoomParameters(){
		//Application.OpenURL("https://tinyurl.com/SlapDance");
		HidePopUpOptions();
		ChangeMenu("PublicRoomMenu", "RoomParametersMenu");
	}

	public void Ready(){
		ColorUtility.TryParseHtmlString("#90EE90", out readyState);
		Text preparer = Miscellaneous.FindObject(absolute_parent, "preparation").GetComponent<Text>();

		Packet packet = new Packet();
		packet.IdMessage = Tools.IdMessage.PlayerReady;
		packet.IdPlayer = Communication.Instance.idClient;
		packet.Data = new[] {Communication.Instance.idRoom.ToString()};
		preparer.color = readyState;
        preparer.text = "PRET A JOUER !";

		Communication.Instance.SetIsInRoom(1);
		Communication.Instance.SendAsync(packet);
	}

	public void OnPacketReceived(object sender, Packet packet)
    {
		if(packet.IdMessage == Tools.IdMessage.StartGame)
        {
			if(packet.Error == Tools.Errors.None)
            {
				s_listAction.WaitOne();
				listAction.Add("loadScene");
				s_listAction.Release();
			}
		}
        else if(packet.IdMessage == Tools.IdMessage.PlayerReady)
        {
			
		}

    }

	IEnumerator LoadYourAsyncScene()
	{
		// The Application loads the Scene in the background as the current Scene runs.
		// This is particularly good for creating loading screens.
		// You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
		// a sceneBuildIndex of 1 as shown in Build Settings.

		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("InGame_VG");

		// Wait until the asynchronous scene fully loads
		while (!asyncLoad.isDone)
		{
			yield return null;
		}
	}

    void Update()
    {
		s_listAction.WaitOne();
		int taille = listAction.Count;
		s_listAction.Release();

		if(taille > 0)
        {
			s_listAction.WaitOne();
			string choixAction = listAction[0];
			s_listAction.Release();

			switch (choixAction)
            {
				case "loadScene":
					StartCoroutine(LoadYourAsyncScene());
					break;
				case "playerReady":
					/* Update l'affichage */
					break;
            }
        }
	}
}