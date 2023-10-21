using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class editMenu : MonoBehaviour
{
    [SerializeField] GameObject selectSprite;
    [SerializeField] static LineRenderer lineSprite;
    private static GameObject firstNodeOfEdge;
    private static GameObject secondNodeOfEdge;
    public enum Mode {
        Select,
        Node,
        Edge,
        Remove
    }

    static Mode currentMode;
    static HashSet<GameObject> nodes = new HashSet<GameObject>();
    static HashSet<GameObject> edges = new HashSet<GameObject>();
    static HashSet<GameObject> selectedNodes = new HashSet<GameObject>();
    static bool isOverNode = false;
    [SerializeField] ButtonController controller;
    [SerializeField] GameObject node;
    [SerializeField] GameObject edge;
    [SerializeField] Camera cam;
    [SerializeField] GameObject workSpace;
    private int latestChar = 65; // = A
    private int latestNodeID = 0; // = A
    private int latestEdgeID = 0; // = A
    public void Clear() { removeAllNodes(); removeAllEdges(); latestChar = 65;latestEdgeID = latestNodeID = 0; }
    public static void addNode(GameObject n) {
        if (n.transform.GetComponent<node>() == null)
            return;
        nodes.Add(n);
    }
    public static void removeNode(GameObject n) { 
        nodes.Remove(n);
    }

    public static void removeEdge(GameObject e) {
        edges.Remove(e);
    }
    public static void removeAllNodes() {
        for (int i = 0; i < nodes.Count; i++) {
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!  must remove the edges too
            Destroy(nodes.ElementAt(i).gameObject);
        }
        nodes.Clear();
    }

    public static void removeAllEdges()
    {
        for (int i = 0; i < edges.Count; i++)
        {
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!  must remove the edges too
            Destroy(edges.ElementAt(i).gameObject);
        }
        edges.Clear();
    }
    public static HashSet<GameObject> getAllNode() { return nodes; }
    public static node getNodeID(int id) {
        foreach (GameObject g in nodes) {
            if (g.GetComponent<node>().getID() == id)
                return g.GetComponent<node>();
        }
        return null;
    }
    public static void addSelectedNode(GameObject g) {
        if (g.transform.GetComponent<node>() == null)
            return;
        selectedNodes.Add(g);
        g.GetComponent<node>().setSelected(true);
    }
    public static void removeSelectedNode(GameObject n)
    {
        selectedNodes.Remove(n);
    }
    public static void removeAllSelectedNodes()
    {
        for (int i = 0; i < selectedNodes.Count; i++)
        {
            try { selectedNodes.ElementAt(i).GetComponent<node>().setSelected(false); } catch (Exception e) { }
        }
        selectedNodes.Clear();
    }

    public static Mode getMode() {
        return currentMode;
    }
    void Start()
    {
        selectMode();
        lineSprite = GameObject.Find("line").GetComponent<LineRenderer>();
        
    }
    private void selectRightButton()
    {
        controller.setSelected(controller.transform.GetChild((int)currentMode).GetComponent<Image>());
    }
    public void selectMode() {
        currentMode = Mode.Select;
        selectRightButton();
    }
    public void nodeMode()
    {
        currentMode = Mode.Node;
        selectRightButton();
    }

    public void edgeMode()
    {
        currentMode = Mode.Edge;
        selectRightButton();
    }
    
    public void removeMode()
    {
        currentMode = Mode.Remove;
        selectRightButton();
    }

   IEnumerator ISpawnNewNode() // to support touch adding
    {
        if (Input.touchCount == 0)
            spawnNewNode();
        else if (Input.touchCount == 1) { // if called by touch then make sure the user want to add new node not want to just moving the camera 
            for (int i = 0; i < 10; i++) { 
                yield return new WaitForSeconds(0.02f);
                if (Input.touchCount == 0) {
                    spawnNewNode();
                    break;
                }
            }
        
        }
        yield return null;

    }

    public int spawnNewNode(float x, float y) {
        GameObject g = Instantiate(node, new Vector3(x, y, 0), node.transform.rotation);
        g.transform.parent = workSpace.transform.GetChild(0);
        g.GetComponent<node>().setID(latestNodeID);
        g.GetComponent<node>().setName(((char)latestChar) + "");
        g.name = "Node ID:" + g.GetComponent<node>().getID();
        latestChar++;
        latestNodeID++;
        if (latestChar == 91) // skip 6 ASCII avoid random chars
            latestChar += 6;
        if (latestChar == 123) // after finsh lowwer case start again the upper case
            latestChar = 65;
        nodes.Add(g);
        return latestNodeID;
    }


    private void spawnNewNode() {
        Vector3 v = cam.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10f);
        spawnNewNode(v.x,v.y);
    }

    public static void setIsOverNode(bool state) { isOverNode = state;}
    public static bool getISOverNode() { return isOverNode; }

    public static void setFirstNodeOfEdge(GameObject g)
    {
        lineSprite.SetPosition(0, g.transform.position);
        firstNodeOfEdge = g;
    }
    public static void setSecondTempNodeOfEdge(GameObject g)
    {
        secondNodeOfEdge = g;
        lineSprite.SetPosition(1, g.transform.position);
    }

    public static void removeSecondTempNodeOfEdge(GameObject g)
    {
        if (secondNodeOfEdge == g)
            secondNodeOfEdge = null;
    }
    public void createNewEdge() {
        if (firstNodeOfEdge == secondNodeOfEdge || firstNodeOfEdge == null || secondNodeOfEdge == null)
            return;
        if (isSameEdges(firstNodeOfEdge, secondNodeOfEdge))
            return;
        GameObject g = Instantiate(edge, Vector3.zero, node.transform.rotation);
        g.transform.SetParent(workSpace.transform.GetChild(1));
        g.GetComponent<edge>().setNode(firstNodeOfEdge, secondNodeOfEdge);
        g.GetComponent<edge>().setID(latestEdgeID);
        g.name = "Edge ID:" + g.GetComponent<edge>().getID();
        latestEdgeID++;
        edges.Add(g);
        firstNodeOfEdge.GetComponent<node>().addEdge(g.GetComponent<edge>());
        secondNodeOfEdge.GetComponent<node>().addEdge(g.GetComponent<edge>());
        //firstNodeOfEdge = secondNode = null;
    }

    public bool isSameEdges(GameObject g1, GameObject g2) {
        GameObject[] gg = new GameObject[] {g1,g2}; 
        foreach (GameObject g in edges) {
            if (g.GetComponent<edge>().hasSameNodes(gg))
                return true;
        }
        return false;
    }
    public static void changePosSelectedNodes(GameObject g) {
        select.getObj().SetActive(false);
        if (selectedNodes.Count == 0) // change pos of node if its not selected (hold to change single node)
            g.GetComponent<node>().setPos(Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10));
        else{
            Vector3 basePos = g.transform.position;
            if (!selectedNodes.Contains(g))  // click at non selected node
                return;
            
                
            Vector3 difrence = Camera.main.ScreenToWorldPoint(Input.mousePosition) - basePos + new Vector3(0, 0, 10);
            foreach (GameObject gm in selectedNodes) { 
                gm.transform.position += difrence;
                gm.GetComponent<node>().updateEdgesPos();


            }
        }
    }


    public static HashSet<GameObject> getSelectedNodes() { return selectedNodes; }
    private void Update()
    {
        bool isOverGUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

        // disable the whole funcation in Visualization
        if (actionMenu.isVisualized) 
        {
            selectSprite.SetActive(false);
            return;
        }
            
        
        if (Input.GetMouseButton(0) && Input.touchCount == 0) // when left click
        {
            if (!isOverNode && !selectSprite.active && !isOverGUI) // deselect all nodes !!!!!!!!!!!!! need to check if hovered at node or not
            {
                removeAllSelectedNodes();
            }
            if (currentMode == Mode.Select)
            {
                selectSprite.SetActive(true);
            }


        }
        else if (!selectSprite.GetComponent<select>().isDisabling & Time.time > 0.1f)
        {
            //selectSprite.SetActive(false);
            selectSprite.GetComponent<select>().disableIt();

        }

        if (currentMode == Mode.Edge && Input.GetMouseButton(0)) // edge mode to draw preview edge
        {
            if (!isOverNode && firstNodeOfEdge != null) {
                lineSprite.SetPosition(1, cam.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10f));
                lineSprite.gameObject.SetActive(true);
            }

        }else {
            lineSprite.gameObject.SetActive(false);
            firstNodeOfEdge = null;
            secondNodeOfEdge = null;
        }

        if (currentMode == Mode.Node && Input.GetMouseButtonDown(0) && !isOverGUI && !isOverNode)
        { // add new node
            StartCoroutine(ISpawnNewNode());
        }

        // shortcut to change mode
        if (Input.GetKeyDown(KeyCode.S))
            selectMode();
        else if (Input.GetKeyDown(KeyCode.N))
            nodeMode();
        else if (Input.GetKeyDown(KeyCode.E))
            edgeMode();
        else if (Input.GetKeyDown(KeyCode.R))
            removeMode();
        if (Input.GetMouseButton(1) && !isOverGUI && !isOverNode) // open the right-click menu  (-Focus, -Random graph, -Clear, -ScreenShot, -)
        { 
            // open the menu
        }
    }




}
