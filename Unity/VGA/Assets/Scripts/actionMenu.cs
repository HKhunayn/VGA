using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class actionMenu : MonoBehaviour
{
    [SerializeField] Color vistedColor = Color.white;
    [SerializeField] Color exploreColor = Color.white;

    private node startingPoint;
    private node endingPoint;
    Coroutine currentAlgorithm;
    float delayTime = 1f;
    [SerializeField]bool isResume = true;
    byte selectingMode = 0; // 0= no selecting , 1= startpoints, 2=endpoint
    [SerializeField] Button[] startEndPointsButtons;
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
        int x = 35;
        Debug.Log(Mathf.Ceil(x));
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
        instance.makeStartEndPointsClickableAgain();
    }


    public static void setEndingPoint(node n) 
    {
        if (instance.endingPoint != null)
            instance.endingPoint.setColorMode(1);
        instance.endingPoint = n;
        instance.makeStartEndPointsClickableAgain();
    }
    private void makeStartEndPointsClickableAgain()
    {
        foreach (Button b in startEndPointsButtons) 
        {
            b.GetComponent<Image>().enabled = true;
        }
            
        setSelectingPointMode(0);
    }
    public void setAlgrothem(Button theButton) {
        
        
        for (int i = 1; i < theButton.transform.parent.childCount - 1; i++) 
        {
            theButton.transform.parent.GetChild(i).GetComponent<Button>().interactable = true;
            if (theButton == theButton.transform.parent.GetChild(i).GetComponent<Button>())
                selectedAlgorithm = (Algorithm)i-1;
        }
        theButton.interactable = false;
        //Debug.Log(selectedAlgorithm);
        theButton.transform.parent.gameObject.SetActive(false);
    }
    public void startVisualization() 
    {
        Debug.Log("Start vis");
        clearAll();
        switch (selectedAlgorithm)
            {
            case Algorithm.BFS:
                runBFS();
                break;
            case Algorithm.DFS:
                runDFS();
                break;
            default:
                NotificationSystem.ShowNotification("No algorithm selected");
                break;
        }
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
    /// 
    void runBFS() 
    {
        currentAlgorithm = StartCoroutine(IRunBFS());
    }
    private IEnumerator IRunBFS()
    {
        notVisitedNodes.Add(startingPoint);
        while (!visitedNodes.Contains(endingPoint) && notVisitedNodes.Count != 0) 
        {
            yield return new WaitForSecondsRealtime(delayTime);
            yield return new WaitUntil(() => isResume);
            notVisitedNodes[0].changeColor(vistedColor);
            string s = "";
            visitedNodes.ForEach(str => s+=string.Join(", ", str));
            string s2 = "";
            notVisitedNodes.ForEach(str => s2 += string.Join(", ", str));
            Debug.Log($"visted: {s}    ,notVisted: {s2}");
            foreach (node n in notVisitedNodes[0].getNeighbors()) // visit first node (apply queue approach)
            {
                if (!notVisitedNodes.Contains(n) && !visitedNodes.Contains(n))
                {
                    notVisitedNodes.Add(n); // add Neighbors of the node to last of notVisitedNodes list (apply queue approach)
                    n.changeColor(exploreColor); // change the color of explorde node
                }
                    
            }
            visitedNodes.Add(notVisitedNodes[0]);
            notVisitedNodes.RemoveAt(0);
        }
        Debug.Log("BFS finshed");
        // make the path
    }

    private void runDFS()
    {

    }
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
