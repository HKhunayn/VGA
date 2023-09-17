using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class edgeClick : MonoBehaviour
{
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1) || Input.touchCount > 0)
            Debug.Log("Clicked lolol;olo");
    }

}
