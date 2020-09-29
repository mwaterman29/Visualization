using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        //init refs
        //Refs
        Node.arrowPrefab = Resources.Load<GameObject>("Arrow");
        Node.nodePrefab = Resources.Load<GameObject>("Node");

        Node.btMat = Resources.Load<Material>("BTMat");
        Node.llMat = Resources.Load<Material>("LLMat");
        Node.treeMat = Resources.Load<Material>("TreeMat");

        createDemoLL();
        createDemoBT();
    }

    public void createDemoLL()
    {
        int[] test = { 1, 2, 3, 4, 5 };
        LinkedList.fromArr(test, new Vector3(5,2,5));
    }

    public void createDemoBT()
    {
        //this is quicker than implementing a method to create one
        //create a bunch of nodes and link them
        Vector3 startPos = new Vector3(-10, 0, 5);
        BinaryTree ten = BinaryTree.createNode(startPos, 10);
        BinaryTree five = BinaryTree.createNode(startPos, 5);
        BinaryTree two = BinaryTree.createNode(startPos, 2);
        BinaryTree seven = BinaryTree.createNode(startPos, 7);
        BinaryTree fifteen = BinaryTree.createNode(startPos, 15);
        BinaryTree twelve = BinaryTree.createNode(startPos, 12);
        BinaryTree twenty = BinaryTree.createNode(startPos, 20);
        BinaryTree seventeen = BinaryTree.createNode(startPos, 17);
        BinaryTree twentythree = BinaryTree.createNode(startPos, 23);

        //left side
        ten.left = five;
        ten.leftLink = Node.stitch(ten, five);

        five.left = two;
        five.leftLink = Node.stitch(five, two);

        five.right = seven;
        five.rightLink = Node.stitch(five, seven);

        //right side
        ten.right = fifteen;
        ten.rightLink = Node.stitch(ten, fifteen);

        fifteen.left = twelve;
        fifteen.leftLink = Node.stitch(fifteen, twelve);

        fifteen.right = twenty;
        fifteen.rightLink = Node.stitch(fifteen, twenty);

        twenty.left = seventeen;
        twenty.leftLink = Node.stitch(twenty, seventeen);

        twenty.right = twentythree;
        twenty.rightLink = Node.stitch(twenty, twentythree);

    }
}
