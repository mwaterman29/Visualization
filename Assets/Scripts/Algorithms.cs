using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;

public class Algorithms : MonoBehaviour
{
    public enum Algorithm { BranchSums = 0, BinarySearch = 1, LinkedListReversal = 2, LinearSearch = 3, ShiftLinkedList = 4};
    public Algorithm selectedAlgorithm;
    private int aMin, aMax;

    public enum IterationMode { Spacebar = 0, Wait1 = 1, Wait3 = 2, Wait10 = 3, Instantaneous = 4 };
    public IterationMode iterationMode;
    private int iMin, iMax;

    //node storage to be held between asyncronous steps of the algorithm
    private Node head;
    private List<Node> selectedNodes = new List<Node>();
    private GameObject selectionRef;
    private GameObject outputRef;

    public bool inAlgo = false;

    private Player player;

    public void Start()
    {
        //initialze object references
        selectionRef = Resources.Load("Selection") as GameObject;
        outputRef = Resources.Load("Output") as GameObject;

        //switching algorithms
        aMax = (int)Enum.GetValues(typeof(Algorithm)).Cast<Algorithm>().Max();
        aMin = (int)Enum.GetValues(typeof(Algorithm)).Cast<Algorithm>().Min();

        //switching iteration modes
        iMax = (int)Enum.GetValues(typeof(IterationMode)).Cast<IterationMode>().Max();
        iMin = (int)Enum.GetValues(typeof(IterationMode)).Cast<IterationMode>().Min();
        player = GetComponent<Player>();

        //defaults
        selectedAlgorithm = 0;
        iterationMode = 0;
    }

    public void Update()
    {
        //switch iteration mode
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            selectedAlgorithm = ((int)selectedAlgorithm) == aMin ? (Algorithm)aMax : (Algorithm)(selectedAlgorithm - 1);
            TempText.setTempText($"Selected algorithm: {selectedAlgorithm.ToString()}", 3);
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            selectedAlgorithm = ((int)selectedAlgorithm) == aMax ? (Algorithm)aMin : (Algorithm)(selectedAlgorithm + 1);
            TempText.setTempText($"Selected algorithm: {selectedAlgorithm.ToString()}", 3);
        }

