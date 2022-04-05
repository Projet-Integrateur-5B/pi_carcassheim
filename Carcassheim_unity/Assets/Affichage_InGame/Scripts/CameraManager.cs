using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Camera mainCamera;
    Vector3 mainCameraPosition;
    Quaternion mainCameraRotation;
    public float dragSpeed = .5f;
    Vector3 dragOrigin;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        mainCameraPosition = mainCamera.transform.position;
        mainCameraRotation = mainCamera.transform.rotation;
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
            upAndDownMotion(-2.5f);
        }

        if (Input.mouseScrollDelta.y < 0 || Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.Alpha6))
        {
            upAndDownMotion(2.5f);
        }
    }

    void cameraClickAndDrag() {
        // Check if mouse is on the table
        // exit in this case
        
        if (Input.GetMouseButtonDown(0)) {
            dragOrigin = Input.mousePosition;
        }

        if (!Input.GetMouseButton(0)) return;
        Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition-dragOrigin);
        mainCameraPosition.x -= pos.x * dragSpeed;
        mainCameraPosition.z -= pos.y * dragSpeed;

        mainCamera.transform.position = mainCameraPosition;
    }

    void upAndDownMotion(float target)
    {
        if (mainCamera.transform.position.z <= 0 && target < 0) {
            mainCameraPosition.z = 0;
            mainCamera.transform.position = mainCameraPosition;
            return;
        }
        if (mainCamera.transform.position.z >= 0.90f && target > 0) {
            return;
        }
        mainCameraPosition.z += target * Time.deltaTime;
        mainCamera.transform.position = mainCameraPosition;
    }

    void leftAndRightMotion(float target)
    {
        if ((mainCamera.transform.position.x <= -1 && target < 0)
            || (mainCamera.transform.position.x >= 1 && target > 0))
            return;
        mainCameraPosition.x += target * Time.deltaTime;
        mainCamera.transform.position = mainCameraPosition;
    }

}
