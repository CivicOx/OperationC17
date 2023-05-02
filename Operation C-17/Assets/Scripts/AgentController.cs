using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    /*This script works to rotate the enemies head at the player*/
    
    public Transform head;
    public GameObject playerCamera;

    void Start() {
        playerCamera = GameObject.Find("Main Camera");
    }
    
    // Update is called once per frame
    void Update()
    {
        //track head to player
        head.LookAt(playerCamera.transform);
        float yRotation = head.localEulerAngles.y;

        //clamp head rotation
        if(yRotation > 70f && yRotation < 290f) {
            if(yRotation > 180f) {
                //clamp at 290
                head.localRotation = Quaternion.Euler(head.localEulerAngles.x, 290f, head.localEulerAngles.z);
            } else {
                //clamp at 70
                head.localRotation = Quaternion.Euler(head.localEulerAngles.x, 70f, head.localEulerAngles.z);
            }
        }  
    }
}
