using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        currentMode = Mode.Select;
        selectMode();
    }

    public static void selectMode() {
        currentMode = Mode.Select;
    }

    public static void setIsOverNode(bool state) { isOverNode = state;}
    
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
        if (Input.GetMouseButton(0))
        {
            if (!isOverNode && !selectSprite.active) // deselect all nodes !!!!!!!!!!!!! need to check if hovered at node or not
            {
                removeAllSelectedNodes();
            }
            if (currentMode == Mode.Select && (Input.touchCount ==0 || Input.GetTouch(0).deltaTime > 1f))
            {
                selectSprite.SetActive(true);
            }

        }
        else {
            selectSprite.SetActive(false);

        }
    }




}
