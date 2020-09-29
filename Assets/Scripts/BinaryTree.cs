using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BinaryTree : Node
{
    public Node left;
    public Node right;

    public Link leftLink;
    public Link rightLink;

    public static BinaryTree createNode(Vector3 pos, int value)
    {
        BinaryTree node;
        GameObject headObject = Instantiate(nodePrefab);
        headObject.transform.position = pos;
        node = headObject.AddComponent<BinaryTree>();
        node.Value = value;
        node.transform.GetComponentInChildren<TMP_Text>().text = $"{value}";
        node.GetComponent<Renderer>().material = Node.btMat;
        node.name = $"BinaryTree {value}";
        return node;
    }
}
