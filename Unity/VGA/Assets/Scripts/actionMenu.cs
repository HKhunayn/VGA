using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
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
    [SerializeField] editMenu editmenu;
    private static actionMenu instance;
    public static bool isVisualized;

    List<node> notVisitedNodes= new List<node>();
    List<node> visitedNodes= new List<node>();
    Stack<List<node>[]> algorithmHistory = new Stack<List<node>[]>();
    
    SortedDictionary<float, List<node>> paths = new SortedDictionary<float, List<node>>();
    Stack<SortedDictionary<float, List<node>>> algorithmHistory2 = new Stack<SortedDictionary<float, List<node>>>();

    Algorithm selectedAlgorithm;

    int counter = 0;
    public enum Algorithm
    {
        BFS,
        DFS,
        GBFS,
        AStar,
        notSelected
    }

    private void Start()
    {
        selectedAlgorithm = Algorithm.notSelected;
        instance = this;
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
            NotificationSystem.ShowNotification("Cant change the algo when the vis is apply");
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
            NotificationSystem.ShowNotification("Make sure you have start and end point!");
            return;
        }
        if (isVisualized)
        {
            if (isResume)
                NotificationSystem.ShowNotification("ALready Visualizing!");
            return;
        }
        isResume = true;

        if (selectedAlgorithm == Algorithm.notSelected) 
        {
            NotificationSystem.ShowNotification("No algorithm selected");
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
            case Algorithm.GBFS:
                runGBFS();
                break;
            case Algorithm.AStar:
                runASTAR();
                break;

            default:
                NotificationSystem.ShowNotification("No algorithm selected");
                break;
        }
    }

    bool isBlind() 
    {
        return (selectedAlgorithm == Algorithm.BFS || selectedAlgorithm == Algorithm.DFS);
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
        //Debug.Log("SetpBack "  + visitedNodes.Count);
        pauseVisualization();
        // do somelogic here!
        //temp();
        if (algorithmHistory.Count > 1)
        {
            algorithmHistory.Pop();
            visitedNodes = new List<node>(algorithmHistory.Peek()[0]);
            notVisitedNodes= new List<node>(algorithmHistory.Peek()[1]);

            if (!isBlind()) 
            
            {
                algorithmHistory2.Pop();
                paths= new SortedDictionary<float, List<node>>(algorithmHistory2.Peek());
            }
                
            //updateHistory();
        }

        Debug.Log("SetpBack2 " + visitedNodes.Count);
        //temp();
        // something that update the new graph vis
        updateVis();
    }
    public void stepForward()
    {
        //Debug.Log("SetpForward");
        pauseVisualization();
        if (visitedNodes.Contains(endingPoint) || (notVisitedNodes.Count == 0 && isBlind()  ))
            return;
        StartCoroutine(IStepForward());

    }

    IEnumerator IStepForward() 
    {
        
        float temp = delayTime;
        delayTime = 0.2f;
        
        //yield return new WaitForSecondsRealtime(0.11f);
        isResume = true;
        yield return new WaitForSecondsRealtime(0.201f);
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
            //Debug.Log($"visted: {s}    ,notVisted: {s2} ,algorithmHistory.count: {algorithmHistory.Count}\t");
        }
            
    }
    // options like delayBetweenSteps, save, load
    public void showMoreOptionMenu() 
    {
        moreOptionMenu.SetActive(!moreOptionMenu.activeInHierarchy);
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
            if (isBlind())
                n.updateText(++counter + "");
            else if (selectedAlgorithm == Algorithm.GBFS) 
            {
                string d = getDistance(n,endingPoint).ToString("F0");
                n.updateText("Heuristic:" + d);
            }
                
            else
                n.updateText("Cost:" + paths.ElementAt(0).Key.ToString("F0"));
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
                if (!visitedNodes.Contains(endingPoint))
                    NotificationSystem.ShowNotification("Cant reach to the endingPoint!");
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
                if (!visitedNodes.Contains(endingPoint))
                    NotificationSystem.ShowNotification("Cant reach to the endingPoint!");
                visitedNodes.Last().fixTheColor();
                Debug.Log("DFS finshed");
                //updateHistory();
                pauseVisualization();
                
            }
        }


        // make the path
    }




    private void runGBFS()
    {
        paths.Clear();
        if (paths.Count == 0 && visitedNodes.Count == 0)
        {
            paths.Add(getDistance(startingPoint,endingPoint), new List<node>{startingPoint});
            //updateHistory();
        }

        currentAlgorithm = StartCoroutine(IRunGBFS());
    }

    private IEnumerator IRunGBFS()
    {
        //updateHistory();
        while (isVisualized)
        {
            string s2 = "";
            foreach (var l in paths.Values)
            {
                foreach (node nnn in l)
                {
                    s2 += nnn+"->";
                }
                s2 +="\n";
            }


            Debug.Log(s2);

            yield return new WaitUntil(() => isResume);
            yield return new WaitForSecondsRealtime(delayTime);
            yield return new WaitUntil(() => isResume);
            List<node> currentPath = new List<node>(paths.ElementAt(0).Value);
            currentPath.Last().setSelected(true);
            changeNodesColor(currentPath.Last(), vistedColor);// change the color of visited node
            currentPath.Last().updateText("Heuristic:" + paths.ElementAt(0).Key.ToString("F0"));
            
            paths.Remove(paths.ElementAt(0).Key);
            if (visitedNodes.Contains(currentPath.Last()))
                continue;
            visitedNodes.Add(currentPath.Last());
            notVisitedNodes.Remove(currentPath.Last());
            foreach (node n in currentPath.Last().getNeighbors()) 
            {

                if (currentPath.Contains(n) || visitedNodes.Contains(n)) // if it in the current path dont add it again ( avoid the loop)
                    continue;

/*                if (n == endingPoint)
                {
                    visitedNodes.Add(n);
                    break;
                }*/
                    
                float heuristic = getDistance(n,endingPoint); // heuristic
                float cost = heuristic; // f(n) = h(n)
                cost += Random.Range(0.00001f,0.1f); // to make the key handel multuple paths with the same last node
                List<node> nodes = new List<node>(currentPath);
                nodes.Add(n);
                Debug.Log($"Cost={cost} , Value:{string.Join("->", nodes)}  Check?={currentPath.Contains(n)}");
                paths.Add(cost, new List<node>( nodes)); //add Neighbors of the node to last of paths Dic (apply queue approach)
                changeNodesColor(n, exploreColor); // change the color of explorde node
                notVisitedNodes.Add(n);
            }


            updateHistory();
            // join the if block whne the Visualization finshed
            if (visitedNodes.Contains(endingPoint) || paths.Count == 0)
            {
                if (!visitedNodes.Contains(endingPoint))
                    NotificationSystem.ShowNotification("Cant reach to the endingPoint!");

                visitedNodes.Last().fixTheColor();
                Debug.Log("GBFS finshed");
                //updateHistory();
                pauseVisualization();

            }
        }


        // make the path
    }



    private void runASTAR()
    {
        paths.Clear();
        if (paths.Count == 0 && visitedNodes.Count == 0)
        {
            paths.Add(getDistance(startingPoint, endingPoint), new List<node> { startingPoint });
            //updateHistory();
        }

        currentAlgorithm = StartCoroutine(IRunASTAR());
    }

    private IEnumerator IRunASTAR()
    {
        //updateHistory();
        while (isVisualized)
        {
            string s2 = "";
            foreach (var l in paths.Values)
            {
                foreach (node nnn in l)
                {
                    s2 += nnn + "->";
                }
                s2 += "\n";
            }


            Debug.Log(s2);

            yield return new WaitUntil(() => isResume);
            yield return new WaitForSecondsRealtime(delayTime);
            yield return new WaitUntil(() => isResume);
            List<node> currentPath = new List<node>(paths.ElementAt(0).Value);
            currentPath.Last().setSelected(true);
            changeNodesColor(currentPath.Last(), vistedColor);// change the color of visited node

            float realCost = 0;
            int i = 0;
            for (i = 0; i < currentPath.Count - 1; i++) 
            {
                realCost += editmenu.GetEdge(currentPath[i].gameObject, currentPath[i + 1].gameObject).getWeight();
                

            }
            Debug.Log("realCost reached " + i + " times");

            currentPath.Last().updateText("Cost:" + paths.ElementAt(0).Key.ToString("F0"));

            paths.Remove(paths.ElementAt(0).Key);
            if (visitedNodes.Contains(currentPath.Last()))
                continue;
            visitedNodes.Add(currentPath.Last());
            notVisitedNodes.Remove(currentPath.Last());
            foreach (node n in currentPath.Last().getNeighbors())
            {

                if (currentPath.Contains(n) || visitedNodes.Contains(n)) // if it in the current path dont add it again ( avoid the loop)
                    continue;

                /*                if (n == endingPoint)
                                {
                                    visitedNodes.Add(n);
                                    break;
                                }*/

                float heuristic = getDistance(n, endingPoint); // heuristic
                float cost = heuristic; // f(n) = h(n)
                cost += realCost;

                cost += Random.Range(0.00001f, 0.1f); // to make the key handel multuple paths with the same last node
                List<node> nodes = new List<node>(currentPath);
                nodes.Add(n);
                Debug.Log($"Cost={cost} , heuristic:{heuristic}  realCost={realCost}");
                paths.Add(cost, new List<node>(nodes)); //add Neighbors of the node to last of paths Dic (apply queue approach)
                changeNodesColor(n, exploreColor); // change the color of explorde node
                notVisitedNodes.Add(n);
            }


            updateHistory();
            // join the if block whne the Visualization finshed
            if (visitedNodes.Contains(endingPoint) || paths.Count == 0)
            {
                if (!visitedNodes.Contains(endingPoint))
                    NotificationSystem.ShowNotification("Cant reach to the endingPoint!");

                visitedNodes.Last().fixTheColor();
                Debug.Log("GBFS finshed");
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
        if (!isBlind())
            algorithmHistory2.Push(new SortedDictionary<float, List<node>>(paths));
        //temp();
        //updateEdgeColor();



    }

/*    void updateEdgeColor() 
    {
        if (selectedAlgorithm == Algorithm.GBFS || selectedAlgorithm == Algorithm.AStar)

        {
            for (int i = 0; i < paths.Values.ElementAt(0).Count - 1; i++)
            {

                GameObject[] nn = new GameObject[] { paths.Values.ElementAt(0)[i].gameObject, paths.Values.ElementAt(0)[i + 1].gameObject };
                setNodeColor(nn);
            }
        }
        else 
        {
            for (int i = 0; i < visitedNodes.Count - 1; i++)
            {

                GameObject[] nn = new GameObject[] { visitedNodes[i].gameObject, visitedNodes[i + 1].gameObject };
                setNodeColor(nn);
            }

        }

       
    
    }


    void setNodeColor(GameObject[] twoNodes = null) 
    {
        foreach (GameObject g in editMenu.getAllEdge())
        {
            if (twoNodes==null)
                g.GetComponent<edge>().setEdgeColor(1);
            else if  (g.GetComponent<edge>().hasSameNodes(twoNodes))
                g.GetComponent<edge>().setEdgeColor(2);

        }
    }*/

    public void saveGraph() 
    {
        StringBuilder sb = new StringBuilder();
        foreach (GameObject n in editMenu.getAllNode())
            sb.Append($"N {n.GetComponent<node>().getID()} {n.GetComponent<node>().getName()} {n.GetComponent<node>().getPos().x} {n.GetComponent<node>().getPos().y}\n");


        foreach (GameObject e in editMenu.getAllEdge())
            sb.Append($"E {e.GetComponent<edge>().getNodes()[0].GetComponent<node>().getID()} {e.GetComponent<edge>().getNodes()[1].GetComponent<node>().getID()}\n");
        WebCall.SaveGraph(sb.ToString());
    }

    public static void getGraph(string commands)
    {
        string[] strings = commands.Split('\n');
        foreach (string s in strings)
        {
            string[] ss = s.Split(' ');
            if (ss[0] == "N")
            {
                instance.editmenu.spawnNewNode(float.Parse(ss[3]), float.Parse(ss[4]), ss[2]);
            }
            else if (ss[0] == "E")
            {
                editMenu.setFirstNodeOfEdge(editMenu.getNodeID(int.Parse(ss[1])).gameObject);
                editMenu.setSecondTempNodeOfEdge(editMenu.getNodeID(int.Parse(ss[2])).gameObject);
                instance.editmenu.createNewEdge();
            }

        }

    }



    float getDistance(node n, node n2) 
    {
        return Mathf.Sqrt(Mathf.Pow(n.transform.position.x - n2.transform.position.x, 2f) + Mathf.Pow(n.transform.position.y - n2.transform.position.y, 2f));
    }
}
