using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.System;

public class RoomParameterRepre : MonoBehaviour
{
    [SerializeField] Slider time_tour_slider;
    [SerializeField] ToggleGroup room_access_toggle;
    [SerializeField] Toggle room_extension_rivier;
    [SerializeField] Toggle room_extension_abbaye;
    [SerializeField] Dropdown room_winmode_drop;


    [SerializeField] List<Selectable> menu_activate;

    // Start is called before the first frame update
    void Start()
    {
        bool interactif = Communication.Instance.idClient != RoomInfo.Instance.idModerateur;
        foreach (Selectable selectable in menu_activate)
        {
            selectable.interactable = interactif;
        }
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

    // Update is called once per frame
    void Update()
    {

    }
}
