using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class camMovementController : MonoBehaviour
{
    Camera cam;
    [SerializeField]float min = 5;
    [SerializeField] float max = 15;
    [SerializeField] float touchZoomMultiplayer = 0.5f;

    float[] lastKnownYPos;
    private void Start()
    {
        cam= Camera.main;
        lastKnownYPos = new float[2]; // save the touch 0 and touch 1 
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y != 0) // for mouse zoom
            changeZoom(Input.mouseScrollDelta.y);


        else if (Input.touchCount > 1) // for touch zoom
        {
            float lastDis = Mathf.Abs(lastKnownYPos[0] - lastKnownYPos[1]) ;
            float crntDis = Mathf.Abs(Input.GetTouch(0).position.y - Input.GetTouch(1).deltaPosition.y);
            if (lastDis != crntDis) // zoom out or zoom out
                changeZoom(touchZoomMultiplayer*Mathf.Clamp(lastDis - crntDis,-1,1));

           
            lastKnownYPos[0] = Input.GetTouch(0).position.y; lastKnownYPos[1]= Input.GetTouch(1).position.y;
        }
            
    }
    void changeZoom(float delta)
    {
        Debug.Log(delta+"");
        cam.orthographicSize=Mathf.Clamp(cam.orthographicSize-= delta, min, max);
    }

    void changePos(Vector3 delta)
    {
        cam.transform.position += delta;
    }

}


