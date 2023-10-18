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
    public static void ShowNotification(string text) 
    {
        instance.notificationObject.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
        instance.notificationObject.SetActive(false);
        instance.notificationObject.SetActive(true);
    }
}
