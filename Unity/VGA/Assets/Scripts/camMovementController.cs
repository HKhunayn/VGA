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
    Vector2 lastTouchPos;
    workspace workspace;
    private void Start()
    {
        cam= Camera.main;
        workspace = GameObject.Find("scripts").GetComponent<workspace>();
        lastKnownPos = new Vector2[2]; // save the touch 0 and touch 1
        cam.orthographicSize = (minZoom + maxZoom) / 2f;
        lastPos = getMouseWorldPos();
        lastTouchPos = Vector2.zero;
        
    }

    private void LateUpdate()
    {

        if (renameNode.isOpened()) // to disable this funcation when rename menu opened
            return;

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


        if (renameNode.isOpened()) // to disable this funcation when rename menu opened
            return;

        if (Input.GetMouseButton(1)) // changing camera location for the mouse
            changeLoc();
        if (Input.touchCount == 1 && !editMenu.getISOverNode() && editMenu.getMode() != editMenu.Mode.Edge) {
            Vector3 diff = new Vector3(Mathf.Abs(lastTouchPos.x - Input.GetTouch(0).position.x), Mathf.Abs(lastTouchPos.y - Input.GetTouch(0).position.y),0f);
            if (diff.x + diff.y > 100f) { // to fix postioning camera to starting point
                lastPos = getMouseWorldPos();
            }
                
            changeLoc();
            lastTouchPos= Input.GetTouch(0).position;
        }
            
        lastPos = getMouseWorldPos();

    }
    void changeZoom(float delta)
    {
        cam.orthographicSize=Mathf.Clamp(cam.orthographicSize-= delta, minZoom, maxZoom);
        workspace.updateNodeOption();
        workspace.updateEdgeOption();
    }
    
    private void changeLoc()
    {

        Vector3 v = cam.transform.position;
        // move the camera pos to mouse diricetion
        cam.transform.position += (lastPos - getMouseWorldPos());
        v = cam.transform.position;
        cam.transform.position = new Vector3(Mathf.Clamp(v.x, -XLimit, XLimit), Mathf.Clamp(v.y, -YLimit, YLimit), v.z);
        workspace.updateNodeOption();
        workspace.updateEdgeOption();
    }

    private Vector3 getMouseWorldPos() {
        return cam.ScreenToWorldPoint(Input.touchCount == 0 ? Input.mousePosition : new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 0.1f));
    
    }

}