        if(Input.GetKeyDown(KeyCode.Semicolon))
        {
            iterationMode = ((int)iterationMode) == iMin ? (IterationMode)iMax : (IterationMode)(iterationMode - 1);
            TempText.setTempText($"Selected iteration mode: {iterationMode.ToString()}", 3);
        }
        if(Input.GetKeyDown(KeyCode.Quote))
        {
            iterationMode = ((int)iterationMode) == iMax ? (IterationMode)iMin : (IterationMode)(iterationMode + 1);
            TempText.setTempText($"Selected iteration mode: {iterationMode.ToString()}", 3);
        }
    }

    IEnumerator WaitForKeyDown(KeyCode keyCode)
    {
        while (!Input.GetKeyDown(keyCode))
            yield return null; //new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
#if UNITY_WEBGL
        while (!Input.GetKeyDown(keyCode))
            yield return null;
        yield return new WaitForEndOfFrame();
#endif
    }

    IEnumerator WaitForKeyUp(KeyCode keyCode)
    {
        while (!Input.GetKeyUp(keyCode))
            yield return null;
    }

    IEnumerator WaitSeconds(float s)
    {
        yield return new WaitForSeconds(s);
    }

    IEnumerator WaitForIteration() // wait or space
    {
        switch(iterationMode)
        {
            case IterationMode.Spacebar:
                yield return WaitForKeyDown(KeyCode.Space);
                break;
            case IterationMode.Wait1:
                yield return WaitSeconds(1);
                break;
            case IterationMode.Wait3:
                yield return WaitSeconds(3);
                break;
            case IterationMode.Wait10:
                yield return WaitSeconds(10);
                break;
            case IterationMode.Instantaneous:
                yield return null;
                break;
            default:
                yield return null;
                break;
        }
    }
    

    public  void startAlgo(Node n)
    {
        if (inAlgo)
        {
            TempText.setTempText("There is already an algorithm in process. Please finish that one's iterations before starting a new one", 3);
            return;
        }
        firstStep(n);
    }


    public  void firstStep(Node n)
    {
        inAlgo = true;
        IEnumerator routine;
        switch(selectedAlgorithm)
        {
            case Algorithm.BranchSums:
                if (n.GetType() != typeof(BinaryTree))
                {
                    TempText.setTempText("Sorry, this type of node isn't valid for Branch Summation", 3);
                    inAlgo = false;
                    return;
                }
                routine = BranchSums((BinaryTree)n);
                StartCoroutine(routine);
                break;
            case Algorithm.LinkedListReversal:
                if (n.GetType() != typeof(LinkedList))
                {
                    TempText.setTempText("Sorry, this type of node isn't valid for Linked List Reversal", 3);
                    inAlgo = false;
                    return;
                }
                routine = LinkedListReversal((LinkedList)n);
                StartCoroutine(routine);
                break;
            case Algorithm.LinearSearch:
                if (n.GetType() != typeof(LinkedList))
                {
                    TempText.setTempText("Sorry, this type of node isn't valid for Linear Search", 3);
                    inAlgo = false;
                    return;
                }
                routine = LinearSearch((LinkedList)n);
                StartCoroutine(routine);
                break;
            case Algorithm.BinarySearch:
                if (n.GetType() != typeof(BinaryTree))
                {
                    TempText.setTempText("Sorry, this type of node isn't valid for Binary Search", 3);
                    inAlgo = false;
                    return;
                }
                routine = BinarySearch((BinaryTree)n);
                StartCoroutine(routine);
                break;
            case Algorithm.ShiftLinkedList:
                if (n.GetType() != typeof(LinkedList))
                {
                    TempText.setTempText("Sorry, this type of node isn't valid for Shifting", 3);
                    inAlgo = false;
                    return;
                }
                routine = ShiftLinkedList((LinkedList)n);
                StartCoroutine(routine);
                break;
            default:
                Debug.LogError($"Something broke with selected algortithm {selectedAlgorithm}");
                break;
        }
    }

    private  void createSelectionAt(Node n)
    {
        GameObject g = Instantiate(selectionRef); //GameObject.CreatePrimitive(PrimitiveType.Sphere);
        g.transform.position = n.transform.position;
        g.transform.localScale = Vector3.one * 1.5f;
        g.name = $"S{n.GetInstanceID()}";
    }

    private  void removeSelectionAt(Node n)
    {
        GameObject toDestroy = GameObject.Find($"S{n.GetInstanceID()}");
        Destroy(toDestroy);
        return;
    }

    private GameObject createOutputAt(Node n)
    {
        GameObject op = Instantiate(outputRef);
        op.transform.position = n.transform.position + 2*Vector3.up;
        return op;
    }

    //Branch Sums

    IEnumerator BranchSums(BinaryTree head)
    {
        print("start");
        //init lists & value
        GameObject output = createOutputAt(head);
        TMP_Text outputText = output.GetComponentInChildren<TMP_Text>();
        outputText.text = "Branch Summation";

        bool done = false;
        Dictionary<int, int> sumAtNodes = new Dictionary<int, int>();
        List<Node> selectedNodes = new List<Node>();
        List<int> branchSums = new List<int>();

        //check edge case: head has no left or right nodes
        if(head.left == null && head.right == null)
        {
            done = true;
            sumAtNodes.Add(-1, head.Value);
        }

        //first iteration is just the head
        selectedNodes.Add(head);
        sumAtNodes.Add(head.GetInstanceID(), 0);
        createSelectionAt(head);

        yield return StartCoroutine(WaitForIteration());

        removeSelectionAt(head);

        int iterations = 0;

        while (done == false)
        {
            if (iterations % 2 == 1)
                yield return null;

            print($"iterating at {iterations}, selected node size {selectedNodes.Count}");
            outputText.text = "Summing branches...";
            iterations++;
            bool finishedThisIteration = true;
            //add all nodes at the end as to not modify selected nodes between 
            List<Node> nextNodes = new List<Node>();
            List<Node> visitedNodes = new List<Node>();

            foreach (Node n in selectedNodes)
            {
                BinaryTree bt = (BinaryTree)n;
                //check if this is the end of a branch
                if(bt.left == null && bt.right == null)
                {
                    branchSums.Add(sumAtNodes[bt.GetInstanceID()] + bt.Value);
                    //remove selection of current node
                    removeSelectionAt(bt);
                    visitedNodes.Add(bt);
                }
                else
                {
                    //if any nodes aren't at the end of their branches, finish iterating
                    finishedThisIteration = false;
                    //select left if not null
                    if(bt.left != null)
                    {
                        //add it to the sums
                        sumAtNodes[bt.left.GetInstanceID()] = sumAtNodes[bt.GetInstanceID()] + bt.Value;
                        //select it
                        createSelectionAt(bt.left);
                        nextNodes.Add(bt.left);
                    }
                    //select right if not null
                    if(bt.right != null)
                    {
                        //add it to the sums
                        sumAtNodes[bt.right.GetInstanceID()] = sumAtNodes[bt.GetInstanceID()] + bt.Value;
                        //select it
                        createSelectionAt(bt.right);
                        nextNodes.Add(bt.right);
                    }
                    //remove selection of current node
                    removeSelectionAt(bt);
                    visitedNodes.Add(bt);
                }
            }

            selectedNodes.AddRange(nextNodes);
            foreach (Node n in visitedNodes)
                selectedNodes.Remove(n);


            if (finishedThisIteration)
            {
                done = true;
                print("done!");
            }
            else
            {
                yield return StartCoroutine(WaitForIteration());
            }
        }
        foreach (Node n in selectedNodes)
            removeSelectionAt(n);

        string s = "{";
        foreach (int i in branchSums)
            s += i + ", ";
        s += "}";
        //print($"fin with sums: {s}");
        outputText.text = $"Sums: {s}";
        yield return StartCoroutine(WaitForIteration());
        Destroy(output);
        inAlgo = false;
    }

    IEnumerator LinkedListReversal(Node n)
    {
        LinkedList previous, current, next;
        previous = (LinkedList)n;
        current = (LinkedList)previous.next;

        //head becomes tail
        previous.next = null;

        createSelectionAt(previous);
        createSelectionAt(current);

        //one input on first iteration
        yield return StartCoroutine(WaitForIteration());

        while (current.next != null)
        {
            print("Iterating");

            //1. store next
            next = (LinkedList)current.next;
            //2. make current point to previous
            current.next = previous;
            //2.1 swap link direction
            Link l = previous.gameObject.GetComponentInChildren<Link>();
            Node.flip(l);
            //2.2 deselect previous before overwriting it
            removeSelectionAt(previous);
            //3. make previous = current;
            previous = current;
            //4. make current = next;
            current = next;
            //4.1 select next node;
            createSelectionAt(current);

            yield return StartCoroutine(WaitForIteration());
        }

        //one input on last iteration

        Link lastLink = previous.gameObject.GetComponentInChildren<Link>();
        Node.flip(lastLink);

        yield return StartCoroutine(WaitForIteration());

        removeSelectionAt(previous);
        removeSelectionAt(current);

        inAlgo = false;
    }

    IEnumerator ShiftLinkedList(LinkedList head)
    {
        GameObject output = createOutputAt(head);
        TMP_Text outputText = output.GetComponentInChildren<TMP_Text>();

        //To shift a linked list, gotta keep track of the head, tail, and nodes number of shift away from both ends
        //get num to shift first:
        int shift = -1;

        player.inputField.ActivateInputField();
        TempText.setTempText("Please enter the number to shift by... (Press space when finished)", 2);

        outputText.text = "Shift by...";

        yield return StartCoroutine(WaitForIteration());

        player.inputField.DeactivateInputField();
        string input = player.inputField.text;
        while (!Int32.TryParse(input, out shift))
        {
            TempText.setTempText("Invalid input, try again", 2);

            yield return StartCoroutine(WaitForKeyUp(KeyCode.Space));

            player.inputField.ActivateInputField();

            yield return StartCoroutine(WaitForKeyDown(KeyCode.Space));

            player.inputField.DeactivateInputField();
            input = player.inputField.text;
        }

        //Count length of the list
        int length = 1;
        LinkedList current = head;
        List<int> values = new List<int>(); //take in all values for graphic implementation
        while (current != null)
        {
            createSelectionAt(current);
            outputText.text = $"Current length = {length}";
            values.Add(current.Value);
            yield return StartCoroutine(WaitForIteration());
            length++;
            removeSelectionAt(current);
            current = (LinkedList)current.next;
        }

        //Now iterate across the list again, with the true number of shifts and the length known
        shift %= length;
        if(shift == 0)
        {
            outputText.text = $"Shifting by some multiple of the length of the list, therefore the original list is preserved.";
            yield return StartCoroutine(WaitForIteration());
        }
        else
        {
            outputText.text = $"Iterating, keeping track of the nodes {Mathf.Abs(shift)} from head/tail.";
            yield return StartCoroutine(WaitForIteration());

            LinkedList kForward = null, kBack = null, end = null;
            current = head;
            int index = 1;
            while(current.next != null)
            {
                createSelectionAt(current);
                if(index == Mathf.Abs(shift))
                {
                    kBack = head;
                    kForward = current;
                }
                if (index > Mathf.Abs(shift))
                    kBack = (LinkedList)kBack.next;
                index++;
                yield return StartCoroutine(WaitForIteration());
                removeSelectionAt(current);
                current = (LinkedList)current.next;
            }
            end = current;

            if(shift > 0)
            {
                outputText.text = $"The new head will be {kBack.next.Value}, and the new tail will be {kBack.Value}";
                createSelectionAt(kBack);
                createSelectionAt(kBack.next);
                yield return StartCoroutine(WaitForIteration());
                //though it is theoretically efficient to shift in place, since unity is being used, it's more efficient to change the node values in place, rather than to actually move GameObjects
                for (int i = 0; i < shift; i++)
                {
                    //get last element, bring it to the front
                    print($"Moving with c: {values.Count}, first = {values[0]}, and last  = {values[values.Count-1]}, full = {values.ToString()}");
                    int item = values[values.Count - 1];
                    values.RemoveAt(values.Count - 1);
                    values.Insert(0, item);
                }
                current = head;
                index = 0;
                while(current != null)
                {
                    current.Value = values[index];
                    current = (LinkedList)current.next;
                    index++;
                }
                yield return StartCoroutine(WaitForIteration());
                removeSelectionAt(kBack);
                removeSelectionAt(kBack.next);
                outputText.text = $"Linked list shifted by {shift}";
                yield return StartCoroutine(WaitForIteration());

            }
            else
            {
                outputText.text = $"The new head will be {kForward.next.Value}, and the new tail will be {kForward.Value}";
                createSelectionAt(kForward);
                createSelectionAt(kForward.next);
                yield return StartCoroutine(WaitForIteration());
                //though it is theoretically efficient to shift in place, since unity is being used, it's more efficient to change the node values in place, rather than to actually move GameObjects
                for (int i = 0; i < Mathf.Abs(shift); i++)
                {
                    //get first element, bring it to the end
                    int item = values[0];
                    values.RemoveAt(0);
                    values.Add(item);
                }
                current = head;
                index = 0;
                while (current != null)
                {
                    current.Value = values[index];
                    current = (LinkedList)current.next;
                    index++;
                }
                yield return StartCoroutine(WaitForIteration());
                removeSelectionAt(kForward);
                removeSelectionAt(kForward.next);
                outputText.text = $"Linked list shifted by {shift}";
                yield return StartCoroutine(WaitForIteration());
            }
        }

        Destroy(output);

        inAlgo = false;

        yield return null;
    }

    IEnumerator LinearSearch(LinkedList head)
    {
        bool found = false;
        GameObject output = createOutputAt(head);
        TMP_Text outputText = output.GetComponentInChildren<TMP_Text>();

        //get value from input field
        int value = -1;

        player.inputField.ActivateInputField();
        TempText.setTempText("Please enter the number to search for... (Press space when finished)", 2);

        outputText.text = "Linear Search for...";

        yield return StartCoroutine(WaitForIteration());

        player.inputField.DeactivateInputField();
        string input = player.inputField.text;
        while(!Int32.TryParse(input, out value))
        {
            TempText.setTempText("Invalid input, try again", 2);

            yield return StartCoroutine(WaitForKeyUp(KeyCode.Space));

            player.inputField.ActivateInputField();

            yield return StartCoroutine(WaitForIteration());

            player.inputField.DeactivateInputField();
            input = player.inputField.text;
        }

        LinkedList current = head;
        //first iteration
        outputText.text = $"Linear Search for {value}";
        yield return StartCoroutine(WaitForIteration());

        while (current != null)
        {
            createSelectionAt(current);
            outputText.text = $"Does {current.Value} == {value}?";
            if (current.Value == value)
            {
                //print("found value");
                outputText.text = $"Yes, {value} found";
                found = true;
                yield return StartCoroutine(WaitForIteration());
                removeSelectionAt(current);
                break;
            }

            yield return StartCoroutine(WaitForIteration());
            removeSelectionAt(current);
            current = (LinkedList)current.next;
        }

        if (!found)
        {
            outputText.text = $"{value} not found in the list...";
            yield return StartCoroutine(WaitForIteration());
        }

        Destroy(output);

        inAlgo = false;

        yield return null;
    }

    IEnumerator BinarySearch(BinaryTree head)
    {
        bool found = false;
        GameObject output = createOutputAt(head);
        TMP_Text outputText = output.GetComponentInChildren<TMP_Text>();

        //get value from input field
        int value = -1;

        player.inputField.ActivateInputField();
        TempText.setTempText("Please enter the number to search for... (Press space when finished)", 2);

        outputText.text = "Binary Search for...";

        yield return StartCoroutine(WaitForIteration());

        player.inputField.DeactivateInputField();
        string input = player.inputField.text;
        while (!Int32.TryParse(input, out value))
        {
            TempText.setTempText("Invalid input, try again", 2);

            yield return StartCoroutine(WaitForKeyUp(KeyCode.Space));

            player.inputField.ActivateInputField();

            yield return StartCoroutine(WaitForIteration());

            player.inputField.DeactivateInputField();
            input = player.inputField.text;
        }

        BinaryTree current = head;
        //first iteration
        outputText.text = $"Binary Search for {value}";
        yield return StartCoroutine(WaitForIteration());
        while (!found)
        {
            createSelectionAt(current);
            outputText.text = $"Does {current.Value} == {value}?";
            if(current.Value == value)
            {
                yield return StartCoroutine(WaitForIteration());
                removeSelectionAt(current);
                outputText.text = $"Yes, {value} found";
                found = true;
                break;
            }

            yield return StartCoroutine(WaitForIteration());
            removeSelectionAt(current);

            if(current.left == null && current.right == null)
            {
                outputText.text = $"{value} not found in the tree...";
                break;
            }
            else
            {
                //conditional and be like
                if(current.left != null && value < current.Value)
                {
                    current = (BinaryTree)current.left;
                }
                else if(current.right != null && value >= current.Value)
                {
                    current = (BinaryTree)current.right;
                }
                else
                {
                    outputText.text = $"{value} not found in the list...";
                    break;
                }

            }
        }
        removeSelectionAt(current);

        yield return StartCoroutine(WaitForIteration());

        Destroy(output);

        inAlgo = false;
    }

    IEnumerator ClosestBTValue(Node n)
    {
        yield return null;
    }
}
