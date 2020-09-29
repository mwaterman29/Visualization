using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Structure : MonoBehaviour
{
    //private static List<int> finalDepths;

    public static float treeWidthBetween;
    public static float treeHeightBetween;

    private static int width, height;

    // Start is called before the first frame update
    void Start()
    {
        treeHeightBetween = 2;
        treeWidthBetween = 2;
        //finalDepths = new List<int>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void createStructure(Node head)
    {
        GameObject structure = new GameObject($"Structure of {head.name}");
        structure.transform.position = head.transform.position;

        //head.transform.SetParent(structure.transform);

        structureSetparent(head, structure.transform);
        if (!validateStructure(structure))
        {
            print("Invalid structure!");
            TempText.setTempText($"Sorry, the structure with head {head.name} is not a valid {head.GetType().Name} structure", 3);
            return;
        }


        //figure out position based on structure type
        positionStructure(head);

    }

    public static void structureSetparent(Node n, Transform parent)
    {
        if (n.transform.parent == parent)
        {
            print($"loop found with node {n.name}");
            return;
        }
        n.transform.SetParent(parent);
        foreach (Link l in n.GetComponentsInChildren<Link>())
        {
            structureSetparent(l.to, parent);
        }
        return;
    }

    public static bool validateStructure(GameObject structure)
    {
        List<int> fromIds = new List<int>();
        List<int> toIds = new List<int>();

        foreach (Link l in structure.GetComponentsInChildren<Link>())
        {
            fromIds.Add(l.from.GetInstanceID());
            toIds.Add(l.to.GetInstanceID());
            if (fromIds.Contains(l.to.GetInstanceID())) // if loop, this is invalid
                return false;
        }
        print($"Fromids c: {fromIds.Count}");

        return true;
    }

    public static void positionStructure(Node head)
    {
        if (head.GetType() == typeof(LinkedList))
        {
            positionStructure((LinkedList)head);
        }
        else if (head.GetType() == typeof(BinaryTree))
        {
            positionStructure((BinaryTree)head);
        }
        else if (head.GetType() == typeof(GenericNode))
        {
            positionStructure((GenericNode)head);
        }
    }

    public static void positionStructure(LinkedList head)
    {
        LinkedList current = head;
        Vector3 currentPos = head.transform.position;
        while (current.next != null)
        {
            //move node
            current.next.transform.position = currentPos + LinkedList.direction * LinkedList.offset;
            //restitch it
            Node.restitch(current, current.next);
            //current = next
            current = (LinkedList)current.next;
            currentPos = current.transform.position;
        }
    }

    public static void positionStructure(BinaryTree head)
    {
        //For positions, find the height and width of the tree using the final depths array: width = length, height = max in the list
        //finalDepths = new List<int>();
        width = 0;
        height = 0;

        getBinaryTreeDepths(head, 0);
        //Make more efficient: store only width and maximum depth rather than a list
        print($"Width of tree: {width}");//{finalDepths.Count}");
        print($"Max Depth of tree: {height}");//{finalDepths.Max()}");

        if (width == 0 || height == 0)
        {
            print("Invalid structure!");
            TempText.setTempText($"Sorry, the structure with head {head.name} is not a valid {head.GetType().Name} structure", 3);
            return;
        }

        //move head to be above void
        head.transform.parent.position = new Vector3(head.transform.parent.position.x, height * treeHeightBetween + 3, head.transform.parent.position.z);

        //position each node
        positionBinaryTree(head, Vector3.zero, treeWidthBetween * width);

        //restitch
        foreach (Link l in head.transform.parent.GetComponentsInChildren<Link>())
            Node.restitch(l);
    }

    public static void positionBinaryTree(BinaryTree node, Vector3 position, float width)
    {
        print($"Positioning node {node.name} at {position.ToString()}");

        //set position of current node:
        node.transform.localPosition = position;

        //divide width by 2
        width /= 2.0f;

        //increment position by height
        position.y -= treeHeightBetween;

        //preform operation on left and right nodes
        if (node.left != null)
        {
            positionBinaryTree((BinaryTree)node.left, position - new Vector3(width, 0), width);
        }
        if (node.right != null)
        {
            positionBinaryTree((BinaryTree)node.right, position + new Vector3(width, 0), width);
        }
    }

    public static void getBinaryTreeDepths(BinaryTree head, int depth)
    {
        depth++;
        if (head.left == null && head.right == null)
        {
            //end of the branch
            //finalDepths.Add(depth);
            height = (depth > height) ? depth : height;
            width++;
            return;
        }
        else
        {
            if (head.left != null && head.leftLink != null)
                getBinaryTreeDepths((BinaryTree)head.left, depth);
            if (head.right != null && head.rightLink != null)
                getBinaryTreeDepths((BinaryTree)head.right, depth);
        }
    }

    public static void positionStructure(GenericNode head)
    {

    }
}
