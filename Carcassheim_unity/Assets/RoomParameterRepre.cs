using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.System;
using ClassLibrary;

public struct RoomParametersStruct
{
    public RoomParametersStruct(bool room_policy, Tools.Mode wm, int timer_tour, int timer_win, int point_win, int tile_win, bool river_on, bool abbaye_on)
    {
        this.wm = wm;
        this.timer_win = timer_win;
        this.tile_win = tile_win;
        this.point_win = point_win;
        this.room_policy = room_policy;
        this.timer_tour = timer_tour;
        this.river_on = river_on;
        this.abbaye_on = abbaye_on;
    }
    public bool room_policy;
    public Tools.Mode wm;
    public int timer_tour;
    public int timer_win;
    public int point_win;
    public int tile_win;
    public bool river_on;
    public bool abbaye_on;
}

public class RoomParameterRepre : MonoBehaviour
{
    [SerializeField] Text id_room;

    [SerializeField] Slider time_tour_slider;
    [SerializeField] ToggleGroup room_access_toggle;
    [SerializeField] Toggle room_extension_rivier;
    [SerializeField] Toggle room_extension_abbaye;
    [SerializeField] Toggle room_access_private;
    [SerializeField] Toggle room_access_public;
    [SerializeField] Dropdown room_winmode_drop;


    [SerializeField] List<Selectable> menu_activate;

    Tools.Mode winmode = Tools.Mode.Default;
    int timer_winmode = 20, def_timer_winmode = 20;
    int tile_winmode = 70, def_tile_winmode = 70;
    int point_winmode = 100, def_point_winmode = 100;

    bool room_policy_public = true, def_room_policy_public = true;
    bool riverOn = false, abbayeOn = false;

    int timer_tour = 60, def_timer_tour = 60;

    RoomParametersStruct change_planned;
    bool changed = false;



    // Start is called before the first frame update
    void Start()
    {
        RoomInfo room = RoomInfo.Instance;
        id_room.text = Communication.Instance.IdRoom.ToString();
        bool interactif = Communication.Instance.IdClient != room.idModerateur;
        foreach (Selectable selectable in menu_activate)
        {
            selectable.interactable = interactif;
        }
        room.repre_parameter = this;
        setParameters(room.isPrivate, room.mode, room.timerJoueur, room.timerPartie, room.scoreMax, room.idTileInit, room.riverOn, room.abbayeOn);
    }

    public void OnRoomWinChange(int index)
    {
        Debug.Log(room_winmode_drop.captionText);
        switch (index)
        {
            case 1:
                winmode = Tools.Mode.Time;
                break;
            case 2:
                winmode = Tools.Mode.Point;
                break;
            default:
                winmode = Tools.Mode.Default;
                break;
        }
        RoomInfo.Instance.mode = winmode;
    }

    public void OnRoomPolicyChange(bool state)
    {
        foreach (Toggle t in room_access_toggle.ActiveToggles())
            Debug.Log("" + t.name + " : " + t.spriteState);
    }

    public void OnRoomExtensionChangeRiviere(bool state)
    {
        Debug.Log("Riviere = " + state);
        RoomInfo.Instance.riverOn = state;
    }
    public void OnRoomExtensionChangeAbbaye(bool state)
    {
        Debug.Log("Abbaye = " + state);
        RoomInfo.Instance.abbayeOn = state;
    }

    public void OnRoomTimerTurnChange()
    {
        Debug.Log("timer tour" + time_tour_slider.value);
        RoomInfo.Instance.timerJoueur = (int)time_tour_slider.value;
    }

    public void addParameters(bool room_policy, Tools.Mode wm, int timer_tour, int timer_win, int point_win, int tile_win, bool river_on, bool abbaye_on)
    {
        changed = true;
        change_planned = new RoomParametersStruct(room_policy, wm, timer_tour, timer_win, point_win, tile_win, river_on, abbaye_on);
    }

    public void setParameters(RoomParametersStruct param)
    {
        changed = false;
        setParameters(param.room_policy, param.wm, param.timer_tour, param.timer_win, param.point_win, param.tile_win, param.river_on, param.abbaye_on);
    }

    public void setParameters(bool room_policy, Tools.Mode wm, int timer_tour, int timer_win, int point_win, int tile_win, bool river_on, bool abbaye_on)
    {
        winmode = wm;
        timer_winmode = timer_win;
        tile_winmode = tile_win;
        point_winmode = point_win;
        room_policy_public = room_policy;
        this.timer_tour = timer_tour;
        riverOn = river_on;
        abbayeOn = abbaye_on;

        switch (winmode)
        {
            case Tools.Mode.Time:
                room_winmode_drop.SetValueWithoutNotify(1);
                break;
            case Tools.Mode.Default:
                room_winmode_drop.SetValueWithoutNotify(0);
                break;
            case Tools.Mode.Point:
                room_winmode_drop.SetValueWithoutNotify(2);
                break;
        }
        time_tour_slider.SetValueWithoutNotify(timer_tour);

        room_access_public.SetIsOnWithoutNotify(!room_policy);
        room_access_private.SetIsOnWithoutNotify(room_policy);

        room_extension_abbaye.SetIsOnWithoutNotify(abbaye_on);
        room_extension_rivier.SetIsOnWithoutNotify(river_on);
    }

    void OnDisable()
    {
        changed = false;
        setParameters(def_room_policy_public, Tools.Mode.Default, def_timer_tour, def_tile_winmode, def_point_winmode, def_tile_winmode, false, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (changed)
            setParameters(change_planned);
    }
}
