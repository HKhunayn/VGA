using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class workspace : MonoBehaviour
{
    [SerializeField] GameObject leftClickMenu;
    [SerializeField] GameObject nodeOption;
    [SerializeField] Camera cam;
    [SerializeField] Transform canvas;
    editMenu em;
    private void Start()
    {
        cam = Camera.main;
        em =GetComponent<editMenu>();
    }
    Vector3 rightClickMousePos;
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            rightClickMousePos = Input.mousePosition;
        bool isOverGUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        if (Input.GetMouseButtonUp(1) && !isOverGUI && !editMenu.getISOverNode() && (Input.mousePosition == rightClickMousePos)) // open the right-click menu  (-Focus, -Random graph, -Clear, -ScreenShot, -)
        {
            StartCoroutine(activeit());
        }
        else if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && leftClickMenu.active)// hide the menu when click
            StartCoroutine(disableLeftClickMenu());
    }

    IEnumerator activeit()
    {
        Vector3 pos = Input.mousePosition;
        yield return new WaitForSeconds(0.05f);
        if (pos == Input.mousePosition) { // to make sure the user does not want to move the camera
            leftClickMenu.transform.position = Input.mousePosition + (new Vector3(100 * canvas.localScale.x, -10 * canvas.localScale.y, 0));
            leftClickMenu.SetActive(true);
        }
    }
    IEnumerator disableLeftClickMenu()
    {
        float time = 0.2f;
        if (Input.GetMouseButton(1))
            time = 0.5f;
        yield return new WaitForSecondsRealtime(time);
        leftClickMenu.SetActive(false);
    }
    public void focus() {
        HashSet<GameObject> nodes = editMenu.getAllNode();
        if (nodes.Count > 0)
        {
            Vector3 v = Vector3.zero;
            foreach (GameObject node in nodes) { v += node.transform.position; }
            cam.transform.position = v / nodes.Count + new Vector3(0, 0, -10);

        }
        else {cam.transform.position = new Vector3(0, 0, -10);}

    }

    public void randomGraph() {
        clear();
        focus();
        int n = UnityEngine.Random.RandomRange(3,7);
        List<int> Ids = new List<int>();
        List<Vector3> usedPos = new List<Vector3>();
        for (int i = 0; i < n; i++) // to create nodes
        {
            Vector3 pos;
            pos = Vector3.zero;
            Ids.Add(em.spawnNewNode(pos.x, pos.y));

        }
        int e = UnityEngine.Random.RandomRange(n, n*(n-1)/2);
        for (int i = 0; i < n; i++) // make each node has 2 edges
        {
            node node = editMenu.getNodeID(i);
            Dictionary<float, node> nearest = new Dictionary<float, node>();

            while (node.getNeighbors().Count < 2)
            {
                
                GameObject g1 = editMenu.getNodeID(Ids[UnityEngine.Random.RandomRange(0, Ids.Count - 1)]).gameObject;
                while (node.hasNeighbor(g1.GetComponent<node>()))
                {
                    g1 = editMenu.getNodeID(Ids[UnityEngine.Random.RandomRange(0, Ids.Count - 1)]).gameObject;
                }
                editMenu.setFirstNodeOfEdge(node.gameObject);
                editMenu.setSecondTempNodeOfEdge(g1);
                em.createNewEdge();

            }
        }
        for (int i = 0; i < e-n; i++) { // create a random edges for the remaining number of total edges
            GameObject g1 = editMenu.getNodeID(Ids[UnityEngine.Random.RandomRange(0, Ids.Count - 1)]).gameObject;
            GameObject g2 = editMenu.getNodeID(Ids[UnityEngine.Random.RandomRange(0, Ids.Count - 1)]).gameObject;
            while (g1.GetComponent<node>().hasNeighbor(g2.GetComponent<node>())) {
                g1 = editMenu.getNodeID(Ids[UnityEngine.Random.RandomRange(0, Ids.Count - 1)]).gameObject;
                g2 = editMenu.getNodeID(Ids[UnityEngine.Random.RandomRange(0, Ids.Count - 1)]).gameObject;
            }
            editMenu.setFirstNodeOfEdge(g1);
            editMenu.setSecondTempNodeOfEdge(g2);
            em.createNewEdge();
        }


    }

    public void clear() { em.Clear(); }
    public void Screenshot() {
        Debug.Log(DateTime.UtcNow.ToString("yyyy-MM-dd-mm:ss"));
        //ScreenCapture.CaptureScreenshot("");
    }



    node lastNode = null;

    public void updateText() {
        nodeOption.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = $"Info:\n   Name: {lastNode.getName()}\n   ID: {lastNode.getID()}\n   Edges: {lastNode.getNeighbors().Count}";

    }
    public void openNodeOption(node n) {
        lastNode = n;
        updateText();
        updateEdgeOption(n);
        nodeOption.SetActive(true);
    }

    public void updateEdgeOption(node n) {
        lastNode = n;
        updateText();
        renameNode.setNode(n);
        nodeOption.transform.position = cam.WorldToScreenPoint(lastNode.transform.position) + (new Vector3(120 * canvas.localScale.x, -80 * canvas.localScale.y, 0));
    }
    public void updateEdgeOption()
    {
        try { updateEdgeOption(lastNode); } catch { }
        
    }
}
