using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField]List<Image> childs; 

    public void setSelected(Image m)
    {
        foreach (Image im in childs)
           im.enabled = true; // enable the button image for other childs
        m.enabled = false;
    }
}
