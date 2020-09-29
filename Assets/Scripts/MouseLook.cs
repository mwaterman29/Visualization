using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float lookSpeed;
    public float minLook, maxLook;
    private Vector2 rotation;
    public static Camera main;
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        rotation = Vector2.zero;
        Cursor.lockState = CursorLockMode.Locked;
        main = Camera.main;
        player = GetComponentInParent<Player>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.inEdit)
            return;

        rotation.y += lookSpeed * Input.GetAxis("Mouse X");
        rotation.x += lookSpeed * -Input.GetAxis("Mouse Y");

        rotation.x = Mathf.Clamp(rotation.x, minLook, maxLook);
        transform.eulerAngles = rotation;
    }

    public static Vector3 getCamForward()
    {
        return main.transform.forward;
    }

    public static Vector3 getCamPosition()
    {
        return main.transform.position;
    }

    public static Transform getCamTransform()
    {
        return main.transform;
    }

    public static Quaternion getCamRotation()
    {
        return main.transform.rotation;
    }
}
