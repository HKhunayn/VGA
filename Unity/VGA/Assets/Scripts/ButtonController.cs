using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField] bool isSwitch= false;
    [SerializeField] bool isClickableAgain= false;
    [SerializeField]List<Image> childs;

    public void setSelected(Image m = null)
    {
        foreach (Image im in childs)
           im.enabled = true; // enable the button image for other childs
        if (m == null) 
        {
            foreach (Image im in childs)
                clickableAgain(im);
            return;
        }
            


        m.enabled = isSwitch;

        if (isClickableAgain) 
        {
            clickableAgain(m);
        }


    }

    void clickableAgain(Image m) 
    {
        m.GetComponent<Button>().interactable = false;
        m.GetComponent<Button>().interactable = true;
        m.enabled = true;
    }
}
