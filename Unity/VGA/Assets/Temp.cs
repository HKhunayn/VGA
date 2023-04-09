using System.Collections;
using System.Text;
using UnityEngine;
using TMPro;
using System.Net;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.Events;
using System;
using UnityEditor.Experimental.GraphView;

public class Temp : MonoBehaviour
{
    TMP_Text text;
    [SerializeField]string ip = "127.0.0.1";
    public UnityEvent myEvent;
    UnityEvent bla;
    [SerializeField] GameObject p;
    void Start()
    {
        text = GetComponent<TMP_Text>();
        pingo();
        StartCoroutine(pingo());
        Instantiate(p);
    }
   

    public void lol() {
        Debug.Log("whats you name (inside lol fun)");
    }
    public void lol2()
    {
        Debug.Log("whats you name (inside lol2 fun)");
    }

    string getIP(string s) {
        try {
            string regex = "[0-9]+\\.[0-9]+\\.[0-9]+\\.[0-9]+";
            Regex r = new Regex(regex);
            foreach (IPAddress o in Dns.GetHostAddresses(s))
            {
                if (r.IsMatch(o.ToString()))
                {
                    return o.ToString();
                }

            }
        }catch(System.Exception e) { return "127.0.0.1"; }
        return "127.0.0.1";

    }
    public void updateIP(TMP_InputField iff) {
        ip = iff.text; 
    }
    IEnumerator pingo() {

        Ping p;
        float t = 0;
        string s =string.Empty;
        while (true) {

            p = new Ping(getIP(ip));
            t = Time.time;
            yield return new WaitUntil(()=>(p.isDone |t+1f < Time.time));
            yield return new WaitForSeconds(1f);
            text.text = p.time + $"ms";
        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
