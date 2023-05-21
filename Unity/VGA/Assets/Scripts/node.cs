using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class node : MonoBehaviour
{

    private int ID;
    [SerializeField]private Color color;
    List<node> neighbors;

    void Start()
    {
        neighbors = new List<node>();
    }

    public void setID(int N) {ID = N;}
    public int getID() { return ID;}
    public void setNeighbors(List<node> l) { neighbors = l; }
    public List<node> getNeighbors() { return neighbors; }
    public void addNeighbor(node n) { neighbors.Add(n); }
    public void removeNeighbor(node n) { try { neighbors.Remove(n); } catch { } }

    public void sePos(Vector3 v) { transform.position = v; }
    public Vector3 getPos() { return transform.position; }

    public void changeColor(Color color) { this.color = color; }



    private void OnMouseDrag()
    {
        if (editMenu.getMode() != editMenu.Mode.Node)
        { return; }
        gameObject.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
    }
}
