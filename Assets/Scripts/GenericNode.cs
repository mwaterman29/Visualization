using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GenericNode : Node
{
    public List<Link> connections = new List<Link>();
    public List<Node> nextNodes = new List<Node>();

    public static GenericNode createNode(Vector3 pos, int value)
    {
        GenericNode node;
        GameObject headObject = Instantiate(nodePrefab);
        headObject.transform.position = pos;
        node = headObject.AddComponent<GenericNode>();
        node.Value = value;
        node.transform.GetComponentInChildren<TMP_Text>().text = $"{value}";
        node.GetComponent<Renderer>().material = Node.treeMat;
        node.name = $"Generic {value}";
        return node;
    }
}
