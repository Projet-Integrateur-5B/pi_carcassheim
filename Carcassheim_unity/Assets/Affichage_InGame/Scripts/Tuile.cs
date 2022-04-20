using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tuile : MonoBehaviour
{

    // * LOOK *************************************************
    public Transform pivotPoint;
    public GameObject model;

    [SerializeField] private TMPro.TMP_Text _id_repre;
    [SerializeField] private List<SlotIndic> slots;
    [SerializeField] private Transform rep_O, rep_u, rep_v;

    Dictionary<int, SlotIndic> slots_mapping;

    // * STAT *************************************************
    private int _id = 0;

    public int Id
    {
        set { _id = value; _id_repre.text = value.ToString(); if (value < 0) _id_repre.gameObject.SetActive(false); }
        get { return _id; }
    }

    // * POSITION *********************************************
    public Position Pos { set; get; } = null; // null = not played
    public List<Position> possibilitiesPosition = new List<Position>();

    void Awake()
    {
        slots_mapping = new Dictionary<int, SlotIndic>();
        foreach (SlotIndic slot in slots)
        {
            Debug.Log(slot.Id);
            slots_mapping.Add(slot.Id, slot);
        }
    }

    public void showPossibilities(PlayerRepre player)
    {
        foreach (SlotIndic slot in slots)
        {
            slot.show(player);
        }
    }

    public void hidePossibilities()
    {
        foreach (SlotIndic slot in slots)
        {
            slot.hide();
        }
    }

    public void highlightFace(PlayerRepre player, int id)
    {
        slots_mapping[id].highlightFace(player);
    }

    public void unlightFace(int id)
    {
        slots_mapping[id].unlightFace();
    }

    public Position isPossible(Position pos)
    {
        foreach (Position true_pos in possibilitiesPosition)
        {
            if (true_pos.X == pos.X && true_pos.Y == pos.Y && (pos.Rotation == -1 || true_pos.Rotation == pos.Rotation))
                return true_pos;
        }
        return null;
    }

    public void nextRotation()
    {
        int x = Pos.X;
        int y = Pos.Y;
        int rotation = Pos.Rotation;
        bool found = false;
        for (int i = 0; i < 3; i++)
        {
            rotation = (rotation + 1) % 4;
            if (isPossible(new Position(x, y, rotation)) != null)
            {
                found = true;
                break;
            }
        }
        if (found)
        {
            Pos = new Position(x, y, rotation);
        }
    }

    public void addSlot(SlotIndic slot)
    {
        slot.transform.parent = pivotPoint;
        slot.front.transform.parent = pivotPoint;
        Vector3 whynot = new Vector3(0, 0.0772999972f, -0.00100000005f);
        slot.front.transform.localPosition = whynot;
        slot.transform.localPosition = rep_O.localPosition + (rep_u.localPosition - rep_O.localPosition) * slot.Xf + (rep_v.localPosition - rep_O.localPosition) * slot.Yf;
        slots.Add(slot);
    }
}