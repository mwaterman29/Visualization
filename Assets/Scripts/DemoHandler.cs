using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //init refs
        //Refs
        Node.arrowPrefab = Resources.Load<GameObject>("Arrow");
        Node.nodePrefab = Resources.Load<GameObject>("Node");

        Node.btMat = Resources.Load<Material>("BTMat");
        Node.llMat = Resources.Load<Material>("LLMat");
        Node.treeMat = Resources.Load<Material>("TreeMat");

        int[] test = { 1, 2, 3, 4, 5 };
        //LinkedList.fromArr(test, new Vector3(0,2,5));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
