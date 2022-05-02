using Assets.System;
using ClassLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PublicRoomMenu : Miscellaneous
{
    private Color readyState;

    public List<string> listAction;
    public Semaphore s_listAction;


    private Transform container;
    private Text id_room; //id de la room (pour l'instant : 'X')

    void Start()
    {
        listAction = new List<string>();
        s_listAction = new Semaphore(1, 1);
        RoomInfo.Instance.idPartie = Communication.Instance.IdRoom;

        OnMenuChange += OnStart;

        container = GameObject.Find("SubMenus").transform.Find("PublicRoomMenu").transform.Find("Text").transform;
        id_room = container.Find("NumberOfRoom").GetComponent<Text>();
    }

    public void OnStart(string pageName)
    {
        switch (pageName)
        {
            case "PublicRoomMenu":
                /* Commuication Async */
                Communication.Instance.IsInRoom = 1;
                Communication.Instance.LancementConnexion();

                Action listening = () =>
                {
                    Communication.Instance.StartLoopListening(OnPacketReceived);
                };
                Task.Run(listening);


                /* Communication pour que le serveur set le port */
                Packet packet = new Packet();
                packet.IdMessage = Tools.IdMessage.PlayerJoin;
                packet.IdPlayer = Communication.Instance.IdClient;
                packet.IdRoom = Communication.Instance.IdRoom;
                packet.Data = Array.Empty<string>();

                Communication.Instance.SendAsync(packet);
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
        Debug.Log("HIDDING ROOM");
        Packet packet = new Packet();
        packet.IdMessage = Tools.IdMessage.PlayerLeave;
        packet.IdPlayer = Communication.Instance.IdClient;
        packet.IdRoom = Communication.Instance.IdRoom;
        packet.Data = Array.Empty<string>();

        Communication.Instance.IsInRoom = 1;
        Communication.Instance.SendAsync(packet);
        Communication.Instance.IsInRoom = 0;

        // HidePopUpOptions();
        Debug.Log("HALLO ?????");
        ChangeMenu("PublicRoomMenu", "RoomSelectionMenu");
    }

    public void ShowRoomParameters()
    {
        HidePopUpOptions();
        ChangeMenu("PublicRoomMenu", "RoomParametersMenu");
    }

    public void Ready()
    {
        ColorUtility.TryParseHtmlString("#90EE90", out readyState);
        Text preparer = Miscellaneous.FindObject(absolute_parent, "preparation").GetComponent<Text>();

        Packet packet = new Packet();
        packet.IdMessage = Tools.IdMessage.PlayerReady;
        packet.IdPlayer = Communication.Instance.IdClient;
        packet.IdRoom = Communication.Instance.IdRoom;
        packet.Data = Array.Empty<string>();

        preparer.color = readyState;
        if (OptionsMenu.langue == 0)
            preparer.text = "PRET A JOUER !"; 
        else if (OptionsMenu.langue == 1)
            preparer.text = "READY TO PLAY!";
        else if (OptionsMenu.langue == 2)
            preparer.text = "SPIELBEREIT!";

        Communication.Instance.IsInRoom = 1;
        Communication.Instance.SendAsync(packet);
    }

    public void OnPacketReceived(object sender, Packet packet)
    {
        if (packet.IdMessage == Tools.IdMessage.StartGame)
        {
            if (packet.Error == Tools.Errors.None)
            {
                RoomInfo.Instance.idTileInit = int.Parse(packet.Data[0]);
                s_listAction.WaitOne();
                listAction.Add("loadScene");
                s_listAction.Release();
            }
        }
        else if (packet.IdMessage == Tools.IdMessage.PlayerJoin)
        {
            Packet packet1 = new Packet();
            packet1.IdMessage = Tools.IdMessage.RoomSettingsGet;
            packet1.IdPlayer = Communication.Instance.IdClient;
            packet1.IdRoom = Communication.Instance.IdRoom;
            packet1.Data = Array.Empty<string>();

            Communication.Instance.SendAsync(packet1);
        }
        else if (packet.IdMessage == Tools.IdMessage.RoomSettingsGet)
        {
            if (packet.Error == Tools.Errors.None)
            {
                string[] res = new string[packet.Data.Length];
                Array.Copy(packet.Data, res, packet.Data.Length);
                RoomInfo.Instance.SetValues(packet.Data);
            }
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

        if (taille > 0)
        {
            s_listAction.WaitOne();
            string choixAction = listAction[0];
            s_listAction.Release();

            switch (choixAction)
            {
                case "loadScene":
                    StartCoroutine(LoadYourAsyncScene());
                    gameObject.SetActive(false);
                    break;
                case "playerReady":
                    /* Update l'affichage */
                    break;
            }

            s_listAction.WaitOne();
            listAction.Clear();
            s_listAction.Release();
        }
    }
}