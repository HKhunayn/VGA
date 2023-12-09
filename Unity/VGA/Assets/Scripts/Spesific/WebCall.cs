using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Linq;
using System.Text;

public class WebCall : MonoBehaviour
{
    private string csrf_token ="";
    [SerializeField]string ip = "127.0.0.1:8000";
    string tokenURL = "";
    string deleteCookieURL = "";
    string getIDURL = "";
    string getgraphURL = "";
    string postURL = "";
    int id=0;
    private static WebCall instance;
    private void Start()
    {
        instance = this;
        tokenURL = $"http://{ip}/get_token/";
        getIDURL = $"http://{ip}/get_session/";
        deleteCookieURL = $"http://{ip}/delete_cookie/";
        getgraphURL = $"http://{ip}/editors/";
        GetToken();
        try
        {
            //int x = int.Parse(Application.absoluteURL.Split("editors/")[1]);
            //getURL = Application.absoluteURL;
            GetData();
            //ip = ip.Substring(0, ip.Length - 1);
        }
        catch {
            NotificationSystem.ShowNotification("Something wrong happend!");
        }
        postURL = $"http://{ip}/editors";
        
        //temp2.text = "IP:"+ip;   
    }

    public void GetData()
    {
        //temp.text = "getting DATA from " + getIDURL;
        StartCoroutine(IGetRequest());
    }

    IEnumerator IGetRequest()
    {
        UnityWebRequest www = UnityWebRequest.Get(getIDURL);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) { }
        //NotificationSystem.ShowNotification(www.error);
        else
        {
            id = int.Parse(www.downloadHandler.text);



            www = UnityWebRequest.Get(getgraphURL + id);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) { }
            //NotificationSystem.ShowNotification(www.error);
            else
            {

                try
                {
                    actionMenu.getGraph(www.downloadHandler.text);
                    //NotificationSystem.ShowNotification(www.downloadHandler.text);

                }
                catch
                { //NotificationSystem.ShowNotification("Can not apear the graph"); }
                }

                deleteCookie();

            }
        }
            
    }
    public void GetToken()
    {
        //NotificationSystem.ShowNotification("getting TOKEN from " + tokenURL);
        StartCoroutine(IGetTokenRequest());
    }

    IEnumerator IGetTokenRequest()
    {
        UnityWebRequest www = UnityWebRequest.Get(tokenURL);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) { }
            //NotificationSystem.ShowNotification(www.error);
        else 
        {
            csrf_token = www.downloadHandler.text;
            //NotificationSystem.ShowNotification(www.downloadHandler.text);
        }
            
    }

    public void deleteCookie()
    {
        //NotificationSystem.ShowNotification("getting TOKEN from " + tokenURL);
        StartCoroutine(IDeleteCookie());
    }

    IEnumerator IDeleteCookie()
    {
        UnityWebRequest www = UnityWebRequest.Get(deleteCookieURL);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) { }
            //NotificationSystem.ShowNotification(www.error);

    }


    public void SendData(string data)
    {
        //NotificationSystem.ShowNotification("sending DATA to " + postURL);
        StartCoroutine(IPostRequest(data));
    }

    IEnumerator IPostRequest(string data)
    {

        Dictionary<string, string> wwwForm = new Dictionary<string, string>();
        wwwForm.Add("_token", csrf_token);
        wwwForm.Add("data", data+"");
        wwwForm.Add("name", editMenu.getAllNode().Count+" nodes");
        //wwwForm.Add("name", editMenu.getAllNode().Count()+" nodes graph");
        UnityWebRequest www = UnityWebRequest.Post(postURL, wwwForm);
        yield return www.SendWebRequest();

/*        if (www.isNetworkError || www.isHttpError) { }
            //NotificationSystem.ShowNotification(www.error);

        else
            NotificationSystem.ShowNotification(www.downloadHandler.text);*/
    }

    public static void SaveGraph(string graph) 
    {
        //Debug.Log(graph);
       instance.SendData(graph);
    }
}