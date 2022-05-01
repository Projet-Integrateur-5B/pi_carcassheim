using UnityEngine;
using UnityEngine.UI;
using Assets.System;
using System;
using ClassLibrary;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class RoomSelectionMenu : Miscellaneous
{
    private Transform roomSelectMenu, panelRooms;

    //il faut qu'on recoive le nombre de room

    private static int nombreRoom = 5;

    List<RoomLine> List_of_Rooms = new List<RoomLine>();

    public List<string> listAction;

    public List<bool> RoomChangeAction = new List<bool>();
    public Semaphore s_listAction;

    void Start()
    {
        roomSelectMenu = Miscellaneous.FindObject(absolute_parent, "RoomSelectionMenu").transform;
        panelRooms = Miscellaneous.FindObject(absolute_parent, "PanelRooms").transform;

        listAction = new List<string>();
        s_listAction = new Semaphore(1, 1);

        OnMenuChange += OnStart;
    }

    private void TableauRoom()
    {
        RoomLine model = FindObject(gameObject, "ROOMLINE").GetComponent<RoomLine>();
        List_of_Rooms.Clear();


        nombreRoom = 0;

        s_listAction.WaitOne();
        int taille = listAction.Count;
        s_listAction.Release();



        s_listAction.WaitOne();
        for (int i = 0; i < taille; i += 5)
        {
            List_of_Rooms.Add(CreateRoomLine(model, listAction[i], listAction[i + 1], listAction[i + 2], listAction[i + 3], listAction[i + 4]));
            nombreRoom++;
        }

        listAction.Clear();
        s_listAction.Release();
    }

    public void OnStart(string pageName)
    {
        switch (pageName)
        {
            case "RoomSelectionMenu":
                /* Commuication Async */
                Communication.Instance.StartListening(OnPacketReceived);
                LoadRoomInfo();
                break;

            default:
                /* Ce n'est pas la bonne page */
                /* Stop la reception dans cette class */
                Communication.Instance.StopListening(OnPacketReceived);
                break;
        }
    }

    public void HideRoomSelection()
    {
        HidePopUpOptions();
        ChangeMenu("RoomSelectionMenu", "HomeMenu");
    }

    public void ShowJoinById()
    {
        HidePopUpOptions();
        ChangeMenu("RoomSelectionMenu", "JoinByIdMenu");
    }

    public void ShowJoinPublicRoom()
    {
        HidePopUpOptions();
        ChangeMenu("RoomSelectionMenu", "PublicRoomMenu");
    }

    public void ShowCreateRoom()
    {
        HidePopUpOptions();
        ChangeMenu("RoomSelectionMenu", "CreateRoomMenu");
    }

    public RoomLine GetRoomLine(int index)
    {
        if (List_of_Rooms.Count <= index || index < 0)
            return null;
        return List_of_Rooms[index];
    }

    public void LoadRoomInfo()
    {
        Packet packet = new Packet();
        packet.IdMessage = Tools.IdMessage.RoomList;
        packet.IdPlayer = Communication.Instance.idClient;
        packet.Data = Array.Empty<string>();

        Communication.Instance.SetIsInRoom(0);
        Communication.Instance.SendAsync(packet);
    }

    public void OnPacketReceived(object sender, Packet packet)
    {
        if (packet.IdMessage == Tools.IdMessage.RoomList)
        {
            if (packet.Error == Tools.Errors.None)
            {
                s_listAction.WaitOne();
                listAction.AddRange(packet.Data);
                s_listAction.Release();
            }
        }
        else if (packet.IdMessage == Tools.IdMessage.RoomAskPort)
        {
            bool res = false;
            if (packet.Error == Tools.Errors.None)
            {
                res = true;
                Communication.Instance.SetPort(int.Parse(packet.Data[0]));
            }

            s_listAction.WaitOne();
            RoomChangeAction.Add(res);
            s_listAction.Release();
        }
    }

    void Update()
    {
        s_listAction.WaitOne();
        int taille = listAction.Count;
        s_listAction.Release();

        if ((taille > 0) && (taille % 5 == 0))
        {
            TableauRoom();
        }


        s_listAction.WaitOne();
        taille = RoomChangeAction.Count;
        s_listAction.Release();

        bool res = false;
        if (taille > 0)
        {
            for (int i = 0; i < taille; i++)
            {
                s_listAction.WaitOne();
                res = (RoomChangeAction[i]);
                s_listAction.Release();
            }

            if (res)
            {
                ChangeMenu("RoomSelectionMenu", "PublicRoomMenu");
            }

            s_listAction.WaitOne();
            RoomChangeAction.Clear();
            s_listAction.Release();
        }
    }
}