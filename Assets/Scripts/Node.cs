using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class Node : MonoBehaviour
{
    //Non-static per-node parts
    public List<Link> pointingTo = new List<Link>();
    protected int value;
    public virtual int Value
    {
        get
        {
            return this.value;
        }
        set
        {
            transform.GetComponentInChildren<TMP_Text>().text = $"{value}";
            if (gameObject.name.IndexOf(' ') != -1)
                gameObject.name = gameObject.name.Substring(0, gameObject.name.IndexOf(' ')) + $"{value}";
            //rename links
            /*
            foreach (Link l in transform.GetComponentsInChildren<Link>())
            {
                if (l != null)
                {
                    l.gameObject.name = ($"{value} to {l.to.value}");
                }
            }

            foreach (Link l in this.pointingTo)
            {
                if (l != null)
                {
                    l.gameObject.name = ($"{l.from.value} to {value}");
                }
            }
            */
            this.value = value;
        }

    }


    //Static global variables and methods
    public Node head;

    public static GameObject arrowPrefab;
    public static GameObject nodePrefab;

    public static Material btMat, treeMat, llMat;

    public virtual void Start()
    {
        
    }


    public static Link stitch(Node a, Node b)
    {
        string nodeName = $"{a.GetInstanceID()} to {b.GetInstanceID()}";

        print($"Stiching {a.value} to {b.value}, with name: {nodeName}");

        //don't stich to itself:
        if (a.Equals(b))
        {
            Debug.LogWarning($"Self stitch attempted with node {a.name}");
            return null;
        }

        //check if this already exists
        if (a.transform.Find(nodeName) != null)
        {
            GameObject existing = a.transform.Find(nodeName).gameObject;
            Destroy(existing);
        }

        GameObject arrow = Instantiate(arrowPrefab);
        arrow.transform.position = (a.transform.position + b.transform.position) / 2;
        arrow.transform.LookAt(b.transform);
        arrow.transform.SetParent(a.transform);
        float d = Vector3.Distance(a.transform.position, b.transform.position);
        float scale = 0.2f * d;
        arrow.transform.localScale = new Vector3(scale, scale, scale - 0.2f);
        arrow.name = nodeName;

        Link arrowComponent = arrow.AddComponent<Link>();
        arrowComponent.from = a;
        arrowComponent.to = b;

        b.pointingTo.Add(arrowComponent); //yes this is doubly-linking but it's only for graphics purposes

        return arrowComponent;
    }
    public static void restitch(Node a, Node b)
    {
        string nodeName = $"{a.GetInstanceID()} to {b.GetInstanceID()}";
        print($"Restiching {a} to {b} with name: {nodeName}");
        GameObject arrow = a.transform.Find(nodeName).gameObject;
        //reposition and rotate
        arrow.transform.position = (a.transform.position + b.transform.position) / 2;
        arrow.transform.LookAt(b.transform);
        arrow.transform.SetParent(a.transform);
        float d = Vector3.Distance(a.transform.position, b.transform.position);
        float scale = 0.2f * d;
        arrow.transform.localScale = new Vector3(scale, scale, scale - 0.2f);
    }

    public static void restitch(Link l)
    {
        restitch(l.from, l.to);

    }

    public static void restitch(Node n)
    {

        foreach (Link l in n.transform.GetComponentsInChildren<Link>())
        {
            if(l != null)
            {
                restitch(l);
            }
        }

        GenericNode t;
        if(t = n.gameObject.GetComponent<GenericNode>())
        {
            foreach (Link l in t.connections)
            {
                if (l != null)
                {
                    restitch(l);
                }
            }
        }

        foreach (Link l in n.pointingTo)
        {
            if (l != null)
            {
                restitch(l);
            }
        }
    }

    //internal only, not a player control
    public static void flip(Link l)
    {
        Node to = l.to;
        Node from = l.from;

        //fix pointing to
        to.pointingTo.Remove(l);
        from.pointingTo.Add(l);

        //fix next 
        if(to.GetType() == typeof(BinaryTree))
        {
            BinaryTree toBT = (BinaryTree)to;
            BinaryTree fromBT = (BinaryTree)from;

            //binary tree nodes can only have one inward facing node (structures also require this)
            foreach(Link linkTo in fromBT.pointingTo)
            {
                if(linkTo != null)
                {
                    TempText.setTempText("Sorry, flipping this link would create an invalid structure. Operation aborted", 3);
                    return;
                }
            }

            if (fromBT.Value < toBT.Value) // on the left
            {
                if (toBT.left == null)
                {
                    toBT.left = fromBT;
                }
                else
                {
                    Destroy(toBT.leftLink.gameObject);
                    toBT.left = fromBT;
                }

                //if the new one is on the left, that means that originally, this node had a greater value, meaning it's on the right.
                fromBT.right = null;
                fromBT.rightLink = null;
            }
            else
            {
                if (toBT.right == null)
                {
                    toBT.right = fromBT;
                }
                else
                {
                    Destroy(toBT.right.gameObject);
                    toBT.right = fromBT;
                }
                //if the new one is on the left, that means that originally, this node had a greater value, meaning it's on the right.
                fromBT.left = null;
                fromBT.leftLink = null;
            }
        }
        else if(to.GetType() == typeof(LinkedList))
        {
            LinkedList toLL = (LinkedList)to;
            LinkedList fromLL = (LinkedList)from;

            toLL.next = fromLL;

        }
        else if(to.GetType() == typeof(GenericNode))
        {
            GenericNode toN = (GenericNode)to;
            GenericNode fromN = (GenericNode)from;

            toN.nextNodes.Add(fromN);
            fromN.nextNodes.Remove(toN);
        }

        Node temp = l.from;
        l.from = l.to;
        l.to = temp;
        l.gameObject.name = $"{l.from.GetInstanceID()} to {l.to.GetInstanceID()}";
        //modified restitch
        Node a = l.from;
        Node b = l.to;
        string nodeName = $"{a.GetInstanceID()} to {b.GetInstanceID()}";
        print($"Restiching {a} to {b} with name: {nodeName}");
        GameObject arrow = b.transform.Find(nodeName).gameObject;
        //reposition and rotate
        arrow.transform.position = (a.transform.position + b.transform.position) / 2;
        arrow.transform.LookAt(b.transform);
        arrow.transform.SetParent(a.transform);
        float d = Vector3.Distance(a.transform.position, b.transform.position);
        float scale = 0.2f * d;
        arrow.transform.localScale = new Vector3(scale, scale, scale - 0.2f);
    }

    public static void removeLink(Link l)
    {
        Node from = l.from;
        if (from.GetType() == typeof(BinaryTree))
        {
            BinaryTree btFrom = (BinaryTree)from;
            if(l.from.value < l.to.value)
            {
                btFrom.right = null;
                btFrom.rightLink = null;
            }
            else
            {
                btFrom.left = null;
                btFrom.leftLink = null;
            }
        }
        else if (from.GetType() == typeof(LinkedList))
        {
            LinkedList llFrom = (LinkedList)from;
            llFrom.next = null;
        }
        else if (from.GetType() == typeof(GenericNode))
        {
            GenericNode tnFrom = (GenericNode)from;
            tnFrom.nextNodes.Remove(l.to);
        }
        Destroy(l.gameObject);
    }

    public static void deleteNode(Node n)
    {
        foreach(Link l in n.pointingTo)
        {
            Destroy(l.gameObject);
        }
        Destroy(n.gameObject);
    }

}
