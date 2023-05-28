using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chatGPT : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] editMenu editmenu;
    [TextAreaAttribute]
    [SerializeField] string s;
    void Start()
    {
        //string s = "node add 0 0 0\r\nnode add 1 5 5\r\nnode add 2 10 0\r\nnode add 3 8 -5\r\nnode add 4 0 -5\r\nnode add 5 -5 0\r\nnode add 6 -10 0\r\nnode add 7 -5 5\r\nedge add 0 0 1\r\nedge add 1 1 2\r\nedge add 2 2 3\r\nedge add 3 3 4\r\nedge add 4 4 0\r\nedge add 5 5 6\r\nedge add 6 6 7\r\nedge add 7 7 0";
        commandHandeler(s);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void commandHandeler(string commands) {
        string[] strings = commands.Split('\n');
        foreach (string s in strings) {
            string[] ss = s.Split(' ');
            if (ss[0] == "node") {
                editmenu.spawnNewNode(float.Parse(ss[3]), float.Parse(ss[4]));
            }
            else if(ss[0]== "edge"){ }
        }
            
    }
}
