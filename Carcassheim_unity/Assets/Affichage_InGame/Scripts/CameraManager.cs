using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] PlateauRepre board;
    Camera mainCamera;
    Vector3 mainCameraPosition;
    Quaternion mainCameraRotation;
    [SerializeField] private float dragSpeed;

    [SerializeField] private Table table;
    [SerializeField] private float limitRay;
    Vector3 dragOrigin;
    bool playerInput = false;
    bool mouseUp = false, mouseDown = false;


    private enum Axis : int
    {
        X,
        Y,
        Z
    }


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        mainCameraPosition = mainCamera.transform.position;
        mainCameraRotation = mainCamera.transform.rotation;
        limitRay = 5;
        dragSpeed = 5.5f;
    }

    private void OnEnable()
    {
        //board.OnBoardExpanded += limitUpdate;
    }


    private void OnDisable()
    {
        //board.OnBoardExpanded -= limitUpdate;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            playerInput = true;
            cameraClickAndDrag();
            cameraEventManager();
        }
    }

    void cameraEventManager()
    {
        if (Input.GetKey(KeyCode.UpArrow))
            upAndDownMotion(2.5f);
        if (Input.GetKey(KeyCode.DownArrow))
            upAndDownMotion(-2.5f);
        if (Input.GetKey(KeyCode.RightArrow))
            leftAndRightMotion(2.5f);
        if (Input.GetKey(KeyCode.LeftArrow))
            leftAndRightMotion(-2.5f);

        if (Input.mouseScrollDelta.y > 0 || Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.Equals))// || getDoubleClick())
        {
            zoomMotion(2.5f);
        }

        if (Input.mouseScrollDelta.y < 0 || Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.Alpha6))
        {
            zoomMotion(-2.5f);
        }

        if (Input.touchCount == 2)
        {
            Touch firstTouch = Input.GetTouch(0);
            Touch secondTouch = Input.GetTouch(1);

            Vector2 firstTouchPrevPos = firstTouch.position - firstTouch.deltaPosition;
            Vector2 secondTouchPrevPos = secondTouch.position - secondTouch.deltaPosition;

            float prevMagnitude = (firstTouchPrevPos - secondTouchPrevPos).magnitude;
            float curMagnitude = (firstTouch.position - secondTouch.position).magnitude;

            float diff = curMagnitude - prevMagnitude;

            zoomMotion(diff * 0.02f);
        }
    }


    bool cameraClickAndDrag()
    {
        Vector3 pointGap;
        float ray = .5f;
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            mouseDown = true;
        }


        if (!Input.GetMouseButton(0))
        {
            pointGap = Input.mousePosition - dragOrigin;
            mouseDown = false;
            return (Mathf.Pow(pointGap.x, 2)) + (Mathf.Pow(pointGap.y, 2)) + (Mathf.Pow(pointGap.z, 2)) >= Mathf.Pow(ray, 2);
        }
        if (mouseDown == true)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);

            if (reachLimit(mainCameraPosition.x - pos.x, (int)Axis.X)
                || reachLimit(mainCameraPosition.y - pos.y, (int)Axis.Y))
            {
                return false;
            }

            mainCameraPosition.x -= pos.x * dragSpeed * Time.deltaTime;
            mainCameraPosition.y -= pos.y * dragSpeed * Time.deltaTime;
            mainCamera.transform.position = mainCameraPosition;

            playerInput = false;
            //mouseDown = false;
        }
        return true;
    }

    void upAndDownMotion(float target)
    {

        if (reachLimit(target, (int)Axis.Y))
            return;
        mainCameraPosition.y += target * Time.deltaTime;
        mainCamera.transform.position = mainCameraPosition;

        playerInput = false;
    }

    void zoomMotion(float target)
    {
        // if (mainCamera.transform.position.z <= 0 && target < 0) {
        //     mainCameraPosition.z = 0;
        //     mainCamera.transform.position = mainCameraPosition;
        //     return;
        // }
        // if (mainCamera.transform.position.z >= 0.90f && target > 0) {
        //     return;
        // }

        if (reachLimit(target, (int)Axis.Z) || (mainCamera.transform.position.z <= 0 && target < 0))
            return;
        mainCameraPosition.z += target * Time.deltaTime;
        mainCamera.transform.position = mainCameraPosition;

        playerInput = false;
    }

    void leftAndRightMotion(float target)
    {
        // Limite du plateau
        if (reachLimit(target, (int)Axis.X))
            return;
        mainCameraPosition.x += target * Time.deltaTime;
        mainCamera.transform.position = mainCameraPosition;

        playerInput = false;
    }

    // Axis : 0->X; 1->Y; 2->Z
    float cameraRay(float value, int axis)
    {
        float mainX = mainCamera.transform.position.x;
        float mainY = mainCamera.transform.position.y;
        float mainZ = mainCamera.transform.position.z;

        mainX = (axis == (int)Axis.X) ? mainX + value : mainX;
        mainY = (axis == (int)Axis.Y) ? mainY + value : mainY;
        mainZ = (axis == (int)Axis.Z) ? mainZ + value : mainZ;

        return Mathf.Sqrt((Mathf.Pow(mainX, 2)) + (Mathf.Pow(mainY, 2)) + (Mathf.Pow(mainZ, 2)));
    }

    bool reachLimit(float target, int axis)
    {
        float camRay = cameraRay(target, axis);

        Debug.Log("limit ray : " + limitRay + " cam ray : " + camRay);

        if (((axis == (int)Axis.Y) && camRay >= limitRay-2) || ((axis == (int)Axis.Z) && camRay >= limitRay-1.75f) || camRay >= limitRay)
            return true;

        return false;

    }

    void limitUpdate()
    {
        limitRay = board.BoardRadius;
    }

}
