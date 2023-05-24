using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class node : MonoBehaviour
{

    private int ID;
    private string name;
    [SerializeField]private Color color;
    [SerializeField] private Color selectTextColor = new Color(158/255f, 222/255f, 191/255f);
    [SerializeField] private Color defualtTextColor = new Color(28/255f, 36/255f, 44/255f);
    TMP_Text text;
    List<node> neighbors;
    GameObject selectedObject;

    void Start()
    {
        neighbors = new List<node>();
        text = transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        selectedObject = transform.GetChild(1).gameObject;
        transform.position += new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f),0);; // to avoid overlapping 
    }

    public void setID(int N) {ID = N;}
    public int getID() { return ID;}
    public void setName(string name) { this.name = name; text.text=name; }
    public string getName() { return name;}
    public void setNeighbors(List<node> l) { neighbors = l; }
    public List<node> getNeighbors() { return neighbors; }
    public void addNeighbor(node n) { neighbors.Add(n); }
    public void removeNeighbor(node n) { try { neighbors.Remove(n); } catch { } }

    public void setPos(Vector3 v) { transform.position = v; }
    public Vector3 getPos() { return transform.position; }

    public void changeColor(Color color) { this.color = color; }
    public void changeFontColor(Color color) { text.color = color; }

    public void setSelected(bool state) {

        selectedObject.SetActive(state);
        text.color = state ? selectTextColor : defualtTextColor;
    }
    private void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
            setSelected(true);
        else if (!editMenu.getSelectedNodes().Contains(gameObject))
            setSelected(false);
    }
    private void OnMouseDrag()
    {
        if (editMenu.getMode() == editMenu.Mode.Node)
        { gameObject.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10); }
        else if (editMenu.getMode() == editMenu.Mode.Select)
            editMenu.changePosSelectedNodes(gameObject);
    }
    private void OnMouseEnter()
    {
        editMenu.setIsOverNode(true);
    }

    private void OnMouseExit()
    {
        editMenu.setIsOverNode(false);
    }

    private void OnMouseDown()
    {
        if (Input.touchCount == 1) {
            if (!editMenu.getSelectedNodes().Contains(gameObject))
                editMenu.addSelectedNode(gameObject);
            else
                StartCoroutine(deSelectIfNotHolding());
        }
            
    }
    IEnumerator deSelectIfNotHolding() { 
        yield return new WaitForSeconds(0.1f);
        if (Input.touchCount == 0)
            editMenu.removeSelectedNode(gameObject);

    }

}
