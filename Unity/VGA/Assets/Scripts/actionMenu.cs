using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class actionMenu : MonoBehaviour
{
    private node startingPoint;
    private node endingPoint;
    Coroutine currentAlgorithm;
    float delayTime = 1f;
    [SerializeField]bool isResume = true;
    byte selectingMode = 0; // 0= no selecting , 1= startpoints, 2=endpoint
    [SerializeField] Collider2D triggerObj;
    [SerializeField] GameObject algorithmMenu;
    [SerializeField] GameObject moreOptionMenu;
    private static actionMenu instance;

    List<node> notVisitedNodes= new List<node>();
    List<node> visitedNodes= new List<node>();
    Stack<List<node>[]> algorithmHistory = new Stack<List<node>[]>();
    Algorithm selectedAlgorithm = Algorithm.BFS;
    public enum Algorithm
    {
        BFS,
        DFS,
        GBFFS,
        AStar,
        Dijkastra
    }

    private void Start()
    {
        instance = this;
        currentAlgorithm = StartCoroutine(X());
    }

    public static bool isSelecting(){return instance.selectingMode > 0; }
    public static bool isStartingSelecting() { return instance.selectingMode == 1; }
    public static bool isEndingSelecting() { return instance.selectingMode == 2; }

    Coroutine selectingC;
    /// <summary>
    /// // 0= no selecting , 1= startpoints, 2=endpoint
    /// </summary>
    /// <param name="selectingMode"></param>
    public void setSelectingPointMode(int selectingMode) 
    {
        this.selectingMode = (byte)selectingMode;
    }

    public static void SetStartingPoint(node n) 
    {
        if (instance.startingPoint != null)
            instance.startingPoint.setColorMode(1);


        //selectingC = StartCoroutine(selecting());
        instance.clearAll();
        instance.notVisitedNodes.Add(instance.startingPoint = n);
    }

    public static void setEndingPoint(node n) 
    {
        if (instance.endingPoint != null)
            instance.endingPoint.setColorMode(1);
        instance.endingPoint = n;
    }
    public void startVisualization() 
    {
        clearAll();
    }

    public void pauseVisualization()
    {

    }

    public void stopVisualization()
    {

    }

    public void stepBack() 
    {
        pauseVisualization();
        // do somelogic here!
    }
    public void stepForward()
    {
        pauseVisualization();
        // do somelogic here!
    }

    public void showAlgorithmMenu() 
    {
        algorithmMenu.SetActive(!algorithmMenu.activeInHierarchy);
    }

    // options like delayBetweenSteps, save, load
    public void showMoreOptionMenu() 
    {
        moreOptionMenu.SetActive(!algorithmMenu.activeInHierarchy);
    }
    void clearAll() 
    {
        notVisitedNodes.Clear();
        visitedNodes.Clear();
        algorithmHistory.Clear();
    }

    /// <summary>
    /// 0= no selecting , 1= startpoints, 2=endpoint
    /// </summary>
    /// <param name="selectingMode"></param>
    /// <returns></returns>
    
    IEnumerator ISelecting(byte selectingMode) 
    {
        //selectingC = true;
        while (true)
        {
            yield return new WaitForEndOfFrame();
            // change triggerObj pos to the mouse pos;
            triggerObj.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) ) 
            {
                RaycastHit hit;
                Physics.Raycast(triggerObj.transform.position, Vector3.up, out hit);
                Debug.Log(hit);
            }
        }
    }

    

    IEnumerator X() 
    {
        while (true)
        {
            
            yield return new WaitForSecondsRealtime(delayTime);
            yield return new WaitUntil(() => isResume);
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.up, out hit);
            //Debug.Log(hit.barycentricCoordinate);
        }

    }
}
