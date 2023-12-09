using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationSystem : MonoBehaviour
{
    [SerializeField] GameObject notificationObject;
    static NotificationSystem instance;
    void Start()
    {
        instance = this;
    }
    Coroutine notiCor;
    public static void ShowNotification(string text) 
    {
        if (instance.notiCor != null) 
        {
            return;
        }
        instance.notificationObject.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = text;
        instance.notiCor = instance.StartCoroutine(instance.IShowNotification());

    }

    IEnumerator IShowNotification() 
    {
        instance.notificationObject.SetActive(true);
        yield return new WaitForSeconds(3.5f); 
        instance.notificationObject.SetActive(false);
        StopCoroutine(instance.notiCor);
        notiCor = null;
    }
}
