using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] PlateauRepre board;
    Camera mainCamera;
    [SerializeField] private float limitRay = 0.1f;
    [SerializeField] private float moveSpeed = 5.5f;

    [SerializeField] private float moveTreshold = 0.001f;

    private int last_scroll = 0;
    private int last_move = 0;
    [SerializeField] private float scrollSpeed = 0.7f;
    [SerializeField] private float Z_max = 0.46f;
    [SerializeField] private float Z_min = -1.6f;

    private Vector3 click_init_pos;
    private Vector3 lastMovePos;
    Vector3 dragOrigin;
    bool click_on = false, moving = false;


    void Awake()
    {
        mainCamera = Camera.main;
        scrollSpeed *= 30;
        moveSpeed *= 30;
        limitRay *= 1.15f;
    }


    private void OnEnable()
    {
        board.OnBoardExpanded += limitUpdate;
    }


    private void OnDisable()
    {
        board.OnBoardExpanded -= limitUpdate;
    }

    private float linearCurve(int x, float a = 0, float b = 100)
    {
        if (a + x > b)
            return 1f;
        return (a + x) / b;
    }

    private float discriminateSign(float x)
    {
        return (x > 0 ? x * 1.4f : x);
    }

    private float amplitude(float z)
    {
        float val = (z - Z_min) / (Z_max - Z_min);
        return val * val;
    }

    public bool cameraUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            click_on = true;
            click_init_pos = transform.position;
            lastMovePos = mainCamera.ScreenToViewportPoint(Input.mousePosition);
            return true;
        }
        else if (Input.GetMouseButtonUp(0) && click_on)
        {
            checkMove();
            click_on = false;
            bool lmoving = moving;
            moving = false;
            return lmoving;
        }
        else if (click_on)
        {
            checkMove();
            return true;
        }
        else if (Input.mouseScrollDelta.y != 0)
        {
            Vector3 nz = transform.position + new Vector3(0, 0, discriminateSign(Input.mouseScrollDelta.y) * Time.deltaTime * scrollSpeed * linearCurve(last_scroll, 100));
            last_scroll += 1;
            if (Z_min > nz.z)
                nz.z = Z_min;
            if (nz.z > Z_max)
                nz.z = Z_max;
            transform.position = nz;
            return true;
        }
        last_scroll = 0;
        last_move = 0;
        return false;
    }

    void checkMove()
    {
        Vector3 pos = mainCamera.ScreenToViewportPoint(Input.mousePosition);
        Vector3 vect = pos - lastMovePos;
        if (!moving)
        {
            if (vect.sqrMagnitude > moveTreshold)
            {
                moving = true;
            }
            else
            {
                return;
            }
        }
        last_move += 1;
        vect.z = 0;
        lastMovePos = pos;

        vect *= Time.deltaTime * moveSpeed * linearCurve(last_move, 80f);
        vect.x *= mainCamera.aspect;
        Vector3 npos = transform.position - vect;
        float factor_z = amplitude(transform.position.z) + 0.01f;
        npos.z = 0;
        if (npos.sqrMagnitude > limitRay * factor_z)
        {
            npos.Normalize();
            npos *= Mathf.Sqrt(limitRay * factor_z);
        }
        npos.z = transform.position.z;
        transform.position = npos;
    }


    void limitUpdate()
    {
        limitRay = board.BoardRadius;
    }

}
