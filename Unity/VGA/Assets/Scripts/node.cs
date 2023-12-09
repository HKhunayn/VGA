using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR;

public class node : MonoBehaviour
{

    private int ID;
    private string name;
    [SerializeField]private Color color;
    [SerializeField] private Color selectTextColor = new Color(158/255f, 222/255f, 191/255f);
    [SerializeField] private Color defualtTextColor = new Color(28/255f, 36/255f, 44/255f);

    [SerializeField] private Color defualtNodeColor = Color.white;
    [SerializeField] private Color startNodeColor = new Color(158 / 255f, 222 / 255f, 191 / 255f);
    [SerializeField] private Color endNodeColor = new Color(222 / 255f, 158 / 255f, 162 / 255f);

    [SerializeField] TMP_Text popUpText;

    TMP_Text text;
    List<node> neighbors;
    List<edge> edges;
    GameObject selectedObject;
    editMenu editmenu;
    workspace workspace;


    void OnEnable()
    {
        neighbors = new List<node>();
        edges = new List<edge>();
        text = transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        selectedObject = transform.GetChild(1).gameObject;
        transform.position += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f),0);; // to avoid overlapping 
        editmenu = GameObject.Find("scripts").GetComponent<editMenu>();
        workspace = GameObject.Find("scripts").GetComponent<workspace>();
    }

    public void setID(int N) {ID = N;}
    public int getID() { return ID;}
    public void setName(string name) { this.name=text.text =name;}
    public string getName() { return name;}
    public void setNeighbors(List<node> l) { neighbors = l; }
    public List<node> getNeighbors() { return neighbors; }
    public void addNeighbor(node n) { neighbors.Add(n); }
    public void removeNeighbor(node n) { try { neighbors.Remove(n); } catch { } }
    public bool hasNeighbor(node n) { return neighbors.Contains(n); }
    public void addEdge(edge e) { edges.Add(e); }
    public void removeEdge(edge e) { try { edges.Remove(e); } catch { } }

    public List<edge> GetEdges() { return edges; }
    public void setPos(Vector3 v) { transform.position = v; updateEdgesPos(); }
    public Vector3 getPos() { return transform.position; }

    public void changeColor(Color color) { this.color = color; GetComponent<SpriteRenderer>().color = color; }
    public void changeFontColor(Color color) { text.color = color; }

    public void updateEdgesPos() { 
        foreach (edge e in edges)
            e.updateLine();
    }
    public void setSelected(bool state) {

        selectedObject.SetActive(state);
        text.color = state ? selectTextColor : defualtTextColor;
    }

    public bool isSelected() { return selectedObject.activeInHierarchy; }

    // 1=defualtColor, 2=startPointColor, 3=endPointColor
    byte colorMode = 1;

    public void setColorMode(byte mode) {  this.colorMode = mode; setNodeColor(mode); }

    /// <summary>
    /// 1=defualtColor, 2=startPointColor, 3=endPointColor
    /// </summary>
    /// <param name="colorCode"></param>
    public void setNodeColor(byte colorCode) 
    {
        text.color = colorCode != 1 ? Color.white: defualtTextColor;
        GetComponent<SpriteRenderer>().color = colorCode == 2?  startNodeColor : colorCode == 3? endNodeColor : defualtNodeColor;
        
    }
    public void fixTheColor()
    {
        setNodeColor(colorMode);
    }

    private void OnMouseOver()
    {



        if (renameNode.isOpened() || actionMenu.isSelecting() || actionMenu.isVisualized) // to disable this funcation when rename menu opened,isSelecting, or isVisualized
            return;
            

        if (Input.GetMouseButton(0))
            setSelected(true);
        else if (!editMenu.getSelectedNodes().Contains(gameObject))
            setSelected(false);

        if (Input.GetMouseButtonDown(1)) // if right click at the node
            workspace.openNodeOption(GetComponent<node>());
        else if (Input.GetMouseButton(1))
            workspace.updateNodeOption(GetComponent<node>());
    }
    private void OnMouseDrag()
    {

        if (renameNode.isOpened() || actionMenu.isSelecting() || actionMenu.isVisualized) // to disable this funcation when rename menu opened,isSelecting, or isVisualized
            return;

        if (editMenu.getMode() == editMenu.Mode.Select || editMenu.getMode() == editMenu.Mode.Node) // if curerent mode is select or node then move all selected nodes
        { 

            editMenu.changePosSelectedNodes(gameObject);

            if (Input.GetMouseButton(1) || Input.touchCount > 0 && Input.GetTouch(0).deltaPosition.x < 5f) // to appears the node option
                workspace.openNodeOption(this);
            else 
            {
                workspace.closeNodeOption();
                workspace.closeEdgeOption();
            }
                
        }
    }
    private void OnMouseEnter()
    {
        if (actionMenu.isVisualized) // to disable this funcation when isVisualized
            return;

        editMenu.setIsOverNode(true);

        if (editMenu.getMode() == editMenu.Mode.Edge)
        {
            editMenu.setSecondTempNodeOfEdge(gameObject);
        }

        // change the node color when its on setStart/setEnd points mode
        if (actionMenu.isStartingSelecting())
        {
            setNodeColor(2);
        }
        else if (actionMenu.isEndingSelecting())
        {
            setNodeColor(3);
        }
    }

    private void OnMouseExit()
    {
        if (actionMenu.isVisualized) // to disable this funcation when isVisualized
            return;


        editMenu.setIsOverNode(false);
        if (!editMenu.getSelectedNodes().Contains(gameObject)) // for disable selection mode for the current node
            setSelected(false);
        if (editMenu.getMode() == editMenu.Mode.Edge)
        {
            editMenu.removeSecondTempNodeOfEdge(gameObject);
        }

        // must check first
        setNodeColor(colorMode);
    }

    IEnumerator setFalse(bool f) { 
        yield return new WaitForSeconds(0.2f);
        f = false;
    }
    private void OnMouseDown()
    {

        if (actionMenu.isVisualized) // to disable this funcation when isVisualized
            return;

        // change the node color when its on setStart/setEnd points mode
        if (actionMenu.isStartingSelecting())
        {
            actionMenu.SetStartingPoint(this);
            colorMode = 2;
        }
        else if (actionMenu.isEndingSelecting())
        {
            actionMenu.setEndingPoint(this);
            colorMode = 3;
        }

        if (Input.touchCount == 1 || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
            if (!editMenu.getSelectedNodes().Contains(gameObject))
                editMenu.addSelectedNode(gameObject);
            else
                StartCoroutine(deSelectIfNotHolding());
        }

        if (editMenu.getMode() == editMenu.Mode.Edge) { // if the current mode is edge mode 
            editMenu.setFirstNodeOfEdge(gameObject);
        }
        else if (editMenu.getMode() == editMenu.Mode.Remove)
        { // if the current mode is remove mode 
            workspace.deleteNode(this);

        }

    }

    private void OnMouseUp() {

        if (actionMenu.isVisualized) // to disable this funcation when isVisualized
            return;

        if (editMenu.getMode() == editMenu.Mode.Edge) // when relese at second node to create new edge
        {
            editmenu.createNewEdge();
        }
    }

    IEnumerator deSelectIfNotHolding() { 
        yield return new WaitForSeconds(0.1f);
        if (Input.touchCount == 0)
            editMenu.removeSelectedNode(gameObject);

    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        updateEdgesPos();
    }

    public void updateText(string text) 
    { 
        popUpText.text = text;
    }

    public override string ToString() { return $"{name}"; }
}
