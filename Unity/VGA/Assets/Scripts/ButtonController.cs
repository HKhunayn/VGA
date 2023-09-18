using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField] bool isSwitch= false;
    [SerializeField] bool isClickableAgain= false;
    [SerializeField]List<Image> childs; 

    public void setSelected(Image m)
    {
        foreach (Image im in childs)
           im.enabled = true; // enable the button image for other childs

        m.enabled = isSwitch;

        if (isClickableAgain) 
        {
            m.GetComponent<Button>().interactable = false;
            m.GetComponent<Button>().interactable = true;
            m.enabled = true;
        }



    }
}
