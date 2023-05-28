using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class edge : MonoBehaviour
{
    private int ID;
    private LineRenderer line;
    private GameObject []nodes;
    void OnEnable()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.widthMultiplier = 0.3f;
        nodes = new GameObject[2];
    }
    public void setID(int N) { ID = N; }
    public int getID() { return ID; }
    //public void setName(string name) { this.name = text.text = name; }
    public string getName() { return name; }
    public void setNode(GameObject node1, GameObject node2) {
        nodes[0] = node1;
        nodes[1] = node2;
    }
    public GameObject[] getNodes() { return nodes; }
    public bool hasSameNodes(GameObject[] nodes2) {
        return (nodes.Contains(nodes2[0]) && nodes.Contains(nodes2[1]));
    }
    public void setNode1(GameObject node1) { nodes[0] = node1; }
    public void setNode2(GameObject node2) { nodes[1] = node2; }

    private void FixedUpdate()
    {
        //Debug.Log("edges:"+ (nodes[0] == null) + ", " + (nodes[1] == null));
        if (nodes[0] == null)
            return;
        if (nodes[0].transform.position != line.GetPosition(0))
            line.SetPosition(0, nodes[0].transform.position);
        if (nodes[1].transform.position != line.GetPosition(1))
            line.SetPosition(1, nodes[1].transform.position);
    }
}
