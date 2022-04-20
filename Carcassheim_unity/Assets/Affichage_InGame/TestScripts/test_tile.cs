using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_tile : MonoBehaviour
{
    public Tuile tile_model;
    public SlotIndic slot_model;
    public int id_read = 0;

    public Tuile act_tuile = null;
    public PlayerRepre act_player = null;
    private bool hidden = true;
    int PlateauLayer;

    // Start is called before the first frame update
    void Start()
    {
        act_player = new PlayerRepre();
        PlateauLayer = LayerMask.NameToLayer("Board");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            if (act_tuile != null)
                Destroy(act_tuile.gameObject);
            act_tuile = Resources.Load<Tuile>("tile" + id_read.ToString());
            act_tuile = Instantiate<Tuile>(act_tuile, transform);
        }
        else if (Input.GetKeyUp(KeyCode.A) && act_tuile != null)
        {
            if (hidden)
            {
                act_tuile.showPossibilities(act_player);
            }
            else
            {
                act_tuile.hidePossibilities();
            }
            hidden = !hidden;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, (1 << PlateauLayer)))
            {
                SlotIndic slot;
                Tuile tile;
                switch (hit.transform.tag)
                {
                    case "SlotCollider":
                        slot = hit.transform.parent.GetComponent<SlotIndic>();
                        if (hit.transform.parent.parent != act_tuile.pivotPoint)
                            Debug.Log("Wrong parent " + hit.transform.parent.parent.name + " instead of " + act_tuile.pivotPoint.name);
                        else
                        {
                            if (!slot.front.gameObject.activeSelf)
                                slot.highlightFace(act_player);
                            else
                                slot.unlightFace();
                        }
                        break;
                    case "TileBodyCollider":
                        if (hit.transform.parent != act_tuile.pivotPoint)
                            Debug.Log("Wrong parent " + hit.transform.parent.parent.name + " instead of " + act_tuile.pivotPoint.name);
                        else
                        {
                            tile = hit.transform.parent.parent.GetComponent<Tuile>();
                            tile.nextRotation();
                        }
                        break;
                    default:
                        Debug.Log("Hit " + hit.transform.tag + " collider");
                        break;
                }
            }
        }
    }
}
