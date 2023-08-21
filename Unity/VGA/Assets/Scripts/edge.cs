using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class edge : MonoBehaviour
{
    private int ID;
    private LineRenderer line;
    private GameObject []nodes;
    private Vector3[] prevNodePos;
    void OnEnable()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.widthMultiplier = 0.3f;
        nodes = new GameObject[2];
        prevNodePos = new Vector3[2];
        updateLine();


    }
    private void Start()
    {
        updateLine();
    }
    public void setID(int N) { ID = N; }
    public int getID() { return ID; }
    //public void setName(string name) { this.name = text.text = name; }
    public string getName() { return name; }
    public void setNode(GameObject node1, GameObject node2) {
        nodes[0] = node1;
        nodes[1] = node2;
        nodes[0].GetComponent<node>().addNeighbor(nodes[1].GetComponent<node>());
        nodes[1].GetComponent<node>().addNeighbor(nodes[0].GetComponent<node>());
    }
    public GameObject[] getNodes() { return nodes; }
    public bool hasSameNodes(GameObject[] nodes2) {
        return (nodes.Contains(nodes2[0]) && nodes.Contains(nodes2[1]));
    }
    public void setNode1(GameObject node1) { nodes[0] = node1;}
    public void setNode2(GameObject node2) { nodes[1] = node2; }

    public void updateLine() {
        if (nodes[0] == null)
            return;
        if (prevNodePos[0] == nodes[0].transform.position && prevNodePos[1] == nodes[1].transform.position)
            return;
        if (nodes[0].transform.position != line.GetPosition(0))
            line.SetPosition(0, nodes[0].transform.position);
        if (nodes[1].transform.position != line.GetPosition(1))
            line.SetPosition(1, nodes[1].transform.position);

        GetComponent<EdgeCollider2D>().points = getEdgeColliderPos();

        prevNodePos[0] = nodes[0].transform.position;
        prevNodePos[1] = nodes[1].transform.position;

    }

    Vector2[] getEdgeColliderPos() 
    {
        Vector2[] v2 = new Vector2[2];
        Vector2 diff = new Vector2(line.GetPosition(0).x - line.GetPosition(1).x, line.GetPosition(0).y - line.GetPosition(1).y);
        Vector2 center = new Vector2((line.GetPosition(0).x + line.GetPosition(1).x) / 2f, (line.GetPosition(0).y + line.GetPosition(1).y) / 2f);
        v2[0] = new Vector2( center.x - (diff.x/3f), center.y - (diff.y / 3f));
        v2[1] = new Vector2(center.x + (diff.x /3f), center.y + (diff.y / 3f));
        return v2;
    }

    public void removeSecondNode(node n) {
        node node = n.gameObject == nodes[0] ? nodes[1].GetComponent<node>() : nodes[0].GetComponent<node>();
        node.removeEdge(this);
        node.removeNeighbor(n);
    }

    private void OnMouseOver()
    {
        if (editMenu.getISOverNode() || (!Input.GetMouseButtonDown(1) && Input.touchCount == 0))
            return;
        Debug.Log($"clicked at edge {name}");
    }
}
