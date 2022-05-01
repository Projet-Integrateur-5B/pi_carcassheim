using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.System;
using ClassLibrary;

public class RoomParameterRepre : MonoBehaviour
{
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


    // Start is called before the first frame update
    void Start()
    {
        RoomInfo room = RoomInfo.Instance;
        bool interactif = Communication.Instance.idClient != room.idModerateur;
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
    }

    public void OnRoomPolicyChange(bool state)
    {
        foreach (Toggle t in room_access_toggle.ActiveToggles())
            Debug.Log("" + t.name + " : " + t.spriteState);
    }

    public void OnRoomExtensionChangeRiviere(bool state)
    {
        Debug.Log("Riviere = " + state);
    }
    public void OnRoomExtensionChangeAbbaye(bool state)
    {
        Debug.Log("Abbaye = " + state);
    }

    public void OnRoomTimerTurnChange()
    {
        Debug.Log(time_tour_slider.value);
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
        setParameters(def_room_policy_public, Tools.Mode.Default, def_timer_tour, def_tile_winmode, def_point_winmode, def_tile_winmode, false, false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
