using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class renameNode : MonoBehaviour
{
    static node savedNode;
    [SerializeField]TMP_InputField name;
    [SerializeField]workspace workspace;
    [SerializeField] GameObject renameObject;
    static public void setNode(node n) { savedNode = n;}
    static public node GetNode() { return savedNode; }
    public static renameNode instance;


    private void Start()
    {
        instance = this;
        renameObject.SetActive(false);
    }

    public void updateName()
    {
        savedNode.setName(name.text);
    }

    public void updatedText()
    {
        if (name.text.Length > 6) { name.text = name.text.Substring(0, 6); }
    }
    public void OpenRenameMenu(bool b) {
        name.text = savedNode.getName();
        workspace.openNodeOption(savedNode);
        renameObject.SetActive(b);
    }


    public static bool isOpened() { return instance.renameObject.activeInHierarchy; }


}
