using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class camMovementController : MonoBehaviour
{
    Camera cam;
    [SerializeField] float XLimit = 256;
    [SerializeField] float YLimit = 256;
    [SerializeField]float minZoom = 10;
    [SerializeField] float maxZoom = 30;
    [SerializeField] float touchZoomMultiplayer = 100f;
    private Vector3 lastPos;
    Vector2[] lastKnownPos;
    [SerializeField] GameObject ggg;

    private void Start()
    {
        cam= Camera.main;
        lastKnownPos = new Vector2[2]; // save the touch 0 and touch 1
        changeZoom((minZoom + maxZoom / 2f));
        lastPos = getMouseWorldPos();
    }

    private void LateUpdate()
    {
        if (Input.mouseScrollDelta.y != 0) // for mouse zoom
            changeZoom(Input.mouseScrollDelta.y);


        else if (Input.touchCount > 1) // for touch zoom
        {
            float lastDis = math.abs(lastKnownPos[0].y - lastKnownPos[1].y) + math.abs(lastKnownPos[0].x - lastKnownPos[1].x);
            float crntDis = math.abs(Input.GetTouch(0).position.y - Input.GetTouch(1).position.y) + Mathf.Abs(Input.GetTouch(0).position.x - Input.GetTouch(1).position.x);
            if (lastDis != crntDis) // zoom out or zoom out
                changeZoom(touchZoomMultiplayer*(crntDis -lastDis)/Mathf.Min(Screen.currentResolution.width, Screen.currentResolution.height));
           
            lastKnownPos[0] = Input.GetTouch(0).position; lastKnownPos[1]= Input.GetTouch(1).position;
        }
    }
    void FixedUpdate() {

        if (Input.GetMouseButton(1)) // changing camera location for the mouse
            changeLoc();
        if (Input.touchCount == 1 && editMenu.getSelectedNodes().Count ==0) 
            changeLoc();
        lastPos = getMouseWorldPos();

    }
    void changeZoom(float delta)
    {
        cam.orthographicSize=Mathf.Clamp(cam.orthographicSize-= delta, minZoom, maxZoom);
    }

    private void changeLoc()
    {

        Vector3 v = cam.transform.position;
        // move the camera pos to mouse diricetion
        if (Input.touchCount == 0) // for mouse
            cam.transform.position += (lastPos - getMouseWorldPos());
        else // for toutch
            cam.transform.position -= new Vector3(Input.GetTouch(0).deltaPosition.x, Input.GetTouch(0).deltaPosition.y, 0)/100f;
        v = cam.transform.position;
        cam.transform.position = new Vector3(Mathf.Clamp(v.x, -XLimit, XLimit), Mathf.Clamp(v.y, -YLimit, YLimit), v.z);
    }

    private Vector3 getMouseWorldPos() {
        return cam.ScreenToWorldPoint(Input.touchCount == 0 ? Input.mousePosition : new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0.1f));
    
    }

}


