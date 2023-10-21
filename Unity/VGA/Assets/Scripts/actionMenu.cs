using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] GameObject algorithmMenu;
    [SerializeField] GameObject moreOptionMenu;
    [SerializeField] GameObject playPauseGroup;
    private static actionMenu instance;
    public static bool isVisualized;

    List<node> notVisitedNodes= new List<node>();
    List<node> visitedNodes= new List<node>();
    Stack<List<node>[]> algorithmHistory = new Stack<List<node>[]>();
    Algorithm selectedAlgorithm = Algorithm.BFS;

    int counter = 0;
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
    public void setAlgrothem(Button theButton) 
    {

        if (isVisualized) 
        {
            // notification.show("cant change the algo when the vis is apply");
            return;
        }
        
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
        if (startingPoint == null || endingPoint == null) 
        {
            closeAllControllButtons();
            return;
        }
        isResume = true;
        if (isVisualized)
        {
            return;
        }
        isVisualized = true;
        counter = 0;
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
        closeAllControllButtons();
        if (startingPoint == null || endingPoint == null)
        {
            
            return;
        }
        isResume = false;
        playPauseGroup.GetComponent<ButtonController>().setSelected(playPauseGroup.transform.GetChild(1).GetComponent<Image>());
        //playPauseGroup.transform.GetChild(1).GetComponent<Image>().enabled = false;
    }

    public void stopVisualization()
    {
        isVisualized = false;
        closeAllControllButtons();
        disableAllSelectedNodes();
    }

    void closeAllControllButtons() 
    {
        playPauseGroup.GetComponent<ButtonController>().setSelected(null);
    }

    public void stepBack() 
    {
        Debug.Log("SetpBack "  + visitedNodes.Count);
        pauseVisualization();
        // do somelogic here!
        temp();
        if (algorithmHistory.Count > 1)
        {
            algorithmHistory.Pop();
            visitedNodes = new List<node>(algorithmHistory.Peek()[0]);
            notVisitedNodes= new List<node>(algorithmHistory.Peek()[1]);
            //updateHistory();
        }

        Debug.Log("SetpBack2 " + visitedNodes.Count);
        temp();
        // something that update the new graph vis
        updateVis();
    }
    public void stepForward()
    {
        Debug.Log("SetpForward");
        pauseVisualization();
        if (visitedNodes.Contains(endingPoint) || notVisitedNodes.Count == 0)
            return;
        StartCoroutine(IStepForward());

    }

    IEnumerator IStepForward() 
    {
        
        float temp = delayTime;
        delayTime = 0.2f;
        
        //yield return new WaitForSecondsRealtime(0.11f);
        isResume = true;
        yield return new WaitForSecondsRealtime(0.21f);
        delayTime = temp;
        isResume = false;
    }
    public void showAlgorithmMenu() 
    {
        algorithmMenu.SetActive(!algorithmMenu.activeInHierarchy);
    }


    void temp() 
    {
        /*        string s = "";
                algorithmHistory.Last()[0].ForEach(str => s += string.Join(", ", str));
                string s2 = "";
                algorithmHistory.Last()[1].ForEach(str => s2 += string.Join(", ", str));
                Debug.Log($"visted: {s}    ,notVisted: {s2} ,algorithmHistory.count: {algorithmHistory.Count}");*/

        foreach (List<node>[] l in algorithmHistory) 
        {
            string s = "";
            l[0].ForEach(str => s += string.Join(", ", str));
            string s2 = "";
            l[1].ForEach(str => s2 += string.Join(", ", str));
            Debug.Log($"visted: {s}    ,notVisted: {s2} ,algorithmHistory.count: {algorithmHistory.Count}\t");
        }
            
    }
    // options like delayBetweenSteps, save, load
    public void showMoreOptionMenu() 
    {
        moreOptionMenu.SetActive(!algorithmMenu.activeInHierarchy);
    }
    void clearAll() 
    {
        if (currentAlgorithm != null)
            StopCoroutine(currentAlgorithm);
        currentAlgorithm = null;
        notVisitedNodes.Clear();
        visitedNodes.Clear();
        algorithmHistory.Clear();
    }


    void disableAllSelectedNodes() 
    {
        for (int i = 0; i < editMenu.getAllNode().Count; i++) 
        {
            node n = editMenu.getAllNode().ElementAt(i).GetComponent<node>();
            n.setSelected(false);
            n.fixTheColor();
        }
    }


    void updateVis() 
    {
        disableAllSelectedNodes();
        Debug.Log(visitedNodes.Count);
        counter = 0;
        foreach (node n in visitedNodes) 
        {
            
            n.setSelected(true);
            changeNodesColor(n, vistedColor);
            n.updateText(++counter + "");
        }

        foreach (node n in notVisitedNodes)
        {

            changeNodesColor(n, exploreColor);
        }

    }

    /// <summary>
    /// 0= no selecting , 1= startpoints, 2=endpoint
    /// </summary>
    /// <param name="selectingMode"></param>
    /// <returns></returns>
    /// 
    void runBFS() 
    {
        if (notVisitedNodes.Count == 0 && visitedNodes.Count == 0) 
        {
            notVisitedNodes.Add(startingPoint);
            //updateHistory();
        }
            
        currentAlgorithm = StartCoroutine(IRunBFS());
    }
    private IEnumerator IRunBFS()
    {
        //updateHistory();
        while (isVisualized) 
        {
            Debug.Log(visitedNodes.Count);

            yield return new WaitUntil(() => isResume);
            yield return new WaitForSecondsRealtime(delayTime);
            yield return new WaitUntil(() => isResume);
            notVisitedNodes[0].setSelected(true);
            changeNodesColor(notVisitedNodes[0], vistedColor);// change the color of visited node
            notVisitedNodes[0].updateText(++counter+"");

            foreach (node n in notVisitedNodes[0].getNeighbors()) // visit first node (apply queue approach)
            {
                if (!notVisitedNodes.Contains(n) && !visitedNodes.Contains(n))
                {
                    notVisitedNodes.Add(n); // add Neighbors of the node to last of notVisitedNodes list (apply queue approach)
                    changeNodesColor(n, exploreColor); // change the color of explorde node
                }
                    
            }
            visitedNodes.Add(notVisitedNodes[0]);
            notVisitedNodes.RemoveAt(0);
            updateHistory();
            // join the if block whne the Visualization finshed
            if (visitedNodes.Contains(endingPoint) || notVisitedNodes.Count == 0)
            {
                visitedNodes.Last().fixTheColor();
                Debug.Log("BFS finshed");
                //updateHistory();
                pauseVisualization();
                
            }
        }

      
        // make the path
    }

    private void runDFS()
    {
        if (notVisitedNodes.Count == 0 && visitedNodes.Count == 0)
        {
            notVisitedNodes.Add(startingPoint);
            //updateHistory();
        }

        currentAlgorithm = StartCoroutine(IRunDFS());
    }

    private IEnumerator IRunDFS()
    {
        //updateHistory();
        while (isVisualized)
        {
            Debug.Log(visitedNodes.Count);

            yield return new WaitUntil(() => isResume);
            yield return new WaitForSecondsRealtime(delayTime);
            yield return new WaitUntil(() => isResume);
            notVisitedNodes.Last().setSelected(true);
            changeNodesColor(notVisitedNodes.Last(), vistedColor);// change the color of visited node
            notVisitedNodes.Last().updateText(++counter + "");
            visitedNodes.Add(notVisitedNodes.Last());
            node temp = notVisitedNodes.Last();
            notVisitedNodes.RemoveAt(notVisitedNodes.Count-1);
            foreach (node n in temp.getNeighbors()) // visit first node (apply Stack approach)
            {
                if (!notVisitedNodes.Contains(n) && !visitedNodes.Contains(n))
                {
                    notVisitedNodes.Add(n); // add Neighbors of the node to last of notVisitedNodes list (apply queue approach)
                    changeNodesColor(n, exploreColor); // change the color of explorde node
                }

            }

            updateHistory();
            // join the if block whne the Visualization finshed
            if (visitedNodes.Contains(endingPoint) || notVisitedNodes.Count == 0)
            {
                visitedNodes.Last().fixTheColor();
                Debug.Log("BFS finshed");
                //updateHistory();
                pauseVisualization();
                
            }
        }


        // make the path
    }

    void changeNodesColor(node n, Color mode) 
    {
        if (n == endingPoint)
            return;
        n.changeColor(mode);
        n.changeFontColor(Color.white);
        
    }


    void updateHistory() 
    {
        /*string s = "";
        visitedNodes.ForEach(str => s += string.Join(", ", str));
        string s2 = "";
        notVisitedNodes.ForEach(str => s2 += string.Join(", ", str));
        Debug.Log($"visted: {s}    ,notVisted: {s2}");*/
        algorithmHistory.Push(new List<node>[] { new List<node>(visitedNodes), new List<node>(notVisitedNodes)});
        temp();
    }
}
