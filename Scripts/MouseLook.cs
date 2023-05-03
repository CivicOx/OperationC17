using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    /*This script works with the mouse input by gathering x and y data and transferring that to
    player and camera rotation that feels natural for the player*/
    
    public float mouseSensitivity = 200f;

    public Transform playerBody;
    public float xAnimation;

    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //remove the cursor from the scene
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //get x and y mouse input data
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //clamp camera rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        //rotate the player based on x
        playerBody.Rotate(Vector3.up * mouseX);

        //works with animator for breathing mechanics
        playerBody.rotation = Quaternion.Euler(xAnimation, playerBody.rotation.eulerAngles.y, 0f);
    }
}
