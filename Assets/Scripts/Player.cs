using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Player : MonoBehaviour
{
    private Camera cam;
    private static Rigidbody playerRB;
    private static Collider playerCollider;

    private int x, z;
    private Vector3 offset;
    private Vector3 jump;
    public bool jumpAllowed;

    //Editor params
    public float moveSpeed;
    public float sprintMult;
    public float crouchMult;
    public float baseJumpForce;

    private bool noClip;

    private GameObject holdingNode;
    private float holdDistance;

    public TMP_InputField inputField;
    public Node editingNode;
    public bool inEdit;

    public bool inCreation;
    public Algorithms algoComponent;

    private Node stitchFrom;
    private Node stitchTo;
    private bool inStitch;

    private GameObject controlsPanel;

    // Start is called before the first frame update
    void Start()
    {

        playerRB = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        inputField = GetComponentInChildren<TMP_InputField>();
        algoComponent = GetComponent<Algorithms>();

        cam = Camera.main;
        jumpAllowed = true;
        jump = new Vector3(0, baseJumpForce, 0);

        noClip = true;

        controlsPanel = transform.GetComponentInChildren<ControlsPanel>().gameObject;
        controlsPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //print($"fps: {Mathf.Round(1.0f/Time.deltaTime)}");
        movement();

        nodeControls();

        nodeCreation();
    }

    private void nodeCreation()
    {
        if(inCreation)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                LinkedList.createNode(MouseLook.getCamPosition() + MouseLook.getCamForward() * 3, 0);
                TempText.fadeInDec = 0.2f;
                TempText.setTempText("Node created!", 1);
                inCreation = false;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                BinaryTree.createNode(MouseLook.getCamPosition() + MouseLook.getCamForward() * 3, 0);
                TempText.fadeInDec = 0.2f;
                TempText.setTempText("Node created!", 1);
                inCreation = false;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                GenericNode.createNode(MouseLook.getCamPosition() + MouseLook.getCamForward() * 3, 0);
                TempText.fadeInDec = 0.2f;
                TempText.setTempText("Node created!", 1);
                inCreation = false;
            }
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.Z))
            {
                inCreation = true;
                TempText.fadeInDec = 0f;
                TempText.setTempText("Node creation started: Press 1 for a LinkedList node, Press 2 for a Binary Tree Node, and Press 3 for a Generic Node", 300); // indefinite
            }
        }
    }

    private void movement()
    {
        if (playerRB.angularVelocity.magnitude != 0)
            playerRB.angularVelocity = Vector3.zero;

        if (inEdit) //cant move while editing nodes
            return;

        transform.eulerAngles = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.V))
            noClip = !noClip;

        //translate
        offset = Vector3.zero;

        z = x = 0;

        z += Input.GetKey(KeyCode.W) ? 1 : 0;
        z += Input.GetKey(KeyCode.S) ? -1 : 0;
        x += Input.GetKey(KeyCode.A) ? -1 : 0;
        x += Input.GetKey(KeyCode.D) ? 1 : 0;

        offset += cam.transform.forward * z;
        offset += cam.transform.right * x;
        offset.y = 0;

        Vector3 moveVector = offset.normalized * moveSpeed * Time.deltaTime;

        if (noClip)
        {
            playerCollider.enabled = false;
            playerRB.useGravity = false;
            playerRB.detectCollisions = false;
            playerRB.velocity = Vector3.zero;

            int y = 0;
            y += (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftShift)) ? -1 : 0;
            y += (Input.GetKey(KeyCode.E) || (Input.GetKey(KeyCode.Space) && !algoComponent.inAlgo)) ? 1 : 0;
            offset.y = y;


            moveVector = offset.normalized * moveSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.CapsLock))
                moveVector *= 2;
            transform.position += moveVector;

        }
        else
        {
            playerCollider.enabled = true;
            playerRB.useGravity = true;
            playerRB.detectCollisions = true;

            transform.position += moveVector;

            if (Input.GetKeyDown(KeyCode.Space))
                playerRB.AddForce(jump);
        }
    }

    private void nodeControls()
    {
        //ui
        if (Input.GetKeyDown(KeyCode.Tab))
            controlsPanel.SetActive(true);
        else if (Input.GetKeyUp(KeyCode.Tab))
            controlsPanel.SetActive(false);

        //Handle editing
        if (inEdit)
        {
            inEditMethod();
            return; //don't pickup and drop on same frame
        }

        //Raycasts
        raycasts();

        if(Input.GetKeyDown(KeyCode.O))
        {
            organize();
        }

    }

    public void organize()
    {
        foreach(Node n in GameObject.FindObjectsOfType<Node>())
        {
            //count nodes pointing to it:
            int count = 0;
            foreach(Link pointing in n.pointingTo) //don't count null elements
                count++;
            print($"Count: {count}");

            //if a node has no inward facing connections, then it's the head of a structure
            if (count == 0)
                Structure.createStructure(n);

        }
    }

    public void inEditMethod()
    {
        if (inEdit)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
            {
                //submit text to edit
                string fromField = inputField.text;
                short newValue;
                if (short.TryParse(fromField, out newValue))
                {
                    editingNode.Value = newValue;
                    TempText.setTempText($"Node Value set to {newValue}", 1);
                    editingNode = null;
                    inEdit = false;
                    inputField.text = "";
                    inputField.DeactivateInputField();
                }
                else
                {
                    TempText.setTempText($"Input failed because {fromField} is invalid!", 2);
                    inputField.text = ""; //clear field
                    inputField.ActivateInputField();
                }
            }

            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Escape))
            {
                inputField.text = "";
                inputField.DeactivateInputField();
                TempText.setTempText($"Editing Aborted", 1);
                inEdit = false;
            }

            return; // no other node controls while editing
        }
    }

    public void stitchNodes(Node n)
    {
        //print($"institch: {inStitch}");
        if (inStitch)
        {
            //type matching
            if (n.GetType() != stitchFrom.GetType())
            {
                //print("Type mismatch");
                TempText.setTempText("Type mismatch between nodes!", 2);
                inStitch = false;
                stitchFrom = null;
                stitchTo = null;
                return;
            }

            //discern type
            LinkedList ll;
            GenericNode tree;
            BinaryTree bt;

            stitchTo = n;

            if (stitchTo == stitchFrom)
            {
                TempText.setTempText("Cannot connect to itself!", 2);
                inStitch = false;
                stitchFrom = null;
                stitchTo = null;
                return;
            }

            //get type of current what you're stitching from
            if (ll = stitchFrom.gameObject.GetComponent<LinkedList>())
            {
                print("LL stitch");
                if (ll.next != null)
                {
                    if(ll.GetComponentInChildren<Link>())
                    {
                        GameObject arrow = ll.GetComponentInChildren<Link>().transform.gameObject;
                        Destroy(arrow);
                    }
                    Node.stitch(stitchFrom, stitchTo);
                    ll.next = stitchTo;
                }
                else
                {
                    Node.stitch(stitchFrom, stitchTo);
                    ll.next = stitchTo;
                }

            }
            else if (tree = stitchFrom.gameObject.GetComponent<GenericNode>())
            {
                tree.nextNodes.Add(stitchTo);
                tree.connections.Add(Node.stitch(stitchFrom, stitchTo));

            }
            else if (bt = stitchFrom.gameObject.GetComponent<BinaryTree>())
            {
                //check if somethin is already pointing into this node
                BinaryTree btTo = (BinaryTree)stitchTo;
                foreach(Link l in btTo.pointingTo)
                {
                    print("l");
                    if(l != null)
                    {
                        Node.removeLink(l);
                    }
                }

                if (stitchTo.Value < stitchFrom.Value) // on the left
                {
                    if (bt.left == null)
                    {
                        bt.left = stitchTo;
                        bt.leftLink = Node.stitch(stitchFrom, stitchTo);
                    }
                    else
                    {
                        Destroy(bt.leftLink.gameObject);
                        bt.leftLink = Node.stitch(stitchFrom, stitchTo);
                        bt.left = stitchTo;
                    }
                }
                else
                {
                    if(bt.right == null)
                    {
                        bt.right = stitchTo;
                        bt.rightLink = Node.stitch(stitchFrom, stitchTo);
                    }
                    else
                    {
                        Destroy(bt.rightLink.gameObject);
                        bt.rightLink = Node.stitch(stitchFrom, stitchTo);
                        bt.right = stitchTo;
                    }
                }
            }

            TempText.setTempText($"Node {stitchFrom.Value} connected to {stitchTo.Value}", 2);

            //After stitching:
            inStitch = false;
            stitchFrom = null;
            stitchTo = null;

        }
        else //select node, enter stitchin mode
        {
            stitchFrom = n;
            inStitch = true;
            TempText.setTempText($"Node of Value {n.Value} selected", 2);
        }

    }

    public void raycasts()
    {
        if (holdingNode != null)
        {
            holdingNode.transform.position = Vector3.Lerp(holdingNode.transform.position, MouseLook.getCamPosition() + MouseLook.getCamForward() * holdDistance, 0.5f); // smooth movement
            holdDistance += Input.mouseScrollDelta.y * 0.33f; // why is this in a vector2?
            holdDistance = Mathf.Clamp(holdDistance, 2, 15);
            if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(2))
            {
                dropNode();
                return; //don't pickup and drop on same frame
            }
            else if(Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
            {
                Node.deleteNode(holdingNode.GetComponent<Node>());
            }
        }

        //Check raycasts
        RaycastHit info;
        if (Physics.Raycast(MouseLook.getCamPosition(), MouseLook.getCamForward(), out info))
        {
            GameObject hit = info.collider.gameObject;
            Node n;
            Link l;
            //if looking at a node
            if (n = hit.GetComponent<Node>())
            {
                //Pick up or drop node
                if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(2))
                {
                    if (holdingNode != null)
                        dropNode();
                    pickupNode(n);
                    return; //don't pickup and drop on same frame
                }

                //Edit value on the node
                if (Input.GetKeyDown(KeyCode.X) || Input.GetMouseButtonDown(0))
                {
                    inEdit = true;
                    inputField.ActivateInputField();
                    editingNode = n;
                }

                //Prevent x from getting in the frame
                if (Input.GetKeyUp(KeyCode.X))
                    inputField.text = "";

                //Stitch two nodes together
                if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(1))
                {
                    stitchNodes(n);
                }

                if(Input.GetKeyDown(KeyCode.P))
                {
                    this.gameObject.GetComponent<Algorithms>().startAlgo(n);
                }
            }
            if (l = hit.GetComponentInParent<Link>())
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    Node.removeLink(l);
                }
            }
        }

        //Partial workaround - space reselects last input field but I don't want it to.
        if (Input.GetKeyUp(KeyCode.Space))
        {
            inputField.DeactivateInputField();
            inputField.text = "";
        }
    }

    private void pickupNode(Node n)
    {
        holdDistance = 5;
        holdingNode = n.gameObject;
    }

    private void dropNode()
    {
        print("dropping");
        Node.restitch(holdingNode.GetComponent<Node>());
        holdingNode = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //print("oce");
        jumpAllowed = true;
        playerRB.velocity = Vector3.zero;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //print("occh");
        jumpAllowed = true;
    }
}
