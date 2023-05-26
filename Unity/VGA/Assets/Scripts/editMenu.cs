using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class editMenu : MonoBehaviour
{
    [SerializeField] GameObject selectSprite;
    public enum Mode {
        Select,
        Node,
        Edge,
        Remove
    }

    static Mode currentMode;
    static HashSet<GameObject> nodes = new HashSet<GameObject>();
    static HashSet<GameObject> selectedNodes = new HashSet<GameObject>();
    static bool isOverNode = false;
    [SerializeField] ButtonController controller;
    [SerializeField] GameObject node;
    [SerializeField] Camera cam;
    private int latestChar = 65; // = A
    private int latestID = 0; // = A
    public static void addNode(GameObject n) {
        if (n.transform.GetComponent<node>() == null)
            return;
        nodes.Add(n);
    }
    public static void removeNode(GameObject n) { 
        nodes.Remove(n);
    }
    public static void removeAllNodes() {
        for (int i = 0; i < nodes.Count; i++) {
            nodes.ElementAt(i).GetComponent<node>().setSelected(false);
        }
        nodes.Clear();
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
            selectedNodes.ElementAt(i).GetComponent<node>().setSelected(false);
        }
        selectedNodes.Clear();
    }

    public static Mode getMode() {
        return currentMode;
    }
    void Start()
    {
        selectMode();
        
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
    private void spawnNewNode() {

        GameObject g= Instantiate(node, cam.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10f), node.transform.rotation);
        g.GetComponent<node>().setID(latestID);
        g.GetComponent<node>().setName(((char)latestChar)+"");
        g.name = "Node ID:"+g.GetComponent<node>().getID();
        latestChar++;
        latestID++;
        if (latestChar == 91) // skip 6 ASCII avoid random chars
            latestChar += 6;
        if (latestChar == 123) // after finsh lowwer case start again the upper case
            latestChar = 65;
        nodes.Add(g);
    }

    public static void setIsOverNode(bool state) { isOverNode = state;}
    public static bool getISOverNode() { return isOverNode; }
    
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
            
            }
        }
    }


    public static HashSet<GameObject> getSelectedNodes() { return selectedNodes; }
    private void Update()
    {
        if (Input.GetMouseButton(0)  && Input.touchCount == 0) // when left click
        {
            if (!isOverNode && !selectSprite.active && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) // deselect all nodes !!!!!!!!!!!!! need to check if hovered at node or not
            {
                removeAllSelectedNodes();
            }
            if (currentMode == Mode.Select)
            {
                selectSprite.SetActive(true);
            }
            

        }
        else if (!selectSprite.GetComponent<select>().isDisabling & Time.time > 0.1f) {
            //selectSprite.SetActive(false);
            selectSprite.GetComponent<select>().disableIt();

        }


        if (currentMode == Mode.Node && Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        { // add new node
            StartCoroutine(ISpawnNewNode());
        }
    }




}
