using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class select : MonoBehaviour
{
    static GameObject obj;
    public bool isDisabling = true;
    void Start()
    {
        obj = GameObject.Find("select");
        isDisabling = true;
        obj.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "node")
            return;
        editMenu.addSelectedNode(collision.gameObject);
        collision.gameObject.GetComponent<node>().setSelected(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "node" || isDisabling)
            return;
        editMenu.removeSelectedNode(collision.gameObject);
        collision.gameObject.GetComponent<node>().setSelected(false);
    }


    Vector3 initPos = Vector3.zero;
    private void Update()
    {
            
        if (initPos == Vector3.zero)
            initPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = (Camera.main.ScreenToWorldPoint(Input.mousePosition) + initPos) / 2 + new Vector3(0, 0, 10);
        transform.localScale = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - initPos);


    }
    private void OnEnable()
    {
        initPos = Vector3.zero;
        isDisabling = false;
    }

    private void OnDisable()
    {
        transform.localScale = new Vector3 (0, 0, 0);
    }
    public static GameObject getObj() {
        return obj;    
    }
    public void disableIt() {
        isDisabling = true;
        transform.localScale = new Vector3(0, 0, 0);
        StartCoroutine(disableit2());
    }
    IEnumerator disableit2() { 
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
    
    }
}

