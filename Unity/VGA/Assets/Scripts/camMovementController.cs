using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Mathematics;
using UnityEngine;

public class camMovementController : MonoBehaviour
{
    Camera cam;
    [SerializeField]float min = 10;
    [SerializeField] float max = 30;
    [SerializeField] float touchZoomMultiplayer = 10f;

    Vector2[] lastKnownYPos;
    private void Start()
    {
        cam= Camera.main;
        lastKnownYPos = new Vector2[2]; // save the touch 0 and touch 1 
    }

    private void LateUpdate()
    {
        if (Input.mouseScrollDelta.y != 0) // for mouse zoom
            changeZoom(Input.mouseScrollDelta.y);


        else if (Input.touchCount > 1) // for touch zoom
        {
            float lastDis = math.abs(lastKnownYPos[0].y - lastKnownYPos[1].y) + math.abs(lastKnownYPos[0].x - lastKnownYPos[1].x);
            float crntDis = math.abs(Input.GetTouch(0).position.y - Input.GetTouch(1).position.y) + Mathf.Abs(Input.GetTouch(0).position.x - Input.GetTouch(1).position.x);
            if (lastDis != crntDis) // zoom out or zoom out
                changeZoom(touchZoomMultiplayer*(crntDis -lastDis)/Mathf.Min(Screen.currentResolution.width, Screen.currentResolution.height));
           
            lastKnownYPos[0] = Input.GetTouch(0).position; lastKnownYPos[1]= Input.GetTouch(1).position;
        }
            
    }
    void changeZoom(float delta)
    {
        //Debug.Log(delta+"");
        cam.orthographicSize=Mathf.Clamp(cam.orthographicSize-= delta, min, max);
    }

    void changePos(Vector3 delta)
    {
        cam.transform.position += delta;
    }

}


