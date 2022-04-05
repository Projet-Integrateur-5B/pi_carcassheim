using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Camera mainCamera;
    Vector3 mainCameraPosition = new Vector3(1, 8, -10);
    Quaternion mainCameraRotation = Quaternion.Euler(40, 0, 0);
    public float defaultFOV = 60;
    public float minFOV = 5;
    public float zoomMultiplier = 5;
    public float zoomDuration = 2;
    public float dragSpeed = .5f;
    Vector3 dragOrigin;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        mainCamera.transform.position = mainCameraPosition;
        mainCamera.transform.rotation = mainCameraRotation;
    }

    // Update is called once per frame
    void Update()
    {
        cameraClickAndDrag();
        cameraEventManager();
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
            if (mainCamera.fieldOfView > minFOV)
                smoothZoom(mainCamera.fieldOfView - 2.5f);

        }

        if (Input.mouseScrollDelta.y < 0 || Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.Alpha6))
        {
            if (mainCamera.fieldOfView < defaultFOV)
                smoothZoom(mainCamera.fieldOfView + 2.5f);
        }
    }

    void cameraClickAndDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
        }

        if (!Input.GetMouseButton(0)) return;
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        mainCameraPosition.x -= pos.x * dragSpeed;
        mainCameraPosition.z -= pos.y * dragSpeed;

        mainCamera.transform.position = mainCameraPosition;
    }

    void upAndDownMotion(float target)
    {
        mainCameraPosition.z += target * Time.deltaTime;
        mainCamera.transform.position = mainCameraPosition;
    }

    void leftAndRightMotion(float target)
    {
        mainCameraPosition.x += target * Time.deltaTime;
        mainCamera.transform.position = mainCameraPosition;
    }

    void smoothZoom(float target)
    {
        float angle = Mathf.Abs((defaultFOV / zoomMultiplier) - defaultFOV);
        mainCamera.fieldOfView = Mathf.MoveTowards(mainCamera.fieldOfView, target, angle / zoomDuration * Time.deltaTime);
    }
}
