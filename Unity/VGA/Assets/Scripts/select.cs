using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class select : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        editMenu.addNode(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        editMenu.removeNode(collision.gameObject);
    }
    
}

