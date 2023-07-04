using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Security.Cryptography.X509Certificates;

public class renameNode : MonoBehaviour
{
    static node savedNode;
    [SerializeField]TMP_InputField name;
    [SerializeField]workspace workspace;
    static public void setNode(node n) { savedNode = n;}
    static public node GetNode() { return savedNode; }
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
        transform.gameObject.SetActive(b);
    }
}
