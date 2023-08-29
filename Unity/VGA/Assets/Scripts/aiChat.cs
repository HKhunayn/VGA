using System.Collections;
using System.Linq;
using System.Net;
using System.Threading;
using UnityEngine;

public class aiChat : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] editMenu editmenu;
    [TextAreaAttribute]
    [SerializeField] string s;
    [SerializeField] string aiInput = "what is this?";
    void Start()
    {
        //string s = "node add 0 0 0\r\nnode add 1 5 5\r\nnode add 2 10 0\r\nnode add 3 8 -5\r\nnode add 4 0 -5\r\nnode add 5 -5 0\r\nnode add 6 -10 0\r\nnode add 7 -5 5\r\nedge add 0 0 1\r\nedge add 1 1 2\r\nedge add 2 2 3\r\nedge add 3 3 4\r\nedge add 4 4 0\r\nedge add 5 5 6\r\nedge add 6 6 7\r\nedge add 7 7 0";
        StartCoroutine(commandHandeler(s));
        //StartCoroutine(AI("exp,text="+aiInput));
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator commandHandeler(string commands) {
        string[] strings = commands.Split('\n');
        foreach (string s in strings) {
            string[] ss = s.Split(' ');
            if (ss[0] == "node") {
                int n = ss.Count() - 1;
                editmenu.spawnNewNode(float.Parse(ss[2]), float.Parse(ss[3]));
            }
            else if(ss[0]== "edge"){
                int n = ss.Count()-1;
                editMenu.setFirstNodeOfEdge(editMenu.getNodeID(int.Parse(ss[1])).gameObject);
                editMenu.setSecondTempNodeOfEdge(editMenu.getNodeID(int.Parse(ss[2])).gameObject);
                editmenu.createNewEdge();
            }

            yield return new WaitForSeconds(0.5f);
        }
            
    }



    private IEnumerator AI(string input)
    {
        string output = null;
        Thread myThread = new Thread(delegate () {
            try {
                using (var client = new WebClient())
                {
                    output = client.DownloadString($"http://127.0.0.1:5000/{input}");
                }
            } catch (System.Exception e) { Debug.Log(e.Message); output= "something unexpected happend"; }
        });
        myThread.Start();
        yield return new WaitUntil(() => output != null);
        AIResponse(output);
    }

    void AIResponse(string response) { Debug.Log(response); StartCoroutine(commandHandeler(response)); }
}
