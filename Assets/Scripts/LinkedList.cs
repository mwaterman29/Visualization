using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LinkedList : Node
{
    //Non-static per node parts
    public Node next = null;

    //Static global variables
    public static int offset = 4;
    public static Vector3 direction = Vector3.right;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static LinkedList fromArr(int[] arr, Vector3 startpos)
    {
        LinkedList head;
        LinkedList current;
        Vector3 currentPos = startpos;

        head = createNode(startpos, arr[0]);

        current = head;

        for (int i = 1; i < arr.Length; i++)
        {
            LinkedList node;
            currentPos += direction * offset;
            node = createNode(currentPos, arr[i]);
            current.next = node;
            stitch(current, node);
            current = node;
        }

        return head;
    }

    public static LinkedList createNode(Vector3 pos, int value)
    {
        LinkedList node;
        GameObject headObject = Instantiate(nodePrefab);
        headObject.transform.position = pos;
        node = headObject.AddComponent<LinkedList>();
        node.Value = value;
        node.transform.GetComponentInChildren<TMP_Text>().text = $"{value}";
        node.GetComponent<Renderer>().material = Node.llMat;
        node.name = $"LinkedList {value}";
        return node;
    }

    public static void createNode(Vector3 pos, int value, LinkedList next)
    {
        LinkedList node;
        node = createNode(pos, value);
        node.next = next;
    }

    IEnumerator waitCoroutine(float s)
    {
        yield return new WaitForSeconds(s);
    }
}
