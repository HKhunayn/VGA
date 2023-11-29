using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class workspace : MonoBehaviour
{
    [SerializeField] GameObject leftClickMenu;
    [SerializeField] GameObject nodeOption;
    [SerializeField] GameObject edgeOption;
    [SerializeField] Camera cam;
    [SerializeField] Transform canvas;
    [SerializeField] ParticleSystem deleteEffect;
    editMenu em;
    private static workspace instance;
    private void Start()
    {
        instance = this;
        cam = Camera.main;
        em =GetComponent<editMenu>();
    }
    Vector3 rightClickMousePos;
    Coroutine activeitC;
    void Update()
    {
        if (actionMenu.isVisualized)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
            rightClickMousePos = Input.mousePosition;
        bool isOverGUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        if (Input.GetMouseButtonUp(1) && !isOverGUI && !editMenu.getISOverNode() && (Input.mousePosition == rightClickMousePos)) // open the right-click menu  (-Focus, -Random graph, -Clear, -ScreenShot, -)
        {
            activeitC = StartCoroutine(activeit());
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
        StopCoroutine(activeitC);
        yield return new WaitForSeconds(0.1f);
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
        Debug.Log("1");
        clear();
        Debug.Log("2");
        focus();
        Debug.Log("3");
        int n = Random.RandomRange(4,8);
        Debug.Log("4");
        List<int> Ids = new List<int>();
        List<Vector3> usedPos = new List<Vector3>();
        Debug.Log("5");
        for (int i = 0; i < n; i++) // to create nodes
        {
            Vector3 pos;
            pos = Vector3.zero;
            Ids.Add(em.spawnNewNode(pos.x, pos.y));

        }
        Debug.Log("6");
        int e = Random.RandomRange(n, n*(n-1)/2);

        for (int i = 0; i < n; i++) 
        {
            for (int j = i; j < n; j++)
            {
                int t = Random.Range(0, 1);
                if (t == 0)
                    break;
                GameObject g1 = editMenu.getNodeID(Ids[i]).gameObject;
                GameObject g2 = editMenu.getNodeID(Ids[j]).gameObject;
                editMenu.setFirstNodeOfEdge(g1);
                editMenu.setSecondTempNodeOfEdge(g2);
                em.createNewEdge();
            }
        }
        for (int i = 0; i < n; i++) // make each node has 2 edges
        {
            node node = editMenu.getNodeID(i);
            Dictionary<float, node> nearest = new Dictionary<float, node>();
            int count = 0;
            while (node.getNeighbors().Count < 2 && count < 50)
            {

                GameObject g1 = editMenu.getNodeID(Ids[Random.RandomRange(0, Ids.Count - 1)]).gameObject;
                while (node.hasNeighbor(g1.GetComponent<node>()))
                {
                    g1 = editMenu.getNodeID(Ids[Random.RandomRange(0, Ids.Count - 1)]).gameObject;
                }
                editMenu.setFirstNodeOfEdge(node.gameObject);
                editMenu.setSecondTempNodeOfEdge(g1);
                em.createNewEdge();
                count++;
            }
        }
        Debug.Log("7");
/*        for (int i = 0; i < e; i++)
        { // create a random edges for the remaining number of total edges
            GameObject g1 = editMenu.getNodeID(Ids[Random.RandomRange(0, Ids.Count - 1)]).gameObject;
            GameObject g2 = editMenu.getNodeID(Ids[Random.RandomRange(0, Ids.Count - 1)]).gameObject;
            while (g1.GetComponent<node>().hasNeighbor(g2.GetComponent<node>()) || g1==g2)
            {
                g1 = editMenu.getNodeID(Ids[Random.RandomRange(0, Ids.Count - 1)]).gameObject;
                g2 = editMenu.getNodeID(Ids[Random.RandomRange(0, Ids.Count - 1)]).gameObject;
            }
            editMenu.setFirstNodeOfEdge(g1);
            editMenu.setSecondTempNodeOfEdge(g2);
            em.createNewEdge();
        }
        Debug.Log($"n:{n}, e:{e}");
*/
    }

    public void clear() { em.Clear(); }
/*    public void Screenshot()
    {
        Debug.Log(DateTime.UtcNow.ToString("yyyy-MM-dd-mm:ss"));
        //ScreenCapture.CaptureScreenshot("");
    }*/



    node lastNode = null;

    public void updateText() {
        if (lastNode == null)
            return;
        nodeOption.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = $"Info:\n   Name: {lastNode.getName()}\n   ID: {lastNode.getID()}\n   Edges: {lastNode.getNeighbors().Count}";

    }
    public void openNodeOption(node n) {
        lastNode = n;
        updateText();
        updateNodeOption(n);
        nodeOption.SetActive(true);
    }


    public void closeNodeOption() 
    {
        nodeOption.SetActive(false);
    }

    public void updateNodeOption(node n) {
        lastNode = n;
        updateText();
        renameNode.setNode(n);
        nodeOption.transform.position = cam.WorldToScreenPoint(lastNode.transform.position) + (new Vector3(120 * canvas.localScale.x, -80 * canvas.localScale.y, 0));
    }
    public void updateNodeOption()
    {
        try { updateNodeOption(lastNode); } catch { }
        
    }


    public void centerTheLastNode() {

        if (lastNode.getNeighbors().Count < 2) // disable when its only has one neigbore (to avoid overlapping) or zero
        {
            closeNodeOption();
            return;
        }
            

        Vector3 v = Vector3.zero;
        foreach(node n in lastNode.getNeighbors())
            v += n.transform.position;
        lastNode.setPos(v/ lastNode.getNeighbors().Count);
        closeNodeOption();
    }


    public void deleteLastNode() {

        editMenu.removeNode(lastNode.gameObject);
        // remove all the edges of the node
        foreach (edge e in lastNode.GetEdges()) {
            editMenu.removeEdge(e.gameObject);
            e.removeSecondNode(lastNode);
            Destroy(e.gameObject);
        }
        editMenu.removeFromTheUsedList((int)(lastNode.getName()[0]));
        deleteEffect.transform.position = lastNode.transform.position;
        deleteEffect.Play();
        Destroy(lastNode.gameObject);
        lastNode = null;
        closeNodeOption();
    }


    edge lastEdge = null;
    public void updateEdgeText()
    {
        if (lastEdge == null)
            return;
        edgeOption.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = $"Info:\n   Node1: {lastEdge.getNodes()[0].GetComponent<node>().getName()}\n   Node2: {lastEdge.getNodes()[1].GetComponent<node>().getName()}";

    }

    public void updateEdgeOption(edge e)
    {
        lastEdge = e;
        updateEdgeText();
        Vector3 v = Vector3.zero;
        foreach(GameObject n in lastEdge.getNodes())
            v+= n.transform.position;
        v /= 2f;
        edgeOption.transform.position = cam.WorldToScreenPoint(v) + (new Vector3(120 * canvas.localScale.x, -80 * canvas.localScale.y, 0));
    }
    public void updateEdgeOption()
    {
        try { updateEdgeOption(lastEdge); } catch { }

    }
    public static void openEdgeOption(edge e)
    {
        instance.lastEdge = e;
        instance.updateEdgeText();
        instance.updateEdgeOption(e);
        instance.edgeOption.SetActive(true);
    }
    public void deleteLastEdge()
    {

        editMenu.removeEdge(lastEdge.gameObject);
        // remove all the nodes of the edge
        lastEdge.getNodes()[0].GetComponent<node>().removeEdge(lastEdge);
        lastEdge.getNodes()[0].GetComponent<node>().removeNeighbor(lastEdge.getNodes()[1].GetComponent<node>());
        lastEdge.getNodes()[1].GetComponent<node>().removeEdge(lastEdge);
        lastEdge.getNodes()[1].GetComponent<node>().removeNeighbor(lastEdge.getNodes()[0].GetComponent<node>());

        Vector3 v = Vector3.zero;
        foreach (GameObject n in lastEdge.getNodes())
            v += n.transform.position;
        v /= 2f;


        deleteEffect.transform.position = v;
        deleteEffect.Play();
        Destroy(lastEdge.gameObject);
        lastEdge = null;
        closeEdgeOption();
    }

    public void closeEdgeOption()
    {
        edgeOption.SetActive(false);
    }
}
