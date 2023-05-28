using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class popup : MonoBehaviour
{
    // Start is called before the first frame update
    Camera cam;
    float lastZoom = 0;
    [SerializeField] float scaleMultiplie = 0.1f;
    [SerializeField] TMP_Text text;
    [SerializeField] Transform node;
    Vector3 lastPos;
    void Awake()
    {
        cam = Camera.main;
        
    }
    private void OnEnable()
    {
        lastPos = node.position;
        reScale();
        updateText();
        StartCoroutine(loop());
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }
    IEnumerator loop()
    {
        while (true) {
            if (lastZoom != cam.orthographicSize)
            {
                reScale();
            }
            if (node.position != lastPos)
                updateText();
            yield return new WaitForSeconds(0.1f);

        }

    }
    void reScale() {
        lastZoom = cam.orthographicSize;
        Vector3 diff = transform.localScale;
        transform.localScale = scaleMultiplie*lastZoom * Vector3.one;
        diff -= transform.localScale;
        transform.position -= new Vector3(0,diff.y,diff.z)/2f;
    }

    void updateText()
    {
        text.text = $"X:{node.position.x.ToString("F1")}, Y:{node.position.y.ToString("F1")}";
        lastPos = node.position;
    }

}
