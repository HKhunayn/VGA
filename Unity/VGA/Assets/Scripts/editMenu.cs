using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
    [SerializeField] GameObject selcet;
    public static void addNode(GameObject n) { 
        nodes.Add(n);
        Debug.Log($"node added: {n.name}  size:{nodes.Count}");
    }
    public static void removeNode(GameObject n) { 

        nodes.Remove(n);
        Debug.Log($"node removed: {n.name}  size:{nodes.Count}");
    }

    public static void tryRemoveNode(GameObject n)
    {

        nodes.Remove(n);
        Debug.Log($"node removed: {n.name}  size:{nodes.Count}");
    }

    IEnumerator try2RemoveNode(GameObject g) { 
        yield return new WaitForEndOfFrame();
        if (selcet.active)
            removeNode(g);
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




    Vector3 initPos = Vector3.zero;
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (currentMode == Mode.Select)
            {
                selectSprite.SetActive(true);
                if (initPos == Vector3.zero)
                    initPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                selectSprite.transform.position = (Camera.main.ScreenToWorldPoint(Input.mousePosition)+ initPos) /2 + new Vector3(0, 0, 10);
                selectSprite.transform.localScale = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - initPos);
            }

        }
        else {
            selectSprite.SetActive(false);
            initPos = Vector3.zero;
        }
    }




}
